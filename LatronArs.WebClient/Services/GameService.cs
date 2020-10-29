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

        public GameState State { get; private set; }

        private void ChangeGameState(GameState state)
        {
            State = state;
        }

        public GameService()
        {
            _game = Game.StartNewGame(ChangeGameState);
            State = _game.State;
        }
    }
}