using System.Collections.Generic;

namespace LatronArs.Engine.Scene.Objects
{
    public interface ITreasureBox
    {
        ICollection<Treasure> Treasures { get; }
    }
}