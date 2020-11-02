using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LatronArs.Engine.Scene.Components;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Models;
using LatronArs.WebClient.Services.Interfaces;
using LatronArs.WebClient.Sprites;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LatronArs.WebClient.Services
{
    public class SpritesService : ISpritesService
    {
        private IJSRuntime _jsRuntime;
        private ICollection<Func<ValueTask<object>>> _buildTasks = new List<Func<ValueTask<object>>>();

        public int SpriteWidth => 30;

        public int SpriteHeight => 38;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public bool TexturesLoaded { get; private set; }

        public bool TexturesBuilt { get; private set; }

        private IDictionary<string, IEnumerable<SpriteVariation>> _spritesList;

        public SpritesService(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
            TexturesLoaded = false;
        }

        private async Task AppendFullImage(string path, bool mask, int startingPosition)
        {
            await _jsRuntime.InvokeAsync<object>("textureExtensions.drawTexture", path + ".png", (SpriteWidth + 2) * startingPosition, 0);
            if (mask)
            {
                await _jsRuntime.InvokeAsync<object>("textureExtensions.drawMask", path + ".mask.png", (SpriteWidth + 2) * startingPosition, 0);
            }
        }

        private async Task AppendSquareImage(string path, int startingPosition)
        {
            await _jsRuntime.InvokeAsync<object>("textureExtensions.drawTexture", path + ".png", (SpriteWidth + 2) * startingPosition, SpriteHeight - SpriteWidth);
        }

        private void FillSpritesList()
        {
            _spritesList = new Dictionary<string, IEnumerable<SpriteVariation>>();
            _spritesList.FillSpritesCollection();
        }

        public async Task SetupTexturesAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();
            Height = SpriteHeight + 2;
            FillSpritesList();
            var sprites = _spritesList.Values.SelectMany(x => x);
            Width = (SpriteWidth + 2) * sprites.Count();
            await _jsRuntime.InvokeAsync<object>("textureExtensions.createCanvas", Width, Height);
            var startingPosition = 0;
            var tasks = new List<Task>();
            foreach (var sprite in sprites)
            {
                if (sprite.IsSquare)
                {
                    tasks.Add(AppendSquareImage(sprite.Path, startingPosition));
                }
                else
                {
                    tasks.Add(AppendFullImage(sprite.Path, sprite.Mask, startingPosition));
                }

                sprite.X = ((SpriteWidth + 2) * startingPosition) + 1;
                sprite.Y = 1;

                startingPosition++;
            }

            await Task.WhenAll(tasks);
            TexturesLoaded = true;
            sw.Stop();
            foreach (var task in _buildTasks)
            {
                await task();
            }

            TexturesBuilt = true;
        }

        public (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition)
        {
            var variationsList = _spritesList[definition.Name];
            var neededVariation = variationsList
                .FirstOrDefault(x =>
                    (x.Direction == null || definition.Direction == x.Direction || (definition.Direction == Direction.Left && x.Mirrored)) &&
                    (x.State == null || definition.State == x.State));
            if (neededVariation != null)
            {
                return (position: new Point { X = neededVariation.X, Y = neededVariation.Y }, mirrored: definition.Direction == Direction.Left && neededVariation.Mirrored);
            }
            else
            {
                return GetSpritePositionByDefinition(new SpriteDefinition
                {
                    Name = "empty"
                });
            }
        }

        public async Task BuildSpriteTexturesAsync(ElementReference canvas)
        {
            var id = int.Parse(canvas.Id);
            if (this.TexturesLoaded)
            {
                await _jsRuntime.InvokeAsync<object>("textureExtensions.buildTextureAndMask", canvas, id);
            }
            else
            {
                _buildTasks.Add(() => _jsRuntime.InvokeAsync<object>("textureExtensions.buildTextureAndMask", canvas, id));
            }
        }
    }
}