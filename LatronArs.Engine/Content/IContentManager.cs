using LatronArs.Engine.Scene.Components;

namespace LatronArs.Engine.Content
{
    public interface IContentManager
    {
        ActorInfo GetActorInfo(string id);

        Floor GetFloor(string id);

        TreasureInfo GetTreasureInfo(string id);
    }
}