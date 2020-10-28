using System.Collections.Generic;
using System.Linq;
using LatronArs.Engine.Scene.Objects;
using LatronArs.Engine.Scene.Objects.Structs;

namespace LatronArs.Engine.Scene
{
    public class Scene
    {
        public int Time { get; set; }

        public int Width { get; }

        public int Height { get; }

        public Actor Player { get; set; }

        public bool Leaving { get; set; }

        public ICollection<Actor> IntelligentActors { get; set; }

        public Tile[][] Tiles { get; set; }

        public Light[,] LightMap { get; set; }

        public List<Noise>[,] NoiseMap { get; set; }

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
            Tiles = new Tile[width][];
            for (int i = 0; i < width; i++)
            {
                Tiles[i] = new Tile[height];
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
        }

        private void AfterActionUpdate()
        {
            // Actions phase
            this.NoiseMap = new List<Noise>[Width, Height];
            var time = Player.ActionDebt;
            Time += time;
            Player.ActionDebt = 0;
            foreach (var actor in IntelligentActors)
            {
                actor.ActionDebt -= time;
                while (actor.ActionDebt <= 0)
                {
                    // TODOLP SomeActions
                    actor.Wait();
                }
            }

            // Updates
            UpdateLightMap();
            Player.UpdateVisionAndMemories();

            // ActorUpdates
            foreach (var actor in IntelligentActors)
            {
                actor.UpdateVisionAndMemories();

                // TODOLP params update and reaction
            }

            // VictoryChecks
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
            if (tile.Actor != null)
            {
                result.AddRange(tile.Actor.Treasures.Select(x => (target: tile.Actor, treasure: x)));
            }

            return result;
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
            AfterActionUpdate();
        }
    }
}