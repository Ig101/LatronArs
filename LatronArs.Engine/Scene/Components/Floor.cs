using System;
using System.Drawing;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Scene.Components
{
    public class Floor
    {
        public string Id { get; init; }

        public string Sprite { get; init; }

        public Color Color { get; init; }

        public Action<Scene, Tile, Actor> InteractionReaction { get; init; }

        public Action<Scene, Tile, Actor> StepReaction { get; init; }
    }
}