using System;
using System.Collections.Generic;
using System.Drawing;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Scene.Objects
{
    public class Actor : ITreasureBox
    {
        public Tile Parent { get; set; }

        public ActorInfo Info { get; set; }

        public ICollection<Treasure> Treasures { get; }

        public bool LightOn { get; set; }

        public int ActionDebt { get; set; }

        public int Team { get; set; }

        // Inherited
        public string Sprite => Info.Sprite;

        public Color Color => Info.Color;

        public ActionInfo MoveAction => Info.MoveAction;

        public ActionInfo SprintAction => Info.SprintAction;

        public ActionInfo InteractAction => Info.InteractAction;

        public Action<Scene, Actor, Actor> InteractionReaction => Info.InteractionReaction;

        public LightInfo Light => Info.Light;

        public int PickupTimeCost => Info.PickupTimeCost;

        public double PickupFromTimeCostModifier => Info.PickupFromTimeCostModifier;

        public double PickupFromNoiseModifier => Info.PickupFromNoiseModifier;

        public double Hearing => Info.Hearing;

        public double Seeing => Info.Seeing;

        public bool Omnidirectional => Info.Omnidirectional;

        public double LightTransmission => Info.LightTransmission;

        public double NoiseTransmission => Info.NoiseTransmission;
    }
}