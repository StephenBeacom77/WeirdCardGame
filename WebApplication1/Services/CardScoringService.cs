using System;
using System.Collections.Generic;
using WeirdCardGame.Data;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Provides a service for scoring cards in the weird card game.
    /// </summary>
    public sealed class CardScoringService : ICardScoringService
    {
        /// <summary>
        ///     Get scored cards from the given cards and wild card.
        /// </summary>
        /// <param name="cards">
        ///     The cards to be scored.
        /// </param>
        /// <param name="wildcard">
        ///     The wild card.
        /// </param>
        /// <returns>
        ///     The scored cards.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the cards argument is null.
        /// </exception>
        public IEnumerable<Card> GetScoredCards(Card[] cards, Card wildcard = null)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));

            for (var index = 0; index < cards.Length; index++)
            {
                yield return GetScoredCard(cards[index], wildcard);
            }
        }

        private Card GetScoredCard(Card card, Card wildcard)
        {
            const int MatchingSuitMultiplier = 2;

            if (card == null) throw new ArgumentNullException(nameof(card));

            var points = card.Suit != wildcard?.Suit
                ? GetPointsForKind(card.Kind)
                : GetPointsForKind(card.Kind) * MatchingSuitMultiplier;
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
    }
}
