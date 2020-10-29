using System.Collections.Generic;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Models.Enums;

namespace LatronArs.Engine.Transition
{
    public class TransitionInfo
    {
        public IEnumerable<Treasure> CollectedTreasures { get; init; }

        public IEnumerable<Spends> Spends { get; init; }

        public DefeatType? DefeatType { get; init; }

        public int StartingMoney { get; init; }
    }
}