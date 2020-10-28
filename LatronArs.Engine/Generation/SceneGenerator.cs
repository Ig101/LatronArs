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
            for (var x = 0; x < 50; x++)
            {
                for (var y = 0; y < 50; y++)
                {
                    tiles.Add(x == 32 && y == 20 ?
                        new Tile(null, x, y, contentManager.GetFloorInfo("stone"), contentManager.GetCeilingInfo("chindalerie")) :
                        new Tile(null, x, y, contentManager.GetFloorInfo("floor")));
                }
            }

            // Decorations
            _ = new Actor(
                tiles.Find(x => x.X == 23 && x.Y == 20),
                contentManager.GetActorInfo("counter"),
                treasures: new[]
                {
                    new Treasure(contentManager.GetTreasureInfo("coin"), 120)
                });
            _ = new Actor(
                tiles.Find(x => x.X == 28 && x.Y == 20),
                contentManager.GetActorInfo("torch"));

            // Guardians
            var intelligentActors = new List<Actor>
            {
                new Actor(
                tiles.Find(x => x.X == 25 && x.Y == 20),
                contentManager.GetActorInfo("guardian"),
                new AI.ActorAI(50, 50),
                2),
                new Actor(
                tiles.Find(x => x.X == 15 && x.Y == 20),
                contentManager.GetActorInfo("guardian"),
                new AI.ActorAI(50, 50),
                2),
                new Actor(
                tiles.Find(x => x.X == 20 && x.Y == 25),
                contentManager.GetActorInfo("guardian"),
                new AI.ActorAI(50, 50),
                2),
                new Actor(
                tiles.Find(x => x.X == 20 && x.Y == 15),
                contentManager.GetActorInfo("guardian"),
                new AI.ActorAI(50, 50),
                2)
            };

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