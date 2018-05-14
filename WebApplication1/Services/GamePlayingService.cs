using System;
using System.Collections.Generic;
using System.Linq;
using WeirdCardGame.Data;
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
        private readonly List<Card> _deck;

        private int _playerCount;

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

            var kinds = GetKinds();
            var suits = GetSuits();

            var deckSize = kinds.Length * suits.Length;

            _deck = new List<Card>(deckSize);
            foreach (int kind in kinds)
            {
                foreach (int suit in suits)
                {
                    _deck.Add(new Card(kind, suit));
                }
            }
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
            var maxPlayerCount = _deck.Count / PlayerHandSize;
            if (playerCount < MinPlayerCount || playerCount > maxPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount,
                    $"Player count must be between {MinPlayerCount} and {maxPlayerCount}.");
            _playerCount = playerCount;

            var wildcard = _drawingService.DrawCard(_deck);

            var results = new PlayerResult[_playerCount];
            for (var playerIndex = 0; playerIndex < _playerCount; playerIndex++)
            {
                var cards = _drawingService.DrawHand(_deck, PlayerHandSize);
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
                PlayerResults = results.OrderByDescending(r => r.Points).ToArray()
            };
            return gameResult;
        }

        private static Kinds[] GetKinds()
        {
            return (Enum.GetValues(typeof(Kinds)) as Kinds[])
                .Where(kind => kind != Kinds.Any).ToArray();
        }

        private static Suits[] GetSuits()
        {
            return (Enum.GetValues(typeof(Suits)) as Suits[])
                .Where(suit => suit != Suits.Any).ToArray();
        }
    }
}
