using System;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Scene.Components
{
    public class Ceiling
    {
        public string Id { get; init; }

        public LightInfo Light { get; init; }

        public Func<Tile, Actor, int> InteractionReaction { get; init; }
    }
}