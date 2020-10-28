using LatronArs.Engine;
using LatronArs.Engine.Scene;
using LatronArs.Models.Enums;
using LatronArs.WebClient.Services.Interfaces;

namespace LatronArs.WebClient.Services
{
    public class GameService : IGameService
    {
        private Game _game;

        public Scene CurrentScene => _game?.CurrentScene;

        public GameState State => _game?.State ?? GameState.Unknown;

        public GameService()
        {
            _game = Game.StartNewGame();
        }
    }
}