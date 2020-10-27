using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.Scene.Components;
using LatronArs.Engine.Scene.Objects.Structs;

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

        public LightInfo CeilingLight { get; set; }

        public bool LightOn { get; set; }

        public bool LightWorksAndOn => LightOn && CeilingLight != null && CeilingLight.Power > 0;

        // Inherit
        public string Sprite => Floor.Sprite;

        public Color Color => Floor.Color;

        public Action<Tile, Actor> InteractionReaction => Floor.InteractionReaction;

        public Action<Tile, Actor> StepReaction => Floor.StepReaction;

        public double NoiseMultiplier => Floor.NoiseMultiplier;

        public Tile(
            Scene parent,
            int x,
            int y,
            Floor floor,
            LightInfo ceilingLight,
            IEnumerable<Treasure> treasures,
            bool lightOn = true)
        {
            Parent = parent;
            X = x;
            Y = y;
            Floor = floor;
            CeilingLight = ceilingLight;
            Treasures = treasures.ToList();
            LightOn = lightOn;
        }
    }
}