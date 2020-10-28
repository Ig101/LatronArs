using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content
{
    public interface IContentManager
    {
        ActorInfo GetActorInfo(string id);

        Floor GetFloorInfo(string id);

        TreasureInfo GetTreasureInfo(string id);

        Ceiling GetCeilingInfo(string id);
    }
}