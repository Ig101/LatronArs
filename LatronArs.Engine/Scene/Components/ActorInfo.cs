using System;
using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;

namespace LatronArs.Engine.Scene.Components
{
    public class ActorInfo
    {
        public string Id { get; init; }

        public string Sprite { get; init; }

        public string Name { get; init; }

        public Color Color { get; init; }

        public ActionInfo MoveAction { get; init; }

        public ActionInfo SprintAction { get; init; }

        public Action<Tile, Actor> InteractAction { get; init; }

        public Func<Actor, Actor, int> InteractionReaction { get; init; }

        public LightInfo Light { get; init; }

        public int PickupTimeCost { get; init; }

        public double PickupFromTimeCostModifier { get; init; }

        public double PickupFromNoiseModifier { get; init; }

        public double Hearing { get; init; }

        public double Seeing { get; init; }

        public bool Omnidirectional { get; init; }

        public double LightTransmission { get; init; }

        public double NoiseTransmission { get; init; }
    }
}