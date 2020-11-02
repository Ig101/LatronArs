using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.WebClient.Pages.Scene
{
    public class PickupModalData
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string ActorName { get; set; }

        public IEnumerable<(Actor target, Treasure treasure)> ActorTreasures { get; set; }

        public IEnumerable<(Actor target, Treasure treasure)> FloorTreasures { get; set; }

        public int CurrentSelection { get; set; }
    }
}