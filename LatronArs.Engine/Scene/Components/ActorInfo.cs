using System;
using System.Collections.Generic;
using System.Drawing;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Scene.Components
{
    public class ActorInfo
    {
        public string Id { get; init; }

        public string Sprite { get; init; }

        public Color Color { get; init; }

        public ActionInfo MoveAction { get; init; }

        public ActionInfo SprintAction { get; init; }

        public ActionInfo InteractAction { get; init; }

        public Action<Scene, Actor, Actor> InteractionReaction { get; init; }

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