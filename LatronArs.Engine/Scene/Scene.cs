using System;
using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;
using LatronArs.Engine.Transition;

namespace LatronArs.Engine.Scene
{
    public class Scene
    {
        public const int ActionPointsPerSecond = 10;
        public const int HoursForPlay = 4;
        public const double LightNoise = 0.1;
        public const int MoveCrushCost = 20;
        public const int SprintCrushCost = 500;

        public event Action<SceneResult> SceneFinished;

        public int Time { get; set; }

        public int Width { get; }

        public int Height { get; }

        public Actor Player { get; set; }

        public bool Leaving { get; set; }

        public ICollection<Actor> IntelligentActors { get; set; }

        public Tile[][] Tiles { get; set; }

        public Light[][] LightMap { get; set; }

        public List<Noise>[][] NoiseMap { get; set; }

        public bool Changed { get; set; }

        public Scene(
            int width,
            int height,
            int time,
            IEnumerable<Tile> tiles,
            IEnumerable<Actor> intelligentActors,
            Actor player)
        {
            Width = width;
            Height = height;
            Time = time;
            LightMap = new Light[width][];
            NoiseMap = new List<Noise>[width][];
            Tiles = new Tile[width][];
            for (int x = 0; x < width; x++)
            {
                Tiles[x] = new Tile[height];
                LightMap[x] = new Light[height];
                NoiseMap[x] = new List<Noise>[height];
                for (int y = 0; y < height; y++)
                {
                    NoiseMap[x][y] = new List<Noise>();
                }
            }

            foreach (var tile in tiles)
            {
                tile.Parent = this;
                Tiles[tile.X][tile.Y] = tile;
            }

            Player = player;
            IntelligentActors = intelligentActors.ToList();
            Leaving = false;

            UpdateLightMap();
            player.UpdateVisionAndMemories();
            foreach (var actor in intelligentActors)
            {
                actor.UpdateVisionAndMemories();
            }
        }

        private void UpdateLightMap()
        {
            // TODO LightMechanic
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    LightMap[x][y] = new Light
                    {
                        Power = 0.9,
                        Color = new Color { R = 255, G = 255, B = 155 }
                    };
                }
            }
        }

        private void AfterActionUpdate(bool meaningful = true)
        {
            // Actions phase
            var time = Player.ActionDebt;
            Time += time;
            Player.ActionDebt = 0;
            foreach (var actor in IntelligentActors)
            {
                actor.ActionDebt -= time;
                while (actor.ActionDebt <= 0)
                {
                    // TODOLP SomeActions, set meaningful for more if any action done
                    actor.Wait();
                }
            }

            if (meaningful)
            {
            // Updates
                UpdateLightMap();
                Player.UpdateVisionAndMemories();

                // ActorUpdates
                foreach (var actor in IntelligentActors)
                {
                    actor.UpdateVisionAndMemories();

                    // TODOLP params update and reaction
                }
            }

            // VictoryChecks
            var timeout = Time / ActionPointsPerSecond > HoursForPlay * 60 * 60;
            if (Player.AIState == Models.Enums.AIState.Unconcious)
            {
                SceneFinished(new SceneResult
                {
                    CollectedTreasures = Array.Empty<Treasure>(),
                    Time = Time,
                    Arrested = true,
                    Timeout = timeout
                });
            }

            if (timeout)
            {
                SceneFinished(new SceneResult
                {
                    CollectedTreasures = Array.Empty<Treasure>(),
                    Time = Time,
                    Arrested = false,
                    Timeout = true
                });
            }

            if (Leaving)
            {
                SceneFinished(new SceneResult
                {
                    CollectedTreasures = Player.Treasures,
                    Time = Time,
                    Arrested = false,
                    Timeout = false
                });
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    NoiseMap[x][y].Clear();
                }
            }
        }

        public bool GetSimpleProcessingState(int x, int y)
        {
            // TODO Distance between player and position
            return true;
        }

        public IEnumerable<(Actor target, Treasure treasure)> GetPickupItems(int x, int y)
        {
            var tile = Tiles[x][y];
            var result = tile.Treasures.Select(x => (target: (Actor)null, treasure: x)).ToList();
            if (tile.Actor != null && tile.Actor != Player)
            {
                result.AddRange(tile.Actor.Treasures.Select(x => (target: tile.Actor, treasure: x)));
            }

            return result;
        }

        public string GetCurrentTime()
        {
            var secondsPassed = Time / ActionPointsPerSecond;
            var hour = secondsPassed / 3600;
            var hourString = hour < 10 ? "0" + hour.ToString() : hour.ToString();
            var minute = (secondsPassed / 60) - (hour * 60);
            var minuteString = minute < 10 ? "0" + minute.ToString() : minute.ToString();
            return $"{hourString}:{minuteString}";
        }

        public void IssueNoise(int x, int y, double power, Actor source, string phrase)
        {
            // TODO NoiseMechanic
        }

        // Actions
        public void Move(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                Player.Move(Tiles[x][y]);
                AfterActionUpdate();
            }
        }

        public void Sprint(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                Player.Sprint(Tiles[x][y]);
                AfterActionUpdate();
            }
        }

        public void Interact(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                Player.Interact(Tiles[x][y]);
                AfterActionUpdate();
            }
        }

        public void Pickup(int x, int y, Actor target, Treasure treasure)
        {
            Player.Pickup(Tiles[x][y], target, treasure);
            AfterActionUpdate();
        }

        public void Wait()
        {
            Player.Wait();
            AfterActionUpdate(false);
        }
    }
}