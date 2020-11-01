using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        private const int DefaultWidth = 1024;
        private const int DefaultHeight = 768;
        private const int DefaultAspectRatio = DefaultWidth / DefaultHeight;
        private const int TileSize = 62;
        private const int TileOffset = 16;

        [Inject]
        private IGameService GameService { get; set; }

        [Inject]
        private ISpritesService SpritesService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ResizeService ResizeService { get; set; }

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected ElementReference PictureCanvasRef { get; set; }

        protected int CanvasWidth { get; set; }

        protected int CanvasHeight { get; set; }

        private double zoom;
        private Stopwatch updatingStopwatch;
        private Timer timer;
        private BoundingClientRect canvasSize;
        private float[] textureMapping;
        private float[] backgroundTextureMapping;
        private float[] maskPositions;
        private float[] vertexes;
        private byte[] colors;
        private byte[] backgroundColors;
        private byte[] masks;

        private double CameraX { get; set; }

        private double CameraY { get; set; }

        private int InterfaceShift { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine(firstRender);
            if (firstRender)
            {
                ResizeService.OnResize += SetupAspectRatio;

                var vertexShader = await HttpClient.GetStringAsync("shaders/vertex-shader-2d.vert");
                var fragmentShader = await HttpClient.GetStringAsync("shaders/fragment-shader-2d.frag");
                await JSRuntime.InvokeAsync<object>("sceneExtensions.registerWebGLContext", PictureCanvasRef, int.Parse(PictureCanvasRef.Id), vertexShader, fragmentShader);
                await SpritesService.BuildSpriteTexturesAsync(PictureCanvasRef);
                updatingStopwatch = new Stopwatch();
                await SetupAspectRatio();

                timer = new Timer(
                    state => ((SceneComponent)state).Redraw(),
                    this,
                    0,
                    33);
            }
        }

        private async Task SetupAspectRatio()
        {
            canvasSize = await JSRuntime.InvokeAsync<BoundingClientRect>("DOMGetBoundingClientRect", PictureCanvasRef);
            var newAspectRatio = canvasSize.Width / canvasSize.Height;
            if (newAspectRatio < DefaultAspectRatio)
            {
                CanvasWidth = DefaultWidth;
                CanvasHeight = (int)(DefaultWidth / newAspectRatio);
                zoom = canvasSize.Width / CanvasWidth;
            }
            else
            {
                CanvasWidth = (int)(DefaultHeight * newAspectRatio);
                CanvasHeight = DefaultHeight;
                zoom = canvasSize.Height / CanvasHeight;
            }

            StateHasChanged();

            if (GameService.CurrentScene != null)
            {
                GameService.CurrentScene.Changed = true;
            }

            Redraw();
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
                    State = AIState.Neutral
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
                },
                texturePosition);
            WebGLHelper.FillLight(backgroundColors, 255, 255, 255, texturePosition);
        }

        private void FillPoint(
            int x,
            int y,
            int texturePosition)
        {
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
                        memoryVal.HasItems,
                        !memoryVal.Visible,
                        false,
                        texturePosition);
                }
                else
                {
                    FillEmptyActor(texturePosition, memoryVal.HasItems);
                }

                if (memoryVal.Visible)
                {
                    var tile = GameService.CurrentScene.Tiles[x][y];
                    var light = GameService.CurrentScene.LightMap[x][y];
                    var tileSprite = new Engine.Scene.Components.SpriteDefinition
                    {
                        Name = tile.Sprite,
                        Direction = Direction.Right,
                        State = AIState.Neutral
                    };
                    WebGLHelper.FillSprite(
                        SpritesService,
                        backgroundTextureMapping,
                        tileSprite,
                        texturePosition);
                    WebGLHelper.FillLight(backgroundColors, (byte)(light.Color.R * light.Power), (byte)(light.Color.G * light.Power), (byte)(light.Color.B * light.Power), texturePosition);
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

        private void Redraw()
        {
            var time = updatingStopwatch.ElapsedMilliseconds;
            updatingStopwatch.Restart();
            if (GameService.CurrentScene == null || !SpritesService.TexturesBuilt)
            {
                return;
            }

            var scene = GameService.CurrentScene;

            if (scene.Changed && scene.Player != null)
            {
                CameraX = scene.Player.Parent.X + 0.5;
                CameraY = scene.Player.Parent.Y - 0.5;
            }

            var cameraLeft = CameraX - (canvasSize.Width / 2 / TileSize / zoom);
            var cameraTop = CameraY - (canvasSize.Height / 2 / TileSize / zoom);

            var left = (int)(cameraLeft - 1);
            var right = (int)(cameraLeft + (canvasSize.Width / TileSize / zoom) + 2);
            var top = (int)(cameraTop - 1);
            var bottom = (int)(cameraTop + (canvasSize.Height / TileSize / zoom) + 2);
            var width = right - left + 1;
            var height = bottom - top + 1;

            if (scene.Changed && scene.Player != null)
            {
                textureMapping = new float[width * height * 12];
                colors = new byte[width * height * 4];
                masks = new byte[width * height * 4];
                backgroundTextureMapping = new float[width * height * 12];
                backgroundColors = new byte[width * height * 4];
                maskPositions = new float[width * height * 12];
                vertexes = new float[width * height * 12];
                var texturePosition = 0;
                for (var y = top; y <= bottom; y++)
                {
                    for (var x = left; x <= right; x++)
                    {
                        WebGLHelper.FillVertexPosition(vertexes, maskPositions, x, y, left, top, TileSize, TileSize, TileOffset, texturePosition);
                        if (x >= 0 && y >= 0 && x < scene.Width && y < scene.Height)
                        {
                            FillPoint(x, y, texturePosition);
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

            WebGLHelper.DrawArrays(
                JSRuntime,
                PictureCanvasRef,
                vertexes,
                maskPositions,
                textureMapping,
                backgroundTextureMapping,
                colors,
                backgroundColors,
                masks,
                (right - left + 1) * TileSize,
                (bottom - top + 1) * TileSize,
                (int)((left - cameraLeft) * TileSize),
                (int)((top - cameraTop) * TileSize),
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