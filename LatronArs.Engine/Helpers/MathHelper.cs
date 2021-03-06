using System;

namespace LatronArs.Engine.Helpers
{
    public static class MathHelper
    {
        public static double RangeBetween(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
        }
    }
}