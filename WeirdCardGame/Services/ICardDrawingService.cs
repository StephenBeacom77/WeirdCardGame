using System.Collections.Generic;
using WeirdCardGame.Models;

namespace WeirdCardGame.Services
{
    /// <summary>
    ///     Defines a service for drawing cards in a card game.
    /// </summary>
    public interface ICardDrawingService
    {
        /// <summary>
        ///     Draw a deck of cards from the box.
        /// </summary>
        List<Card> DrawDeck();

        /// <summary>
        ///     Draw a single card from the deck.
        /// </summary>
        Card DrawCard(List<Card> deck);

        /// <summary>
        ///     Draw a hand of cards from the deck.
        /// </summary>
        Card[] DrawHand(List<Card> deck, int handSize);
    }
}