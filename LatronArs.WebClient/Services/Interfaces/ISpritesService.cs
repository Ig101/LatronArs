using System.Drawing;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.WebClient.Services.Interfaces
{
    public interface ISpritesService
    {
        int SpriteWidth { get; }

        int SpriteHeight { get; }

        int Width { get; }

        (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition);

        (WebGLTexture texture, WebGLTexture mask) GetSpriteTextures(WebGLContext gl);
    }
}