using System.Collections.Generic;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content.Floors
{
    public class FloorsManager
    {
        public static Dictionary<string, Floor> GetFloors()
        {
            return new Dictionary<string, Floor>
            {
                {
                    "floor", new Floor
                    {
                        Id = "floor",
                        Sprite = "floor",
                        NoiseMultiplier = 1,
                        StepReaction = null
                    }
                },
                {
                    "stone", new Floor
                    {
                        Id = "stone",
                        Sprite = "stoneFloor",
                        NoiseMultiplier = 2,
                        StepReaction = null
                    }
                }
            };
        }
    }
}