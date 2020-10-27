using System;
using System.Collections.Generic;
using System.Drawing;
using LatronArs.Engine.Scene.Components;

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

        // Inherit
        public string Sprite => Floor.Sprite;

        public Color Color => Floor.Color;

        public Action<Scene, Tile, Actor> InteractionReaction => Floor.InteractionReaction;

        public Action<Scene, Tile, Actor> StepReaction => Floor.StepReaction;
    }
}