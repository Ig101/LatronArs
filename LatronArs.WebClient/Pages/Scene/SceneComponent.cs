using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Helpers;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services;
using LatronArs.WebClient.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Pages.Scene
{
    public partial class SceneComponent : ComponentBase, IDisposable
    {
        private const int DefaultWidth = 760;
        private const int DefaultHeight = 540;
        private const float DefaultAspectRatio = (float)DefaultWidth / DefaultHeight;
        private const int TileSize = 30;
        private const int TileOffset = 8;
        private const int InputDelay = 200;
        private const int WaitDelay = 100;
        private const int CaptionSymbolsCount = 29;

        [Inject]
        private IGameService GameService { get; set; }

        [Inject]
        private ISpritesService SpritesService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private EventsService ResizeService { get; set; }

        [Inject]
        private HttpClient HttpClient { get; set; }

        protected ElementReference PictureCanvasRef { get; set; }

        protected int CanvasWidth { get; set; }

        protected int CanvasHeight { get; set; }

        protected PickupModalData PickupModal { get; set; }

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

        private IEnumerable<KeyEventHandler<SceneComponent>> keyHandlers;
        private IList<KeyEventHandler<SceneComponent>> pressedHandlers;
        private bool shift;
        private bool alt;
        private bool ctrl;
        private long timeTillActionRepeat = 0;
        private long waitTimer = 2000;

        private double CameraX { get; set; }

        private double CameraY { get; set; }

        private int InterfaceShift { get; set; }

        private bool ModalOpened => PickupModal != null;

        private int PickupModalShift => (int)((canvasSize?.Width ?? CanvasWidth) * 0.3);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                EventsService.OnResize += SetupAspectRatio;
                EventsService.OnKeyDown += OnKeyDown;
                EventsService.OnKeyUp += OnKeyUp;

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

                pressedHandlers = new List<KeyEventHandler<SceneComponent>>();
                keyHandlers = new[]
                {
                    new KeyEventHandler<SceneComponent>
                    {
                        Codes = new[] { "KeyW", "ArrowUp" },
                        Direction = Direction.Top,
                        Action = SceneActions.MoveUp,
                        Hold = true,
                        ShiftAction = SceneActions.SprintUp,
                        ShiftHold = true,
                        CtrlAction = SceneActions.PickupUp,
                        AltAction = SceneActions.InteractUp
                    },
                    new KeyEventHandler<SceneComponent>
                    {
                        Codes = new[] { "KeyS", "ArrowDown" },
                        Direction = Direction.Bottom,
                        Action = SceneActions.MoveDown,
                        Hold = true,
                        ShiftAction = SceneActions.SprintDown,
                        ShiftHold = true,
                        CtrlAction = SceneActions.PickupDown,
                        AltAction = SceneActions.InteractDown
                    },
                    new KeyEventHandler<SceneComponent>
                    {
                        Codes = new[] { "KeyA", "ArrowLeft" },
                        Direction = Direction.Left,
                        Action = SceneActions.MoveLeft,
                        Hold = true,
                        ShiftAction = SceneActions.SprintLeft,
                        ShiftHold = true,
                        CtrlAction = SceneActions.PickupLeft,
                        AltAction = SceneActions.InteractLeft
                    },
                    new KeyEventHandler<SceneComponent>
                    {
                        Codes = new[] { "KeyD", "ArrowRight" },
                        Direction = Direction.Right,
                        Action = SceneActions.MoveRight,
                        Hold = true,
                        ShiftAction = SceneActions.SprintRight,
                        ShiftHold = true,
                        CtrlAction = SceneActions.PickupRight,
                        AltAction = SceneActions.InteractRight
                    },
                    new KeyEventHandler<SceneComponent>
                    {
                        Codes = new[] { "Space" },
                        Action = SceneActions.PickupCenter,
                        ShiftAction = SceneActions.PickupCenter,
                        CtrlAction = SceneActions.PickupCenter,
                        AltAction = SceneActions.InteractCenter
                    }
                };
            }
        }

        private void UnexpectedRedraw()
        {
            StateHasChanged();

            if (GameService.CurrentScene != null)
            {
                GameService.CurrentScene.Changed = true;
            }

            Redraw();
        }

        private bool MakeAction(KeyEventHandler<SceneComponent> handler, bool hold)
        {
            if (PickupModal != null && hold)
            {
                return true;
            }

            if (ctrl && handler.CtrlAction != null && handler.CtrlHold == hold)
            {
                handler.CtrlAction(this);
                waitTimer = WaitDelay;
                return true;
            }

            if (alt && !ctrl && handler.AltAction != null && handler.AltHold == hold)
            {
                handler.AltAction(this);
                waitTimer = WaitDelay;
                return true;
            }

            if (shift && !alt && !ctrl && handler.ShiftAction != null && handler.ShiftHold == hold)
            {
                handler.ShiftAction(this);
                waitTimer = WaitDelay;
                return true;
            }

            if (!alt && !ctrl && !shift && handler.Action != null && handler.Hold == hold)
            {
                handler.Action(this);
                waitTimer = WaitDelay;
                return true;
            }

            return false;
        }

        private static string MakeCaptionFromActorName(string name)
        {
            StringBuilder sb = new StringBuilder();
            var symbols = (CaptionSymbolsCount - name.Length) / 2;
            for (var i = 0; i < symbols; i++)
            {
                sb.Append('-');
            }

            sb.Append(name);
            for (var i = 0; i < symbols; i++)
            {
                sb.Append('-');
            }

            return sb.ToString();
        }

        private void OpenPickupWindow(int x, int y)
        {
            var items = GameService.CurrentScene.GetPickupItems(x, y);
            var actorItems = items.Where(x => x.target != null).ToList();
            var pickupModalData = new PickupModalData
            {
                ActorName = actorItems.Count > 0 ? MakeCaptionFromActorName(actorItems[0].target.Name) : null,
                ActorTreasures = actorItems,
                FloorTreasures = items.Where(x => x.target == null).ToList(),
                X = x,
                Y = y
            };
            PickupModal = pickupModalData;
            InterfaceShift = PickupModalShift;
            UnexpectedRedraw();
        }

        protected void CollectTreasureOrReset((Actor target, Treasure treasure)? item)
        {
            if (item != null)
            {
                GameService.CurrentScene.Pickup(PickupModal.X, PickupModal.Y, item.Value.target, item.Value.treasure);
            }

            PickupModal = null;
            InterfaceShift = 0;
            UnexpectedRedraw();
        }

        private void OnKeyDown(KeyboardEvent e)
        {
            if (e.Code == "Escape")
            {
                return;
            }

            if (e.Code == "Digit0")
            {
                ctrl = true;
                return;
            }

            if (e.Code == "Minus")
            {
                alt = true;
                return;
            }

            if (e.Code == "Equal")
            {
                shift = true;
                return;
            }

            if (PickupModal != null)
            {
                return;
            }

            var action = keyHandlers.FirstOrDefault(x => x.Codes.Contains(e.Code));
            if (action != null && !pressedHandlers.Any(x => x == action))
            {
                if (MakeAction(action, true))
                {
                    timeTillActionRepeat = InputDelay * 2;
                }

                pressedHandlers.Add(action);
            }
        }

        private void OnKeyUp(KeyboardEvent e)
        {
            if (e.Code == "Escape")
            {
                // TODO escape
                return;
            }

            if (e.Code == "Digit0")
            {
                ctrl = false;
                return;
            }

            if (e.Code == "Minus")
            {
                alt = false;
                return;
            }

            if (e.Code == "Equal")
            {
                shift = false;
                return;
            }

            var action = keyHandlers.FirstOrDefault(x => x.Codes.Contains(e.Code));
            if (action != null)
            {
                var handler = pressedHandlers.FirstOrDefault(x => x == action);
                if (handler != null)
                {
                    pressedHandlers.Remove(handler);
                    MakeAction(handler, false);
                }
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
            }
            else
            {
                CanvasWidth = (int)(DefaultHeight * newAspectRatio);
                CanvasHeight = DefaultHeight;
            }

            zoom = canvasSize.Width / CanvasWidth;

            UnexpectedRedraw();
        }

        private void FillEmptyActor(
            int texturePosition,
            bool silhouette = false,
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
            WebGLHelper.FillColor(colors, masks, 0, 0, 0, 0, shines, silhouette, false, texturePosition);
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
                var sprite = memory.Sprite;
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
                        memory.HasItems,
                        !memory.Visible,
                        false,
                        texturePosition);
                }
                else
                {
                    FillEmptyActor(texturePosition, !memory.Visible, memory.HasItems);
                }

                if (memory.Visible)
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

        private void HandleUpdates(long time)
        {
            if (timeTillActionRepeat > 0)
            {
                timeTillActionRepeat -= time;
            }
            else if (pressedHandlers != null)
            {
                var done = false;
                var position = pressedHandlers.Count;
                while (!done && position > 0)
                {
                    position--;
                    done = MakeAction(pressedHandlers[position], true);
                }

                if (done)
                {
                    timeTillActionRepeat += InputDelay;
                }
            }

            if (!ModalOpened)
            {
                if (waitTimer > 0)
                {
                    waitTimer -= time;
                }
                else
                {
                    GameService.CurrentScene.Wait();
                    waitTimer += WaitDelay;
                }
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

            HandleUpdates(time);

            var scene = GameService.CurrentScene;

            if (scene.Changed && scene.Player != null)
            {
                if (PickupModal != null)
                {
                    CameraX = PickupModal.X + 0.5;
                    CameraY = PickupModal.Y - 0.5;
                }
                else
                {
                    CameraX = scene.Player.CurrentTile.X + 0.5;
                    CameraY = scene.Player.CurrentTile.Y - 0.5;
                }
            }

            var cameraLeft = CameraX - ((canvasSize.Width + InterfaceShift) / 2 / TileSize / zoom);
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
                EventsService.OnResize -= SetupAspectRatio;
                EventsService.OnKeyDown -= OnKeyDown;
                EventsService.OnKeyUp -= OnKeyUp;
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