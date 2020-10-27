using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;

namespace LatronArs.Engine.Scene
{
    public class Scene
    {
        public int Time { get; set; }

        public Actor Player { get; set; }

        public Memory[][] Memories { get; set; }

        public Tile[][] Tiles { get; set; }

        public Light[][] LightMap { get; set; }

        public List<Noise>[][] NoiseMap { get; set; }
    }
}