using System;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Scene.Components
{
    public class ActionInfo
    {
        public double NoiseModifier { get; init; }

        public int TimeCost { get; init; }

        public Action<Scene, Actor, ActionInfo> Action { get; init; }
    }
}