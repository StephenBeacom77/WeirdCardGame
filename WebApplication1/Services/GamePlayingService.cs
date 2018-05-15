using System;
using System.Linq;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Provides a service for playing the weird card game.
    /// </summary>
    public sealed class GamePlayingService : IGamePlayingService
    {
        private const int MinPlayerCount = 1;
        private const int PlayerHandSize = 5;

        private readonly ICardDrawingService _drawingService;
        private readonly ICardScoringService _scoringService;

        /// <summary>
        ///     Makes a new game playing service.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if any argument is null.
        /// </exception>
        public GamePlayingService(
            ICardDrawingService drawingService,
            ICardScoringService scoringService)
        {
            _drawingService = drawingService
                ?? throw new ArgumentNullException(nameof(drawingService));
            _scoringService = scoringService
                ?? throw new ArgumentNullException(nameof(scoringService));
        }

        /// <summary>
        ///     Play the game with the specified number of players.
        /// </summary>
        /// <param name="playerCount">
        ///     The number of players.
        /// </param>
        /// <returns>
        ///     The result of playing the game.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the number of players is out of range.
        /// </exception>
        public GameResult PlayGame(int playerCount)
        {
            var deck = _drawingService.DrawDeck();

            var maxPlayerCount = (deck.Count - 1) / PlayerHandSize;
            if (playerCount < MinPlayerCount || playerCount > maxPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount,
                    $"Player count must be between {MinPlayerCount} and {maxPlayerCount}.");

            var wildcard = _drawingService.DrawCard(deck);

            var results = new PlayerResult[playerCount];
            for (var playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                var cards = _drawingService.DrawHand(deck, PlayerHandSize);
                var scoredCards = _scoringService.GetScoredCards(cards, wildcard).ToArray();
                results[playerIndex] = new PlayerResult
                {
                    Player = playerIndex + 1,
                    Cards = scoredCards,
                    Points = scoredCards.Sum(c => c.Points),
                };
            }

            var gameResult = new GameResult()
            {
                Wildcard = wildcard,
                PlayerResults = results
                    .OrderByDescending(r => r.Points)
                    .OrderBy(r => r.Player)
                    .ToArray()
            };
            return gameResult;
        }
    }
}
