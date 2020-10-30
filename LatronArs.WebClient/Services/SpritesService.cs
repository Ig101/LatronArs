using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Engine.Scene.Components;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services.Interfaces;
using LatronArs.WebClient.Sprites;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Services
{
    public class SpritesService : ISpritesService, IDisposable
    {
        private IJSRuntime _jsRuntime;

        public int SpriteWidth => 62;

        public int SpriteHeight => 78;

        public int Width { get; private set; }

        public int Height { get; private set; }

        private IDictionary<string, IEnumerable<SpriteVariation>> _spritesList;

        private AutoResetEvent _mutex;

        public SpritesService(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
            _mutex = new AutoResetEvent(false);
        }

        private async Task AppendFullImage(string path, bool mask, int startingPosition)
        {
            await _jsRuntime.InvokeAsync<object>("texture.drawImage", path + ".png", (SpriteWidth + 2) * startingPosition, 0);
            if (mask)
            {
                await _jsRuntime.InvokeAsync<object>("texture.drawImage", path + ".mask.png", (SpriteWidth + 2) * startingPosition, SpriteHeight + 2);
            }
        }

        private async Task AppendSquareImage(string path, int startingPosition)
        {
            await _jsRuntime.InvokeAsync<object>("texture.drawImage", path + ".png", (SpriteWidth + 2) * startingPosition, SpriteHeight - SpriteWidth);
        }

        private void FillSpritesList()
        {
            _spritesList = new Dictionary<string, IEnumerable<SpriteVariation>>();
            _spritesList.FillSpritesCollection();
        }

        public async Task SetupTexturesAsync()
        {
            Height = SpriteHeight + 2;
            FillSpritesList();
            var sprites = _spritesList.Values.SelectMany(x => x);
            Width = (SpriteWidth + 2) * sprites.Count();
            await _jsRuntime.InvokeAsync<object>("texture.createCanvas", Height * 2, Width);
            var startingPosition = 0;
            foreach (var sprite in sprites)
            {
                if (sprite.IsSquare)
                {
                    await AppendSquareImage(sprite.Path, startingPosition);
                }
                else
                {
                    await AppendFullImage(sprite.Path, sprite.Mask, startingPosition);
                }

                sprite.X = ((SpriteWidth + 2) * startingPosition) + 1;
                sprite.Y = 1;

                startingPosition++;
            }

            _mutex.Set();
        }

        public (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition)
        {
            var variationsList = _spritesList[definition.Name];
            var neededVariation = variationsList
                .First(x =>
                    (x.Direction == null || definition.Direction == x.Direction || (definition.Direction == Direction.Left && x.Mirrored)) &&
                    (x.State == null || definition.State == x.State));
            return (position: new Point { X = neededVariation.X, Y = neededVariation.Y }, mirrored: definition.Direction == Direction.Left && neededVariation.Mirrored);
        }

        public async Task<(WebGLTexture texture, WebGLTexture mask)> GetSpriteTexturesAsync(WebGLContext gl)
        {
            await Task.Run(() => _mutex.WaitOne());
            var textureData = await _jsRuntime.InvokeAsync<ImageData>("texture.getTexture", 0, 0, Width, Height);
            var maskData = await _jsRuntime.InvokeAsync<ImageData>("texture.getTexture", 0, Height, Width, Height);

            var texture = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, texture);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 0x2600);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 0x2600);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, Width, Height, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, textureData.Data);

            var mask = await gl.CreateTextureAsync();
            await gl.BindTextureAsync(TextureType.TEXTURE_2D, mask);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_S, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_WRAP_T, 0x812F);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MAG_FILTER, 0x2600);
            await gl.TexParameterAsync(TextureType.TEXTURE_2D, TextureParameter.TEXTURE_MIN_FILTER, 0x2600);
            await gl.TexImage2DAsync(Texture2DType.TEXTURE_2D, 0, PixelFormat.RGBA, Width, Height, PixelFormat.RGBA, PixelType.UNSIGNED_BYTE, maskData.Data);

            return (texture, mask);
        }

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {
                _mutex.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}