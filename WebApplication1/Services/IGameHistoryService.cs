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
        void Save(GameResult gameResult);
    }
}