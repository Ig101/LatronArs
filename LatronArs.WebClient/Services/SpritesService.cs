using System.Drawing;
using LatronArs.Engine.Scene.Components;
using LatronArs.WebClient.Services.Interfaces;

namespace LatronArs.WebClient.Services
{
    public class SpritesService : ISpritesService
    {
        public int SpriteWidth => 64;

        public int SpriteHeight => 80;

        public (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition)
        {
            throw new System.NotImplementedException();
        }
    }
}