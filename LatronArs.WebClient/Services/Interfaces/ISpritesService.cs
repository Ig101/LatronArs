using System.Drawing;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.WebClient.Services.Interfaces
{
    public interface ISpritesService
    {
        int SpriteWidth { get; }

        int SpriteHeight { get; }

        (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition);
    }
}