using WeirdCardGame.Data;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Defines a service for card game history.
    /// </summary>
    public interface IGameHistoryService
    {
        /// <summary>
        ///     Saves a game result.
        /// </summary>
        Game Save(GameResult gameResult);
    }
}