namespace LatronArs.Engine.Scene.Components
{
    public class TreasureInfo
    {
        public string Id { get; init; }

        public int Value { get; set; }

        public double PickupTimeCost { get; init; }

        public double PickupNoise { get; init; }

        public bool Shines { get; init; }
    }
}