using System.Collections.Generic;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Defines a service for scoring cards in a card game.
    /// </summary>
    public interface ICardScoringService
    {
        /// <summary>
        ///     Gets scored cards from the given cards and wild card.
        /// </summary>
        IEnumerable<ScoredCard> GetScoredCards(Card[] cards, Card wildcard);
    }
}