using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.Content;
using LatronArs.Engine.Generation;
using LatronArs.Engine.Transition;
using LatronArs.Models.Enums;
using LatronArs.Models.Saves;

namespace LatronArs.Engine
{
    public class Game
    {
        public event Action<GameState> ChangeGameState;

        public Guid Id { get; set; }

        public GameState State { get; set; }

        public Random Random { get; set; }

        public int Money { get; set; }

        public int Level { get; set; }

        public int Seed { get; set; }

        public ISceneGenerator SceneGenerator { get; set; }

        public IContentManager ContentManager { get; set; }

        public Scene.Scene CurrentScene { get; set; }

        public TransitionInfo TransitionInfo { get; set; }

        private static IEnumerable<Spends> GetSpends(int money)
        {
            var spends = new List<Spends>
            {
                new Spends
                {
                    Name = "Provision",
                    Amount = 5
                },
                new Spends
                {
                    Name = "Rent",
                    Amount = 5
                },
                new Spends
                {
                    Name = "Treatment",
                    Amount = 200
                }
            };
            var miscSpend = (int)((money - spends.Sum(x => x.Amount)) * 0.8);
            if (miscSpend > 0)
            {
                spends.Add(new Spends
                {
                    Name = "Other",
                    Amount = miscSpend
                });
            }

            return spends;
        }

        private void FinishSceneProcessing(SceneResult result)
        {
            var startingMoney = Money;
            Money += result.CollectedTreasures.Sum(x => x.Value * x.Amount);
            var spends = GetSpends(Money);
            Money -= spends.Sum(x => x.Amount);
            DefeatType? defeatType = null;
            if (result.Arrested)
            {
                defeatType = DefeatType.Arrest;
            }
            else if (result.Timeout)
            {
                defeatType = DefeatType.Timeout;
            }
            else if (Money < 0)
            {
                defeatType = DefeatType.Shortage;
            }

            State = defeatType != null ? GameState.Defeat : GameState.Transition;
            TransitionInfo = new TransitionInfo
            {
                CollectedTreasures = result.CollectedTreasures,
                Spends = spends,
                StartingMoney = startingMoney,
                DefeatType = defeatType
            };

            ChangeGameState(State);
            ClearScene();
        }

        private void ClearScene()
        {
            if (CurrentScene != null)
            {
                CurrentScene.SceneFinished -= FinishSceneProcessing;
            }
        }

        private void SetupNewScene(SceneSave save = null)
        {
            ClearScene();

            CurrentScene = SceneGenerator.GenerateScene(Level, Seed, ContentManager);
            CurrentScene.SceneFinished += FinishSceneProcessing;
        }

        public static Game StartNewGame(Action<GameState> callback, int seed = -1)
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

            game.ChangeGameState += callback;
            game.SetupNewScene();

            return game;
        }
    }
}