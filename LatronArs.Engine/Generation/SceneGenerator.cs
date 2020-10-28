using System.Collections.Generic;
using LatronArs.Engine.Content;
using LatronArs.Engine.Scene.Objects;

namespace LatronArs.Engine.Generation
{
    public class SceneGenerator : ISceneGenerator
    {
        public Scene.Scene GenerateScene(int level, int seed, IContentManager contentManager)
        {
            // Tiles
            var tiles = new List<Tile>();

            // Decorations

            // Guardians
            var intelligentActors = new List<Actor>();

            // Player
            var player = new Actor(
                tiles.Find(x => x.X == 20 && x.Y == 20),
                contentManager.GetActorInfo("thief"),
                new AI.ActorAI(50, 50),
                1);

            return new Scene.Scene(
                50,
                50,
                0,
                tiles,
                intelligentActors,
                player);
        }
    }
}