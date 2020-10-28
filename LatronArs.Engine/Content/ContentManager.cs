using System.Collections.Generic;
using LatronArs.Engine.Content.Actors;
using LatronArs.Engine.Content.Ceilings;
using LatronArs.Engine.Content.Floors;
using LatronArs.Engine.Content.Treasures;
using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content
{
    public class ContentManager : IContentManager
    {
        private IDictionary<string, ActorInfo> _actors;
        private IDictionary<string, Floor> _floors;
        private IDictionary<string, Ceiling> _ceilings;
        private IDictionary<string, TreasureInfo> _treasures;

        public ContentManager()
        {
            _actors = ActorsManager.GetActors();
            _floors = FloorsManager.GetFloors();
            _ceilings = CeilingsManager.GetCeilings();
            _treasures = TreasuresManager.GetTreasures();
        }

        public ActorInfo GetActorInfo(string id)
        {
            _actors.TryGetValue(id, out var val);
            return val;
        }

        public Ceiling GetCeilingInfo(string id)
        {
            _ceilings.TryGetValue(id, out var val);
            return val;
        }

        public Floor GetFloorInfo(string id)
        {
            _floors.TryGetValue(id, out var val);
            return val;
        }

        public TreasureInfo GetTreasureInfo(string id)
        {
            _treasures.TryGetValue(id, out var val);
            return val;
        }
    }
}