using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Engine.Scene.Components;
using Microsoft.AspNetCore.Components;

namespace LatronArs.WebClient.Services.Interfaces
{
    public interface ISpritesService
    {
        int SpriteWidth { get; }

        int SpriteHeight { get; }

        int Width { get; }

        (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition);

        Task<(WebGLTexture texture, WebGLTexture mask)> GetSpriteTexturesAsync(WebGLContext gl);

        Task SetupTexturesAsync();
    }
}