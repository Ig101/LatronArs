using System;
using LatronArs.Engine.Content;
using LatronArs.Engine.Generation;
using LatronArs.Models.Enums;

namespace LatronArs.Engine
{
    public class Game
    {
        public Guid Id { get; set; }

        public GameState State { get; set; }

        public Random Random { get; set; }

        public int Money { get; set; }

        public int Level { get; set; }

        public int Seed { get; set; }

        public ISceneGenerator SceneGenerator { get; set; }

        public IContentManager ContentManager { get; set; }

        public Scene.Scene CurrentScene { get; set; }

        private void SetupNewScene()
        {
            CurrentScene = SceneGenerator.GenerateScene(Level, Seed, ContentManager);
        }

        public static Game StartNewGame(int seed = -1)
        {
            var random = new Random();
            if (seed < 0)
            {
                seed = random.Next();
            }

            var game = new Game()
            {
                Id = Guid.NewGuid(),
                State = GameState.Playing,
                Random = random,
                Seed = seed,
                Level = 1,
                Money = 0,
                SceneGenerator = new SceneGenerator(),
                ContentManager = new ContentManager()
            };

            game.SetupNewScene();

            return game;
        }
    }
}