using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Defines a service for playing a card game.
    /// </summary>
    public interface IGamePlayingService
    {
        /// <summary>
        ///     Play the game with the specified number of players.
        /// </summary>
        GameResult PlayGame(int playerCount);
    }
}