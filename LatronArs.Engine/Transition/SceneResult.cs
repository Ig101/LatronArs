using System.Collections;
using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Transition
{
    public class SceneResult
    {
        public IEnumerable<Treasure> CollectedTreasures { get; init; }

        public int Time { get; set; }

        public bool Arrested { get; set; }

        public bool Timeout { get; set; }
    }
}