using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.AI;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;

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

        public List<Point> VisibleSquares { get; }

        public ActorAI AI { get; set; }

        public bool LightWorksAndOn => LightOn && Light != null && Light.Power > 0;

        // Inherited
        public string Sprite => Info.Sprite;

        public Color Color => Info.Color;

        public ActionInfo MoveAction => Info.MoveAction;

        public ActionInfo SprintAction => Info.SprintAction;

        public ActionInfo InteractAction => Info.InteractAction;

        public Action<Actor, Actor> InteractionReaction => Info.InteractionReaction;

        public LightInfo Light => Info.Light;

        public int PickupTimeCost => Info.PickupTimeCost;

        public double PickupFromTimeCostModifier => Info.PickupFromTimeCostModifier;

        public double PickupFromNoiseModifier => Info.PickupFromNoiseModifier;

        public double Hearing => Info.Hearing;

        public double Seeing => Info.Seeing;

        public bool Omnidirectional => Info.Omnidirectional;

        public double LightTransmission => Info.LightTransmission;

        public double NoiseTransmission => Info.NoiseTransmission;

        public Actor(
            Tile parent,
            ActorInfo info,
            ActorAI ai,
            IEnumerable<Treasure> treasures,
            int team,
            int debt = 0,
            bool lightOn = true)
        {
            Info = info;
            Treasures = treasures.ToList();
            VisibleSquares = new List<Point>();
            AI = ai;
            ActionDebt = debt;
            Team = team;
            Parent = parent;
            LightOn = lightOn;
        }

        private void AddTreasure(Treasure treasure)
        {
            var currentTreasure = Treasures.FirstOrDefault(x => treasure.Id == x.Id);
            if (currentTreasure != null)
            {
                currentTreasure.Amount += treasure.Amount;
            }
            else
            {
                Treasures.Add(treasure);
            }
        }

        private void IssueNoise(Tile tile, double power, string phrase)
        {
            Parent.Parent.IssueNoise(tile.X, tile.Y, power, this, phrase);
        }

        public void IssueNoise(double power, string phrase)
        {
            IssueNoise(Parent, power, phrase);
        }

        public void Interact(Tile tile)
        {
            if (InteractAction != null)
            {
                IssueNoise(tile, tile.NoiseMultiplier * InteractAction.NoiseModifier, null);
                ActionDebt += InteractAction.TimeCost;
                InteractAction.Action(tile, this, InteractAction);
            }
        }

        public void Move(Tile tile)
        {
            if (MoveAction != null && tile.Actor == null)
            {
                IssueNoise(tile, tile.NoiseMultiplier * MoveAction.NoiseModifier, null);
                ActionDebt += MoveAction.TimeCost;
                MoveAction.Action(tile, this, MoveAction);
            }
        }

        public void Sprint(Tile tile)
        {
            if (SprintAction != null && tile.Actor == null)
            {
                IssueNoise(tile, tile.NoiseMultiplier * SprintAction.NoiseModifier, null);
                ActionDebt += SprintAction.TimeCost;
                SprintAction.Action(tile, this, SprintAction);
            }
        }

        public void Pickup(Tile tile, Actor target, Treasure treasure)
        {
            var noiseModifier = target?.PickupFromNoiseModifier ?? 1;
            var timeModifier = target?.PickupFromTimeCostModifier ?? 1;
            IssueNoise(tile, noiseModifier * treasure.PickupNoise * PickupFromNoiseModifier, null);
            ActionDebt += (int)(timeModifier * treasure.PickupTimeCost * PickupFromTimeCostModifier);
            (target?.Treasures ?? tile.Treasures).Remove(treasure);
            AddTreasure(treasure);
        }

        public void Wait()
        {
            ActionDebt++;
        }
    }
}