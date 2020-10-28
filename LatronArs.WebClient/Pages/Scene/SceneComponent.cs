using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services;
using LatronArs.WebClient.Services.Interfaces;
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
        private IGameService GameService { get; set; }

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
        private Stopwatch updatingStopwatch;
        private Timer timer;

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
            var time = updatingStopwatch.ElapsedMilliseconds;
            updatingStopwatch.Restart();
            if (GameService.CurrentScene == null)
            {
                return;
            }

            /*
            if (this.scene && this.shadersProgram) {

      if (shift) {
        if (this.directionTimer > 0) {
          this.directionTimer -= shift;
        } else {
          for (const action of this.smartDirections) {
            if (action.pressed) {
              action.action();
              break;
            }
          }
        }
        this.tickerTime -= shift;
        if (this.tickerTime <= 0) {
          this.tickerState = !this.tickerState;
          this.tickerTime += this.tickerPeriod;
        }
      }

      const time = performance.now();
      const sceneRandom = new Random(this.scene.hash);
      const cameraLeft = this.cameraX - (this.canvasWidth - this.interfaceShift + this.leftInterfaceShift) / 2 / this.tileWidth;
      const cameraTop = this.cameraY - this.canvasHeight / 2 / this.tileHeight;
      const left = Math.floor(cameraLeft) - 1;
      const right = Math.ceil(cameraLeft + this.canvasWidth / (this.tileWidth)) + 1;
      const top = Math.floor(cameraTop) - 1;
      const bottom = Math.ceil(cameraTop + this.canvasHeight / (this.tileHeight)) + 1;
      const width = right - left + 1;
      const height = bottom - top + 1;
      if (this.scene.visualizationChanged) {

        if (this.rangeMapIsActive) {
          this.fillRangeMapAndPathes(
            this.scene.currentActor.z + this.scene.currentActor.height,
            this.scene.currentActor.x,
            this.scene.currentActor.y,
            cameraLeft,
            cameraTop);
        }

        this.textureMapping = new Float32Array(width * height * 12);
        this.colors = new Uint8Array(width * height * 4);
        this.activities = new Uint8Array(width * height);
        this.backgroundTextureMapping = new Float32Array(width * height * 12);
        this.backgrounds = new Uint8Array(width * height * 4);
        this.mainTextureVertexes = new Float32Array(width * height * 12);
        let texturePosition = 0;
        this.healthActors = [];
        for (let y = -20; y <= 60; y++) {
          for (let x = -40; x <= 80; x++) {
            if (x >= left && y >= top && x <= right && y <= bottom) {
              fillVertexPosition(this.mainTextureVertexes, x, y, left, top, this.tileWidth, this.tileHeight, texturePosition);
              if (x >= 0 && y >= 0 && x < this.scene.width && y < this.scene.height) {
                sceneRandom.next();
                const visibleActor = this.drawPoint(x, y, texturePosition,
                  this.colors, this.activities, this.textureMapping,
                  this.backgrounds, this.backgroundTextureMapping);
                if (visibleActor && visibleActor.tags.includes('active') && visibleActor.maxDurability) {
                  this.healthActors.push(visibleActor);
                }
              } else {
                const biom = getRandomBiom(sceneRandom, this.scene.biom);
                this.drawDummyPoint(x, y, biom.char, biom.color, texturePosition,
                  this.colors, this.activities, this.textureMapping, this.backgrounds, this.backgroundTextureMapping);
              }
              texturePosition++;
            } else {
              sceneRandom.next();
            }
          }
        }
        this.cursorVertexes = undefined;
      }

      this.redPath = new Path2D();
      this.greenPath = new Path2D();
      for (const actor of this.healthActors) {
        if (!this.selectedActor || this.selectedActor.x !== actor.x || this.selectedActor.y !== actor.y) {
          this.drawHealth(actor, Math.round((actor.x - cameraLeft) * this.tileWidth), Math.round((actor.y - cameraTop) * this.tileHeight));
        }
      }

      if (this.cursorVertexes) {
        for (let i = 0; i < 12; i++) {
          this.textureMapping[this.cursorVertexes.position * 12 + i] = this.cursorVertexes.textureMapping[i];
        }
        for (let i = 0; i < 4; i++) {
          this.colors[this.cursorVertexes.position * 4 + i] = this.cursorVertexes.colors[i];
        }
        this.activities[this.cursorVertexes.position] = this.cursorVertexes.activity;
        this.cursorVertexes = undefined;
      }

      const mouseX = Math.floor(this.mouseState.x);
      const mouseY = Math.floor(this.mouseState.y);

      const rangeMapPositionX = mouseX - this.scene.currentActor.x + RANGED_RANGE;
      const rangeMapPositionY = mouseY - this.scene.currentActor.y + RANGED_RANGE;
      if (this.selectedActor) {
        const position = (this.selectedActor.y - top) * width + (this.selectedActor.x - left);
        this.cursorVertexes = {
          position,
          textureMapping: this.textureMapping.slice(position * 12, position * 12 + 12),
          colors: this.colors.slice(position * 4, position * 4 + 4),
          activity: this.activities[position]
        };
        fillColor(this.colors, this.activities, 255, 255, 0, 1, true, position);
        fillChar(this.charsService, this.textureMapping, this.selectedActor.char, position, this.selectedActor.left);
        this.drawHealth(
          this.selectedActor,
          Math.round((this.selectedActor.x - cameraLeft) * this.tileWidth),
          Math.round((this.selectedActor.y - cameraTop) * this.tileHeight));
      } else if (this.canAct &&
        this.rangeMapIsActive &&
        rangeMapPositionX >= 0 &&
        rangeMapPositionY >= 0 &&
        rangeMapPositionX < RANGED_RANGE * 2 + 1 &&
        rangeMapPositionY < RANGED_RANGE * 2 + 1 &&
        this.rangeMap[rangeMapPositionX][rangeMapPositionY] !== undefined) {

        const value = this.rangeMap[rangeMapPositionX][rangeMapPositionY];
        if (value !== undefined) {
          const position = (mouseY - top) * width + (mouseX - left);
          this.cursorVertexes = {
            position,
            textureMapping: this.textureMapping.slice(position * 12, position * 12 + 12),
            colors: this.colors.slice(position * 4, position * 4 + 4),
            activity: this.activities[position]
          };
          fillColor(this.colors, this.activities, 255, 255, 0, 1, true, position);
          const actors = this.scene.tiles[mouseX][mouseY].actors;
          if (actors.length === 0 || actors[actors.length - 1].tags.includes('tile')) {
            fillChar(this.charsService, this.textureMapping, 'x', position, false);
          }
        }
      }

      drawArrays(
        this.canvasWebGLContext,
        this.shadersProgram,
        this.mainTextureVertexes,
        this.colors,
        this.backgrounds,
        this.textureMapping,
        this.backgroundTextureMapping,
        this.activities,
        this.charsTexture,
        Math.round((left - cameraLeft) * this.tileWidth),
        Math.round((top - cameraTop - 1) * this.tileHeight),
        (right - left + 1) * this.tileWidth,
        (bottom - top + 1) * this.tileHeight,
        (right - left + 1),
        (bottom - top + 1),
        this.charsService.width,
        this.charsService.spriteHeight);
      this.canvas2DContext.clearRect(0, 0, this.canvasWidth, this.canvasHeight);
      this.canvas2DContext.lineWidth = 4;
      this.canvas2DContext.strokeStyle = 'rgba(255, 0, 0, 1.0)';
      this.canvas2DContext.stroke(this.redPath);
      this.canvas2DContext.strokeStyle = 'rgba(0, 255, 0, 1.0)';
      this.canvas2DContext.stroke(this.greenPath);

      if (this.rangeMapIsActive && (this.canActNoBlocked)) {
        this.canvas2DContext.lineWidth = 2;
        this.canvas2DContext.strokeStyle = 'rgba(255, 255, 0, 1.0)';
        this.canvas2DContext.stroke(this.allowedTargetPath);
      }

      this.scene.visualizationChanged = false;

            */
            GameService.CurrentScene.Changed = false;
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