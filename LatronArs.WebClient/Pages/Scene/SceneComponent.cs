using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Pages.Scene
{
    public class SceneComponent : ComponentBase, IDisposable
    {
        private static readonly int DefaultWidth = 1600;
        private static readonly int DefaultHeight = 1080;
        private static readonly int DefaultAspectRatio = DefaultWidth / DefaultHeight;

        [Inject]
        private GameService GameService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private ResizeService ResizeService { get; set; }

        protected BECanvasComponent PictureCanvasRef { get; set; }

        protected BECanvasComponent HudCanvasRef { get; set; }

        protected ElementReference PictureContainer { get; set; }

        private WebGLContext _pictureContext;
        private Canvas2DContext _hudContext;

        private double zoom;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ResizeService.OnResize += SetupAspectRatio;

            _pictureContext = await PictureCanvasRef.CreateWebGLAsync();
            _hudContext = await HudCanvasRef.CreateCanvas2DAsync();

            await SetupAspectRatio();
        }

        private async Task SetupAspectRatio()
        {
            var rect = await JSRuntime.InvokeAsync<BoundingClientRect>("DOMGetBoundingClientRect", PictureContainer);
            var newAspectRatio = rect.Width / rect.Height;
            var canvasSize = newAspectRatio < DefaultAspectRatio ?
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
            await PictureCanvasRef.SetParametersAsync(ParameterView.FromDictionary(canvasSize));
            await HudCanvasRef.SetParametersAsync(ParameterView.FromDictionary(canvasSize));
            zoom = rect.Width / (long)canvasSize["Width"];
            if (GameService.CurrentScene != null)
            {
                GameService.CurrentScene.Changed = true;
            }

            await Redraw();
        }

        private async Task Redraw()
        {
            if (GameService.CurrentScene == null)
            {
                return;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ResizeService.OnResize -= SetupAspectRatio;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}