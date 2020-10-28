using System;
using LatronArs.Engine.Content;
using LatronArs.Engine.Generation;
using LatronArs.Models.Enums;

namespace LatronArs.Engine
{
    public class Game
    {
        public string Id { get; set; }

        public GameState State { get; set; }

        public Random Random { get; set; }

        public int Money { get; set; }

        public int Level { get; set; }

        public int Seed { get; set; }

        public ISceneGenerator SceneGenerator { get; set; }

        public IContentManager ContentManager { get; set; }

        public Scene.Scene CurrentScene { get; set; }
    }
}