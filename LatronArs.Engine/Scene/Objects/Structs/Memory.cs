using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Scene.Objects.Structs
{
    public class Memory
    {
        public SpriteDefinition Sprite { get; set; }

        public double LightLevel { get; set; }

        public int Team { get; set; }

        public bool Visible { get; set; }

        public bool HasItems { get; set; }
    }
}