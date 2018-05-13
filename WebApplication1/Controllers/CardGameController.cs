using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    /// <summary>
    ///     possible game results: single winner vs tied
    /// </summary>

    [Route("api/[controller]")]
    public class CardGameController : Controller
    {
        private const int HandSize = 5;
        private const int MinPlayerCount = 1;
        private const int MaxPlayerCount = 10;

        [HttpGet("[action]")]
        public Card[] GetRuleCards()
        {
            var cards = new Card[]
            {
                new Card(Card.Kinds.Ace, Card.Suits.None, GetPointsForKind((int)Card.Kinds.Ace)),
                new Card(Card.Kinds.King, Card.Suits.None, GetPointsForKind((int)Card.Kinds.King)),
                new Card(Card.Kinds.Queen, Card.Suits.None, GetPointsForKind((int)Card.Kinds.Queen)),
                new Card(Card.Kinds.Jack, Card.Suits.None, GetPointsForKind((int)Card.Kinds.Jack)),
                new Card(Card.Kinds.Ten, Card.Suits.None, GetPointsForKind((int)Card.Kinds.Ten)),
                new Card(Card.Kinds.None, Card.Suits.None, GetPointsForKind((int)Card.Kinds.None)),
            };
            return cards;
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
            return gameResult;
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
                case (int)Card.Kinds.Ace:
                    return PointsForAce;
                case (int)Card.Kinds.Ten:
                    return PointsForTen;
                case (int)Card.Kinds.King:
                    return PointsForKing;
                case (int)Card.Kinds.Queen:
                    return PointsForQueen;
                case (int)Card.Kinds.Jack:
                    return PointsForJack;
            }
            return PointsForOther;
        }

        public class GameResult
        {
            public Card Wildcard { get; set; }
            public PlayerResult[] PlayerResults { get; set; }
        }

        public class PlayerResult
        {
            public int Player { get; set; }
            public int Points { get; set; }
            public Card[] Cards { get; set; }
        }

        private class Deck
        {
            /// <summary>
            /// - faces could be dictionary of index, symbol, value
            /// - suits could be dictionary of index, symbol
            /// </summary>

            private static readonly int KindCount = Enum.GetValues(typeof(Card.Kinds)).Length - 1;
            private static readonly int SuitCount = Enum.GetValues(typeof(Card.Suits)).Length - 1;

            private List<Card> pile = new List<Card>(KindCount * SuitCount);
            private Random shuffler = new Random();

            public Deck()
            {
                var kinds = Enum.GetValues(typeof(Card.Kinds));
                var suits = Enum.GetValues(typeof(Card.Suits));
                foreach (int kind in kinds)
                {
                    if (kind == (int)Card.Kinds.None) continue;
                    foreach (int suit in suits)
                    {
                        if (suit == (int)Card.Suits.None) continue;
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

        public class Card
        {
            public Card(Kinds kind, Suits suit, int points = 0)
            {
                Kind = (int)kind;
                Suit = (int)suit;
                Points = points;
            }

            public Card(int kind, int suit, int points = 0)
            {
                Kind = kind;
                Suit = suit;
                Points = points;
            }

            public int Kind { get; private set; }
            public int Suit { get; private set; }
            public int Points { get; private set; }

            public enum Kinds
            {
                None = 0,
                Ace = 1,
                Two = 2,
                Three = 3,
                Four = 4,
                Five = 5,
                Six = 6,
                Seven = 7,
                Eight = 8,
                Nine = 9,
                Ten = 10,
                Jack = 11,
                Queen = 12,
                King = 13
            }

            public enum Suits
            {
                None = 0,
                Hearts = 1,
                Clubs = 2,
                Diamonds = 3,
                Spades = 4
            }
        }
    }
}
