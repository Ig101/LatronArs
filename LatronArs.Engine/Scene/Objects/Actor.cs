using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.AI;
using LatronArs.Engine.Helpers;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.Scene.Objects
{
    public class Actor : ITreasureBox, ILightBox
    {
        public Tile CurrentTile { get; set; }

        public ActorInfo Info { get; set; }

        public Direction Direction { get; set; }

        public ICollection<Treasure> Treasures { get; }

        public bool LightOn { get; set; }

        public int ActionDebt { get; set; }

        // Team 0 is neutral to all
        public int Team { get; set; }

        public bool LightWorksAndOn => LightOn && Light != null && Light.Power > 0;

        public ActorAI AI { get; set; }

        // Inherited
        public string Name => Info.Name;

        public ActionInfo MoveAction => Info.MoveAction;

        public ActionInfo SprintAction => Info.SprintAction;

        public Action<Tile, Actor> InteractAction => Info.InteractAction;

        public Func<Actor, Actor, int> InteractionReaction => Info.InteractionReaction;

        public LightInfo Light => Info.Light;

        public int PickupTimeCost => Info.PickupTimeCost;

        public double PickupFromTimeCostModifier => Info.PickupFromTimeCostModifier;

        public double PickupFromNoiseModifier => Info.PickupFromNoiseModifier;

        public double Hearing => Info.Hearing;

        public double Seeing => Info.Seeing;

        public bool Omnidirectional => Info.Omnidirectional;

        public double LightTransmission => Info.LightTransmission;

        public double NoiseTransmission => Info.NoiseTransmission;

        public Memory[][] Memories => AI?.Memories;

        public AIState AIState => AI?.State ?? AIState.Neutral;

        public bool DoAnalysis => AI?.DoAnalysis ?? false;

        public SpriteDefinition Sprite => new SpriteDefinition
        {
            Name = string.Intern(Info.Sprite),
            State = AIState,
            Direction = Direction,
            Color = Info.Color
        };

        public bool HasItems => CurrentTile.Parent.Player != this && Treasures.Any(x => x.Shines);

        public Actor(
            Tile parent,
            ActorInfo info,
            ActorAI ai = null,
            int team = 0,
            IEnumerable<Treasure> treasures = null,
            int debt = 0,
            bool lightOn = true)
        {
            Info = info;
            Treasures = treasures?.ToList() ?? new List<Treasure>();
            AI = ai;
            ActionDebt = debt;
            Team = team;
            CurrentTile = parent;
            LightOn = lightOn;
            if (ai != null)
            {
                ai.Parent = this;
            }

            parent.Actor = this;
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

        public void IssueNoise(Tile tile, double power, string phrase)
        {
            CurrentTile.Parent.IssueNoise(tile.X, tile.Y, power, this, phrase);
        }

        private void AddActorToMemory(int x, int y, Actor actor)
        {
            // TODO Add memory calculating
            Memories[x][y] = new Memory
            {
                Sprite = actor.Sprite,
                LightLevel = 0,
                Team = actor.Team,
                HasItems = actor.HasItems
            };
        }

        public void UpdateVisionAndMemories()
        {
            if (Memories != null)
            {
                // TODO UpdateVisionRight
                // TODO Add memory calculating
                for (var x = 0; x < Memories.Length; x++)
                {
                    for (var y = 0; y < Memories[x].Length; y++)
                    {
                        if (MathHelper.RangeBetween(x, y, CurrentTile.X, CurrentTile.Y) <= 7.6)
                        {
                            var tile = CurrentTile.Parent.Tiles[x][y];
                            Memories[x][y] = new Memory
                            {
                                Sprite = tile.Actor?.Sprite,
                                LightLevel = 1,
                                Team = tile.Actor?.Team ?? 0,
                                Visible = true,
                                HasItems = tile.Actor != null ? tile.Actor.HasItems : tile.HasItems
                            };
                        }
                        else if (Memories[x][y] != null)
                        {
                            Memories[x][y].Visible = false;
                        }
                    }
                }
            }
        }

        public void ChangeDirection(int x, int y)
        {
            Direction = x > CurrentTile.X ?
                Direction.Right : x < CurrentTile.X ?
                Direction.Left : y > CurrentTile.Y ?
                Direction.Bottom : y < CurrentTile.Y ?
                Direction.Top : Direction;
        }

        public void IssueNoise(double power, string phrase)
        {
            IssueNoise(CurrentTile, power, phrase);
        }

        public void Interact(Tile tile)
        {
            CurrentTile.Parent.Changed = true;
            if (InteractAction != null)
            {
                ChangeDirection(tile.X, tile.Y);
                InteractAction(tile, this);
            }
        }

        public void Move(Tile tile)
        {
            CurrentTile.Parent.Changed = true;
            if (MoveAction != null)
            {
                ChangeDirection(tile.X, tile.Y);
                if (tile.Actor == null)
                {
                    IssueNoise(tile, tile.NoiseMultiplier * MoveAction.NoiseModifier, null);
                    ActionDebt += MoveAction.TimeCost;
                    MoveAction.Action(tile, this, MoveAction);
                }
                else
                {
                    AddActorToMemory(tile.X, tile.Y, tile.Actor);
                }
            }
        }

        public void Sprint(Tile tile)
        {
            CurrentTile.Parent.Changed = true;
            if (SprintAction != null && tile.Actor == null)
            {
                ChangeDirection(tile.X, tile.Y);
                if (tile.Actor == null)
                {
                    IssueNoise(tile, tile.NoiseMultiplier * SprintAction.NoiseModifier, null);
                    ActionDebt += SprintAction.TimeCost;
                    SprintAction.Action(tile, this, SprintAction);
                }
                else
                {
                    AddActorToMemory(tile.X, tile.Y, tile.Actor);
                }
            }
        }

        public void Pickup(Tile tile, Actor target, Treasure treasure)
        {
            CurrentTile.Parent.Changed = true;
            ChangeDirection(tile.X, tile.Y);
            var noiseModifier = target?.PickupFromNoiseModifier ?? 1;
            var timeModifier = target?.PickupFromTimeCostModifier ?? 1;
            IssueNoise(tile, noiseModifier * treasure.PickupNoise, null);
            ActionDebt += (int)(timeModifier * treasure.PickupTimeCost * PickupTimeCost);
            (target?.Treasures ?? tile.Treasures).Remove(treasure);
            AddTreasure(treasure);
        }

        public void Wait()
        {
            ActionDebt += 5;
        }

        public (double noise, double time) SwitchLight()
        {
            if (Light == null)
            {
                return (0, 0);
            }

            LightOn = !LightOn;
            return (PickupFromNoiseModifier * Scene.LightNoise, Light.TurnOffCostModifier);
        }
    }
}