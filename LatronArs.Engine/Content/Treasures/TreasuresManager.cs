using System.Collections.Generic;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content.Treasures
{
    public static class TreasuresManager
    {
        public static Dictionary<string, TreasureInfo> GetTreasures()
        {
            return new Dictionary<string, TreasureInfo>
            {
                {
                    "coin", new TreasureInfo
                    {
                        Id = "coin",
                        Name = "Coins",
                        Value = 1,
                        PickupTimeCostModifier = 0.5,
                        PickupNoise = 0.1,
                        Shines = true
                    }
                }
            };
        }
    }
}