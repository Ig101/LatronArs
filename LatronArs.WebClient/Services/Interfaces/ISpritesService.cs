using System.Drawing;
using System.Threading.Tasks;
using LatronArs.Engine.Scene.Components;
using Microsoft.AspNetCore.Components;

namespace LatronArs.WebClient.Services.Interfaces
{
    public interface ISpritesService
    {
        int SpriteWidth { get; }

        int SpriteHeight { get; }

        int Width { get; }

        bool TexturesBuilt { get; }

        (Point position, bool mirrored) GetSpritePositionByDefinition(SpriteDefinition definition);

        Task BuildSpriteTexturesAsync(ElementReference canvas);

        Task SetupTexturesAsync();
    }
}