using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.Scene.Components
{
    public class SpriteDefinition
    {
        public string Name { get; set; }

        public Direction Direction { get; set; }

        public AIState State { get; set; }

        public bool HasItems { get; set; }

        public Color Color { get; set; }
    }
}