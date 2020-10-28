using System;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;

namespace LatronArs.Engine.Scene.Components
{
    public class Floor
    {
        public string Id { get; init; }

        public string Sprite { get; init; }

        public Color Color { get; init; }

        public double NoiseMultiplier { get; init; }

        public Action<Tile, Actor> StepReaction { get; init; }
    }
}