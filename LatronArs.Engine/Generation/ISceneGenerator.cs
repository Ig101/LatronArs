using LatronArs.Engine.Content;

namespace LatronArs.Engine.Generation
{
    public interface ISceneGenerator
    {
        Scene.Scene GenerateScene(int level, int seed, IContentManager contentManager);
    }
}