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
        public const int PointsForAce = 11;
        public const int PointsForTen = 10;
        public const int PointsForKing = 4;
        public const int PointsForQueen = 3;
        public const int PointsForJack = 2;
        public const int PointsForOther = 0;

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
        public IEnumerable<ScoredCard> GetScoredCards(Card[] cards, Card wildcard = null)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));

            for (var index = 0; index < cards.Length; index++)
            {
                yield return GetScoredCard(cards[index], wildcard);
            }
        }

        private ScoredCard GetScoredCard(Card card, Card wildcard)
        {
            const int MatchingSuitMultiplier = 2;

            if (card == null) throw new ArgumentNullException(nameof(card));

            var points = card.Suit != wildcard?.Suit
                ? GetPointsForKind(card.Kind)
                : GetPointsForKind(card.Kind) * MatchingSuitMultiplier;
            return new ScoredCard(card.Kind, card.Suit, points);
        }

        private int GetPointsForKind(int kind)
        {
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
