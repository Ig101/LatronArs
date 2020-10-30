using LatronArs.Models.Enums;

namespace LatronArs.WebClient.Models
{
    public class SpriteVariation
    {
        public AIState? State { get; set; }

        public Direction? Direction { get; set; }

        public string Path { get; set; }

        public bool Mask { get; set; }

        public bool IsSquare { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public bool Mirrored { get; set; }
    }
}