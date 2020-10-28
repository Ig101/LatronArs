using LatronArs.Models.Enums;

namespace LatronArs.Engine.Scene.Components
{
    public class SpriteDefinition
    {
        public string Name { get; set; }

        public Direction Direction { get; set; }

        public AIState State { get; set; }

        public bool HasItems { get; set; }
    }
}