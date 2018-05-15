using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WeirdCardGame.Data;
using WeirdCardGame.Models;
using WeirdCardGame.Services;

namespace WeirdCardGame.UnitTests.Services
{
    [TestFixture]
    public class CardDrawingServiceTests
    {
        [Test]
        public void ShouldThrowOnDrawCardIfDeckIsNull()
        {
            var cds = new CardDrawingService();

            var ex = Assert.Throws<ArgumentNullException>(() => cds.DrawCard(null));
            Assert.That(ex.ParamName, Is.EqualTo("deck"));
        }

        [Test]
        public void ShouldDrawDeckWithAllKindsAndSuits()
        {
            var cds = new CardDrawingService();
            var deck = cds.DrawDeck();

            Assert.That(deck.Count, Is.EqualTo(52));

            var kinds = (Enum.GetValues(typeof(Kinds)) as Kinds[])
                .Where(kind => kind != Kinds.Any).ToArray();
            var suits = (Enum.GetValues(typeof(Suits)) as Suits[])
                .Where(suit => suit != Suits.Any).ToArray();

            foreach (var kind in kinds)
            {
                foreach (var suit in suits)
                {
                    var card = deck.Single(c => c.Kind == (int)kind && c.Suit == (int)suit);
                }
            }
            Assert.Pass("Cards of every kind and suit found in deck.");
        }

        [Test]
        public void ShouldDrawCardFromDeckIfCardsLeft()
        {
            var cds = new CardDrawingService();
            var deck = cds.DrawDeck();

            const int cardsInFullDeck = 52;
            for (var index = 0; index < cardsInFullDeck; index++)
            {
                var card = cds.DrawCard(deck);
                var cardsLeft = cardsInFullDeck - (index + 1);
                Assert.That(deck.Count, Is.EqualTo(cardsLeft));
            }
        }

        [Test]
        public void ShouldThrowOnDrawCardFromDeckIfNoCardsLeft()
        {
            var cds = new CardDrawingService();
            var deck = new List<Card>(0);

            var ex = Assert.Throws<InvalidOperationException>(() => cds.DrawCard(deck));
            Assert.That(ex.Message, Contains.Substring("Cannot draw card."));
        }

        [Test]
        public void ShouldDrawHandFromDeckIfCardsLeft()
        {
            var cds = new CardDrawingService();
            var deck = cds.DrawDeck();

            const int cardsInHand = 2;
            const int cardsInFullDeck = 52;
            for (var index = 0; index < cardsInFullDeck / cardsInHand; index++)
            {
                var card = cds.DrawHand(deck, cardsInHand);
                var cardsLeft = cardsInFullDeck - ((index + 1) * cardsInHand);
                Assert.That(deck.Count, Is.EqualTo(cardsLeft));
            }
        }

        [Test]
        public void ShouldThrowOnDrawHandFromDeckIfNotEnoughCardsLeft()
        {
            var cds = new CardDrawingService();
            var deck = new List<Card>(1) { new Card(Kinds.Ace, Suits.Diamonds) };

            var ex = Assert.Throws<InvalidOperationException>(() => cds.DrawHand(deck, 2));
            Assert.That(ex.Message, Contains.Substring("Cannot draw hand."));
        }
    }
}
