using System.Drawing;
using Blazor.Extensions.Canvas.WebGL;
using LatronArs.Engine.Scene.Components;
using LatronArs.WebClient.Services.Interfaces;

namespace LatronArs.WebClient.Services
{
    public class SpritesService : ISpritesService
    {
        public int SpriteWidth => 64;

        public int SpriteHeight => 80;

        public int Width { get; private set; }

        public (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition)
        {
            throw new System.NotImplementedException();
        }

        public (WebGLTexture texture, WebGLTexture mask) GetSpriteTextures(WebGLContext gl)
        {
            throw new System.NotImplementedException();
        }
    }
}