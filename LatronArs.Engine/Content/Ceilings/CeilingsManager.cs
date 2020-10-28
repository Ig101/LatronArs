using System.Collections.Generic;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content.Ceilings
{
    public static class CeilingsManager
    {
        public static Dictionary<string, Ceiling> GetCeilings()
        {
            return new Dictionary<string, Ceiling>
            {
                {
                    "chindalerie", new Ceiling
                    {
                        Id = "chindalerie",
                        Light = new LightInfo { Power = 8, Color = new Scene.Objects.Structs.Color { R = 255, G = 255, B = 100 }, TurnOffCostModifier = 5 },
                        InteractionReaction = ActionsManager.SwitchLight
                    }
                }
            };
        }
    }
}