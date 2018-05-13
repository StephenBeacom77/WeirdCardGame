using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeirdCardGame.Data;
using WeirdCardGame.Models.Cards;

namespace WeirdCardGame.Controllers
{
    /// <summary>
    ///     Controller for game of cards with weird rules.
    /// </summary>
    /// <remarks>
    ///     todo: fill in the readme file
    ///         - using VS2017, EF Core (memory mode), nUnit, Moq
    ///         - brief notes on
    ///             - clear winner versus tie - only clear winner saved to db
    ///             - this means that any tie should be shown as such to be consistent
    ///     todo: remove unused code
    ///     todo: component for drawing card in ts/angular
    ///     todo: client side service for talking to server
    ///     todo: package the solution by cloning to a different path!
    ///     
    ///     todo: Once all cards are dealt, the winner is shown on the screen
    ///         - bonus points for animation
    ///         - maybe just show the winner info after the players are listed
    ///         - maybe just leave this bit alone - probably the best option
    ///     todo: history of all previous winners should be stored in a database.
    ///         - previous winners in database
    ///             - game service should store the winning player per round in winners table
    ///                 - columns are: round as int, player number as nullable int, pk = round
    ///                 - if no clear winner for a given round then store null as winner
    ///             - use EF and in memory db option for storage - see FC code!
    ///     todo: add some unit tests for server code of significance
    ///         - indicate in readme that not all code is tested due to lack of time
    ///             - indicate what else should be tested?
    ///         - CardGameController
    ///             - GetRuleCards
    ///                 - single test to check for all rule cards
    ///             - PlayGame
    ///                 - multiple tests to check for different player counts
    ///                     - allowed: 1, 2, 9, 10
    ///                     - invalid: 0, 11
    ///                 - probably need a CardScoringService to score cards on kind and suit
    ///                     - it has GetScoredCard(card, wildcard)
    ///                         - it has logic from GetPointsForKind and GetScoredCard
    ///                     - it can be tested and mocked for other tests
    ///             - Deck
    ///                 - test that it has 52 cards
    ///                     - by drawing them one by one
    ///                         - check off every combo of kind and suit with zero points
    ///                         - the 53rd draw should fail
    /// </remarks>
    [Route("api/[controller]")]
    public sealed class CardGameController : Controller
    {
        private const int HandSize = 5;
        private const int MinPlayerCount = 1;
        private const int MaxPlayerCount = 10;

        private readonly GameContext _gameContext;

        public CardGameController(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        [HttpGet("[action]")]
        public Card[] GetRuleCards()
        {
            var cards = new Card[]
            {
                new Card(Kinds.Ace, Suits.None, GetPointsForKind((int)Kinds.Ace)),
                new Card(Kinds.King, Suits.None, GetPointsForKind((int)Kinds.King)),
                new Card(Kinds.Queen, Suits.None, GetPointsForKind((int)Kinds.Queen)),
                new Card(Kinds.Jack, Suits.None, GetPointsForKind((int)Kinds.Jack)),
                new Card(Kinds.Ten, Suits.None, GetPointsForKind((int)Kinds.Ten)),
                new Card(Kinds.None, Suits.None, GetPointsForKind((int)Kinds.None)),
            };
            return cards;
        }

        [HttpGet("[action]")]
        public Kind[] GetCardKinds()
        {
            return _gameContext.Kinds.AsNoTracking().ToArray();
        }

        [HttpGet("[action]")]
        public Suit[] GetCardSuits()
        {
            return _gameContext.Suits.AsNoTracking().ToArray();
        }

        [HttpGet("[action]")]
        public GameResult PlayGame(int playerCount)
        {
            if (playerCount < MinPlayerCount || playerCount > MaxPlayerCount)
                throw new ArgumentOutOfRangeException(nameof(playerCount), playerCount, $"Must be between {MinPlayerCount} and {MaxPlayerCount}");

            var deck = new Deck();

            var wildcard = deck.DrawCard();
            var playerResults = new PlayerResult[playerCount];

            for (var playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                var hand = GetScoredCards(deck.DrawHand(HandSize), wildcard);
                playerResults[playerIndex] = new PlayerResult
                {
                    Player = playerIndex + 1,
                    Cards = hand,
                    Points = hand.Sum(c => c.Points),
                };
            }

            var gameResult = new GameResult()
            {
                Wildcard = wildcard,
                PlayerResults = playerResults.OrderByDescending(pr => pr.Points).ToArray()
            };

            SaveGameResult(gameResult);
            return gameResult;
        }

        private void SaveGameResult(GameResult gameResult)
        {
            var winnerId = gameResult.PlayerResults[0].Player;
            _gameContext.Games.Add(new Game { PlayerId = winnerId });
            _gameContext.SaveChanges();
        }

        private Card[] GetScoredCards(Card[] hand, Card wildcard)
        {
            if (hand == null) throw new ArgumentNullException(nameof(hand));
            if (wildcard == null) throw new ArgumentNullException(nameof(wildcard));

            var scoredCards = new Card[hand.Length];
            for (var index = 0; index < hand.Length; index++)
            {
                scoredCards[index] = GetScoredCard(hand[index], wildcard);
            }
            return scoredCards;
        }

        private Card GetScoredCard(Card card, Card wildcard)
        {
            if (card == null) throw new ArgumentNullException(nameof(card));
            if (wildcard == null) throw new ArgumentNullException(nameof(wildcard));

            var points = card.Suit != wildcard?.Suit
                ? GetPointsForKind(card.Kind)
                : GetPointsForKind(card.Kind) * 2;
            return new Card(card.Kind, card.Suit, points);
        }

        private int GetPointsForKind(int kind)
        {
            const int PointsForAce = 11;
            const int PointsForTen = 10;
            const int PointsForKing = 4;
            const int PointsForQueen = 3;
            const int PointsForJack = 2;
            const int PointsForOther = 0;

            switch (kind)
            {
                case (int)Kinds.Ace:
                    return PointsForAce;
                case (int)Kinds.Ten:
                    return PointsForTen;
                case (int)Kinds.King:
                    return PointsForKing;
                case (int)Kinds.Queen:
                    return PointsForQueen;
                case (int)Kinds.Jack:
                    return PointsForJack;
            }
            return PointsForOther;
        }

        private sealed class Deck
        {
            private static readonly int KindCount = Enum.GetValues(typeof(Kinds)).Length - 1; // exclude kind of none
            private static readonly int SuitCount = Enum.GetValues(typeof(Suits)).Length - 1; // exclude suit of none

            private List<Card> pile = new List<Card>(KindCount * SuitCount);
            private Random shuffler = new Random();

            public Deck()
            {
                var kinds = Enum.GetValues(typeof(Kinds));
                var suits = Enum.GetValues(typeof(Suits));
                foreach (int kind in kinds)
                {
                    if (kind == (int)Kinds.None) continue;
                    foreach (int suit in suits)
                    {
                        if (suit == (int)Suits.None) continue;
                        pile.Add(new Card(kind, suit));
                    }
                }
            }

            public Card DrawCard()
            {
                if (pile.Count == 0)
                    throw new InvalidOperationException("Cannot draw card. No cards left.");

                var card = pile[shuffler.Next(0, pile.Count)];
                pile.Remove(card);
                return card;
            }

            public Card[] DrawHand(int cardCount)
            {
                if (pile.Count - cardCount <= 0)
                    throw new InvalidOperationException($"Cannot draw hand. {pile.Count} cards left.");

                var hand = new Card[cardCount];
                for (var cardIndex = 0; cardIndex < cardCount; cardIndex++)
                {
                    hand[cardIndex] = pile[shuffler.Next(0, pile.Count)];
                    pile.Remove(hand[cardIndex]);
                }
                return hand
                    .OrderByDescending(c => c.Kind)
                    .OrderBy(c => c.Suit)
                    .ToArray();
            }
        }
    }
}
