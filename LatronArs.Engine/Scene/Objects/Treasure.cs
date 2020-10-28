using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Scene.Objects
{
    public class Treasure
    {
        public TreasureInfo Info { get; set; }

        public int Amount { get; set; }

        // Inherited
        public string Id => Info.Id;

        public int Value => Info.Value;

        public double PickupTimeCost => Info.PickupTimeCost;

        public double PickupNoise => Info.PickupNoise;

        public bool Shines => Info.Shines;

        public Treasure(TreasureInfo info, int amount)
        {
            Info = info;
            Amount = amount;
        }
    }
}