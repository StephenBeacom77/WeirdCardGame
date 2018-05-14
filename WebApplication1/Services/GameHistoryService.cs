using System;
using System.Linq;
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
            var winner = gameResult.PlayerResults[0];
            var winners = gameResult.PlayerResults.Count(x => x.Points == winner.Points);
            _gameContext.Games.Add(new Game { PlayerId = winners == 1 ? winner.Player : default(int?) });
            _gameContext.SaveChanges();
        }
    }
}
