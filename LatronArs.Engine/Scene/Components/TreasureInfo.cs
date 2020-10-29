namespace LatronArs.Engine.Scene.Components
{
    public class TreasureInfo
    {
        public string Id { get; init; }

        public int Value { get; init; }

        public string Name { get; init; }

        public double PickupTimeCostModifier { get; init; }

        public double PickupNoise { get; init; }

        public bool Shines { get; init; }
    }
}