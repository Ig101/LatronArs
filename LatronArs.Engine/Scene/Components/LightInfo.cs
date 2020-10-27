using System.Drawing;

namespace LatronArs.Engine.Scene.Components
{
    public class LightInfo
    {
        public double Power { get; init; }

        public Color Color { get; init; }

        public double TurnOffCostModifier { get; init; }
    }
}