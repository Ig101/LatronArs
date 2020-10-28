using LatronArs.Engine.Scene;
using LatronArs.Models.Enums;

namespace LatronArs.WebClient.Services.Interfaces
{
    public interface IGameService
    {
        Scene CurrentScene { get; }

        GameState State { get; }
    }
}