using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.Scene.Objects
{
    public class Tile : ITreasureBox
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Scene Parent { get; set; }

        public Actor Actor { get; set; }

        public ICollection<Treasure> Treasures { get; }

        public Floor Floor { get; set; }

        public Ceiling Ceiling { get; set; }

        public bool LightOn { get; set; }

        public bool LightWorksAndOn => LightOn && Light != null && Light.Power > 0;

        // Inherit
        public string Sprite => string.Intern(Floor.Sprite);

        public LightInfo Light => Ceiling?.Light;

        public Func<Tile, Actor, int> InteractionReaction => Ceiling?.InteractionReaction;

        public Action<Tile, Actor> StepReaction => Floor.StepReaction;

        public double NoiseMultiplier => Floor.NoiseMultiplier;

        public Tile(
            Scene parent,
            int x,
            int y,
            Floor floor,
            Ceiling ceiling = null,
            bool lightOn = true,
            IEnumerable<Treasure> treasures = null)
        {
            Parent = parent;
            X = x;
            Y = y;
            Floor = floor;
            Ceiling = ceiling;
            Treasures = treasures?.ToList() ?? new List<Treasure>();
            LightOn = lightOn;
        }
    }
}