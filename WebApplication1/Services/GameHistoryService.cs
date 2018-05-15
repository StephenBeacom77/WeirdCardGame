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
        /// <exception cref="ArgumentNullException">
        ///     Thrown if gameResult or its PlayerResults property is null.
        /// </exception>
        public Game Save(GameResult gameResult)
        {
            if (gameResult == null)
                throw new ArgumentNullException(nameof(gameResult));
            if (gameResult.PlayerResults == null)
                throw new ArgumentNullException($"{nameof(gameResult)}.PlayerResults");
            if (gameResult.PlayerResults[0] == null)
                throw new ArgumentNullException($"{nameof(gameResult)}.PlayerResults.0");

            var winner = gameResult.PlayerResults[0];
            var winners = gameResult.PlayerResults.Count(x => x.Points == winner.Points);
            var game = _gameContext.Games.Add(new Game { PlayerId = winners == 1 ? winner.Player : default(int?) });
            _gameContext.SaveChanges();
            game.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            return game.Entity;
        }
    }
}
