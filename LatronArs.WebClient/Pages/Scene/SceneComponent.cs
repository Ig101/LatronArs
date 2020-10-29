using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Helpers;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services;
using LatronArs.WebClient.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Pages.Scene
{
    public class SceneComponent : ComponentBase, IDisposable
    {
        private const int DefaultWidth = 1600;
        private const int DefaultHeight = 1080;
        private const int DefaultAspectRatio = DefaultWidth / DefaultHeight;
        private const int TileSize = 64;
        private const int TileOffset = 16;

        [Inject]
        private IGameService GameService { get; set; }

        [Inject]
        private ISpritesService SpritesService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ResizeService ResizeService { get; set; }

        protected BECanvasComponent PictureCanvasRef { get; set; }

        protected BECanvasComponent HudCanvasRef { get; set; }

        protected ElementReference PictureContainer { get; set; }

        private WebGLContext _pictureContext;
        private Canvas2DContext _hudContext;
        private WebGLProgram _program;
        private WebGLTexture _spritesTexture;
        private WebGLTexture _masksTexture;

        private double zoom;
        private Stopwatch updatingStopwatch;
        private Timer timer;
        private BoundingClientRect canvasSize;
        private float[] textureMapping;
        private float[] backgroundTextureMapping;
        private float[] vertexes;
        private byte[] colors;
        private byte[] backgroundColors;
        private byte[] masks;

        private double CameraX { get; set; }

        private double CameraY { get; set; }

        private int InterfaceShift { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ResizeService.OnResize += SetupAspectRatio;

            _pictureContext = await PictureCanvasRef.CreateWebGLAsync();
            _hudContext = await HudCanvasRef.CreateCanvas2DAsync();

            await SetupAspectRatio();

            updatingStopwatch = new Stopwatch();
            timer = new Timer(
                state => ((SceneComponent)state).Redraw().Start(),
                this,
                0,
                33);
        }

        private async Task SetupAspectRatio()
        {
            canvasSize = await JSRuntime.InvokeAsync<BoundingClientRect>("DOMGetBoundingClientRect", PictureContainer);
            var newAspectRatio = canvasSize.Width / canvasSize.Height;
            var canvasSizeParams = newAspectRatio < DefaultAspectRatio ?
                new Dictionary<string, object>()
                {
                    { "Width", (long)DefaultWidth },
                    { "Height", (long)(DefaultWidth / newAspectRatio) }
                }
                :
                new Dictionary<string, object>()
                {
                    { "Width", (long)(DefaultHeight * newAspectRatio) },
                    { "Height", (long)DefaultHeight }
                };
            await PictureCanvasRef.SetParametersAsync(ParameterView.FromDictionary(canvasSizeParams));
            await HudCanvasRef.SetParametersAsync(ParameterView.FromDictionary(canvasSizeParams));
            zoom = canvasSize.Width / (long)canvasSizeParams["Width"];
            if (GameService.CurrentScene != null)
            {
                GameService.CurrentScene.Changed = true;
            }

            await Redraw();
        }

        private void FillEmptyActor(
            int texturePosition,
            bool shines = false)
        {
            WebGLHelper.FillSprite(
                SpritesService,
                textureMapping,
                new Engine.Scene.Components.SpriteDefinition
                {
                    Name = "empty",
                    Direction = Direction.Right,
                    State = AIState.Neutral,
                    HasItems = false
                },
                texturePosition);
            WebGLHelper.FillColor(colors, masks, 0, 0, 0, 0, shines, false, false, texturePosition);
        }

        private void FillEmptyTile(
            int texturePosition)
        {
            WebGLHelper.FillSprite(
                SpritesService,
                backgroundTextureMapping,
                new Engine.Scene.Components.SpriteDefinition
                {
                    Name = "empty",
                    Direction = Direction.Right,
                    State = AIState.Neutral,
                    HasItems = false,
                },
                texturePosition);
            WebGLHelper.FillBackground(colors, 0, 0, 0, texturePosition);
        }

        private void FillPoint(
            int x,
            int y,
            int texturePosition)
        {
            var tile = GameService.CurrentScene.Tiles[x][y];
            var memory = GameService.CurrentScene.Player.Memories[x][y];
            if (memory != null)
            {
                var memoryVal = memory.Value;
                var sprite = memoryVal.Sprite;
                if (sprite != null)
                {
                    WebGLHelper.FillSprite(
                        SpritesService,
                        textureMapping,
                        sprite,
                        texturePosition);
                    WebGLHelper.FillColor(
                        colors,
                        masks,
                        sprite.Color.R,
                        sprite.Color.G,
                        sprite.Color.B,
                        255,
                        sprite.HasItems,
                        !memoryVal.Visible,
                        false,
                        texturePosition);
                }
                else
                {
                    FillEmptyActor(texturePosition, sprite.HasItems);
                }

                if (memoryVal.Visible)
                {
                    var tileSprite = tile.Sprite;
                    WebGLHelper.FillSprite(
                        SpritesService,
                        backgroundTextureMapping,
                        tileSprite,
                        texturePosition);
                    WebGLHelper.FillBackground(colors, tileSprite.Color.R, tileSprite.Color.G, tileSprite.Color.B, texturePosition);
                }
                else
                {
                    FillEmptyTile(texturePosition);
                }
            }
            else
            {
                FillEmptyActor(texturePosition);
                FillEmptyTile(texturePosition);
            }
        }

        private async Task Redraw()
        {
            var time = updatingStopwatch.ElapsedMilliseconds;
            updatingStopwatch.Restart();
            if (GameService.CurrentScene == null)
            {
                return;
            }

            var scene = GameService.CurrentScene;

            if (scene.Changed && scene.Player != null)
            {
                CameraX = scene.Player.Parent.X + 0.5;
                CameraY = scene.Player.Parent.Y + 0.5;
            }

            var cameraLeft = CameraX - ((canvasSize.Width - InterfaceShift) / 2 / TileSize);
            var cameraTop = CameraY - (canvasSize.Height / 2 / TileSize);

            var left = (int)(cameraLeft - 1);
            var right = (int)(cameraLeft + (canvasSize.Width / TileSize) + 2);
            var top = (int)(cameraTop - 1);
            var bottom = (int)(cameraTop + (canvasSize.Height / TileSize) + 2);
            var width = right - left + 1;
            var height = bottom - top + 1;

            if (scene.Changed && scene.Player != null)
            {
                textureMapping = new float[width * height * 12];
                colors = new byte[width * height * 4];
                masks = new byte[width * height * 4];
                backgroundTextureMapping = new float[width * height * 12];
                backgroundColors = new byte[width * height * 4];
                vertexes = new float[width * height * 12];
                var texturePosition = 0;
                for (var y = top; y <= bottom; y++)
                {
                    for (var x = left; x <= right; x++)
                    {
                        WebGLHelper.FillVertexPosition(vertexes, x, y, left, top, TileSize, TileSize, TileOffset, texturePosition);
                        if (x >= 0 && y >= 0 && x < scene.Width && y < scene.Height)
                        {
                        }
                        else
                        {
                            FillEmptyTile(texturePosition);
                            FillEmptyActor(texturePosition);
                        }

                        texturePosition++;
                    }
                }
            }

            await WebGLHelper.DrawArrays(
                _pictureContext,
                _program,
                vertexes,
                textureMapping,
                backgroundTextureMapping,
                colors,
                backgroundColors,
                masks,
                _spritesTexture,
                _masksTexture,
                (int)((left - cameraLeft) * TileSize),
                (int)((top - cameraTop - 1) * TileSize),
                (right - left + 1) * TileSize,
                (bottom - top + 1) * TileSize,
                right - left + 1,
                bottom - top + 1,
                SpritesService.Width,
                SpritesService.SpriteHeight);

            scene.Changed = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ResizeService.OnResize -= SetupAspectRatio;
                timer.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}