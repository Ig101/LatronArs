using LatronArs.Models.Enums;

namespace LatronArs.Engine.Scene.Objects.Structs
{
    public struct Noise
    {
        public double Power { get; set; }

        public NoiseAlertness Source { get; set; }

        public int SourceX { get; set; }

        public int SourceY { get; set; }

        public int ExpirationTimeLine { get; set; }
    }
}