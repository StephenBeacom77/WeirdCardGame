using System;
using System.Collections.Generic;
using System.Linq;
using WeirdCardGame.Data;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Provides a service for drawing cards in the weird card game.
    /// </summary>
    public sealed class CardDrawingService : ICardDrawingService
    {
        private readonly Random _shuffler = new Random();

        /// <summary>
        ///     Draw a deck of cards from the box.
        /// </summary>
        /// <returns>
        ///     The deck of cards with all 13 kinds of card in all 4 suits.
        /// </returns>
        public List<Card> DrawDeck()
        {
            var kinds = GetKinds();
            var suits = GetSuits();

            var deck = new List<Card>(kinds.Length * suits.Length);
            foreach (int kind in kinds)
            {
                foreach (int suit in suits)
                {
                    deck.Add(new Card(kind, suit));
                }
            }
            return deck;
        }

        /// <summary>
        ///     Draw a single card from the deck.
        /// </summary>
        /// <param name="deck">
        ///     The deck to draw from.
        /// </param>
        /// <returns>
        ///     The single card, removed from the deck.
        /// </returns>
        public Card DrawCard(List<Card> deck)
        {
            if (deck == null)
                throw new ArgumentNullException(nameof(deck));
            if (deck.Count - 1 < 0)
                throw new InvalidOperationException("Cannot draw card. No cards left.");

            var card = deck[_shuffler.Next(0, deck.Count)];
            deck.Remove(card);
            return card;
        }

        /// <summary>
        ///     Draw a hand of cards from the deck.
        /// </summary>
        /// <param name="deck">
        ///     The deck to draw from.
        /// </param>
        /// <param name="handSize">
        ///     The number of cards to draw.
        /// </param>
        /// <returns>
        ///     The hand of cards, removed from the deck.
        /// </returns>
        public Card[] DrawHand(List<Card> deck, int handSize)
        {
            if (deck == null)
                throw new ArgumentNullException(nameof(deck));
            if (deck.Count - handSize < 0)
                throw new InvalidOperationException($"Cannot draw hand. {deck.Count} card(s) left.");

            var hand = new Card[handSize];
            for (var cardIndex = 0; cardIndex < handSize; cardIndex++)
            {
                hand[cardIndex] = deck[_shuffler.Next(0, deck.Count)];
                deck.Remove(hand[cardIndex]);
            }
            return hand
                .OrderBy(c => c.Suit)
                .OrderByDescending(c => c.Kind)
                .ToArray();
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
