using System.Collections.Generic;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content.Actors
{
    public static class ActorsManager
    {
        public static Dictionary<string, ActorInfo> GetActors()
        {
            return new Dictionary<string, ActorInfo>
            {
                {
                    "thief", new ActorInfo
                    {
                        Id = "thief",
                        Sprite = "thief",
                        Name = "You",
                        Color = new Scene.Objects.Structs.Color { R = 255, G = 255, B = 255 },
                        MoveAction = new ActionInfo
                        {
                            NoiseModifier = 0.1,
                            TimeCost = 600,
                            Action = ActionsManager.SimpleMove
                        },
                        SprintAction = new ActionInfo
                        {
                            NoiseModifier = 0.5,
                            TimeCost = 300,
                            Action = ActionsManager.SimpleMove
                        },
                        InteractAction = ActionsManager.PlayerInteraction,
                        InteractionReaction = null,
                        Light = null,
                        PickupTimeCost = 200,
                        PickupFromNoiseModifier = 1,
                        PickupFromTimeCostModifier = 1,
                        Hearing = 0.1,
                        Seeing = 0.1,
                        Omnidirectional = true,
                        LightTransmission = 0.9,
                        NoiseTransmission = 0.9
                    }
                },
                {
                    "guardian", new ActorInfo
                    {
                        Id = "guardian",
                        Sprite = "guardian",
                        Name = "Guard",
                        Color = new Scene.Objects.Structs.Color { R = 255, G = 255, B = 255 },
                        MoveAction = new ActionInfo
                        {
                            NoiseModifier = 0.4,
                            TimeCost = 800,
                            Action = ActionsManager.SimpleMove
                        },
                        SprintAction = new ActionInfo
                        {
                            NoiseModifier = 1,
                            TimeCost = 400,
                            Action = ActionsManager.SimpleMove
                        },
                        InteractAction = ActionsManager.GuardianInteraction,
                        InteractionReaction = null,
                        Light = null,
                        PickupTimeCost = 200,
                        PickupFromNoiseModifier = 1,
                        PickupFromTimeCostModifier = 2,
                        Hearing = 0.4,
                        Seeing = 0.4,
                        Omnidirectional = false,
                        LightTransmission = 0.9,
                        NoiseTransmission = 0.9
                    }
                },
                {
                    "counter", new ActorInfo
                    {
                        Id = "counter",
                        Sprite = "counter",
                        Name = "Counter",
                        Color = new Scene.Objects.Structs.Color { R = 255, G = 255, B = 255 },
                        MoveAction = null,
                        SprintAction = null,
                        InteractAction = null,
                        InteractionReaction = null,
                        Light = null,
                        PickupTimeCost = 1000,
                        PickupFromNoiseModifier = 1.5,
                        PickupFromTimeCostModifier = 1.3,
                        Hearing = 1,
                        Seeing = 1,
                        Omnidirectional = true,
                        LightTransmission = 0.7,
                        NoiseTransmission = 0.8
                    }
                },
                {
                    "torch", new ActorInfo
                    {
                        Id = "torch",
                        Sprite = "torch",
                        Name = "Torch",
                        Color = new Scene.Objects.Structs.Color { R = 255, G = 255, B = 255 },
                        MoveAction = null,
                        SprintAction = null,
                        InteractAction = null,
                        InteractionReaction = ActionsManager.SwitchLight,
                        Light = null,
                        PickupTimeCost = 1000,
                        PickupFromNoiseModifier = 100,
                        PickupFromTimeCostModifier = 2,
                        Hearing = 1,
                        Seeing = 1,
                        Omnidirectional = true,
                        LightTransmission = 1,
                        NoiseTransmission = 1
                    }
                }
            };
        }
    }
}