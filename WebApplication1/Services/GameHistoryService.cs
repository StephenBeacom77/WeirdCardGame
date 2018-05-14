using System;
using WeirdCardGame.Data;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Provides a service for card game results.
    /// </summary>
    public sealed class GameHistoryService : IGameHistoryService
    {
        private readonly GameContext _gameContext;

        /// <summary>
        ///     Makes a new game playing service.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if any argument is null.
        /// </exception>
        public GameHistoryService(GameContext gameContext)
        {
            _gameContext = gameContext
                ?? throw new ArgumentNullException(nameof(gameContext));
        }

        /// <summary>
        ///     Saves the game result.
        /// </summary>
        /// <param name="gameResult">
        ///     The game result to save.
        /// </param>
        public void Save(GameResult gameResult)
        {
            var winnerId = gameResult.PlayerResults[0].Player;
            _gameContext.Games.Add(new Game { PlayerId = winnerId });
            _gameContext.SaveChanges();
        }
    }
}
