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
    public class GamePlayingServiceTests
    {
        /// <summary>
        ///     Fake card drawing service with simple rules.
        /// </summary>
        /// <remarks>
        ///     Drawing cards:
        ///     - Reduced deck size
        ///     - Dedicated wildcard has kind/suit of Any/Any.
        ///     - Ace, King, Queen, Jack and Ten are the card kinds for hands.
        ///     - Hearts, Diamonds, Clubs and Spades are the card suits for hands.
        ///     - The above is guaranteed by always drawing the first card.
        ///     Scoring cards:
        ///     - Ace scores 1, Others score 0.
        ///     - Red suit doubles card score, Black suit keeps card score.
        ///     - Ace of Hearts/Diamonds gets 2, Ace of Clubs/Spades gets 1.
        /// </remarks>
        private class FakeDrawingService : ICardDrawingService
        {
            private Kinds[] _kinds = new Kinds[]
            {
                Kinds.Ace, Kinds.King, Kinds.Queen, Kinds.Jack, Kinds.Ten
            };

            private Suits[] _suits = new Suits[]
            {
                Suits.Hearts, Suits.Diamonds, Suits.Clubs, Suits.Spades
            };

            public List<Card> DrawDeck()
            {
                var deck = new List<Card>
                {
                   new Card(Kinds.Any, Suits.Any) // one wildcard
                };
                foreach (var suit in _suits)
                {
                    foreach (var kind in _kinds)
                    {
                        deck.Add(new Card(kind, suit)); // 5 kinds * 4 suits = 20 cards
                    }
                }
                return deck;
            }

            public Card DrawCard(List<Card> deck)
            {
                if (deck == null || deck.Count < 1)
                    throw new InvalidOperationException();

                var card = deck[0];
                deck.Remove(card);
                return card;
            }

            public Card[] DrawHand(List<Card> deck, int handSize)
            {
                if (deck == null || deck.Count < handSize)
                    throw new InvalidOperationException();

                var hand = new List<Card>();
                for (var card = 0; card < handSize; card++)
                {
                    hand.Add(DrawCard(deck));
                }
                return hand.ToArray();
            }
        }

        /// <summary>
        ///     Fake card scoring service with simple rules.
        /// </summary>
        /// <remarks>
        ///     Ace card scores 1, Any other Kind scores 0.
        ///     Red suit doubles card score, Black suit keeps card score.
        /// </remarks>
        public class FakeScoringService : ICardScoringService
        {
            public IEnumerable<ScoredCard> GetScoredCards(Card[] cards, Card wildcard)
            {
                if (cards == null)
                    throw new ArgumentNullException();

                var scoredCards = new List<ScoredCard>();
                foreach (var card in cards)
                {
                    var points = card.Kind == (int)Kinds.Ace ? 1 : 0;
                    points = card.Suit == (int)Suits.Hearts || card.Suit == (int)Suits.Diamonds
                        ? points * 2 : points;
                    scoredCards.Add(new ScoredCard(card.Kind, card.Suit, points));
                }
                return scoredCards;
            }
        }

        [Test]
        public void ShouldThrowOnConstructorIfCardDrawingServiceNull()
        {
            var cds = default(CardDrawingService);
            var css = new CardScoringService();
            var ex = Assert.Throws<ArgumentNullException>(() => new GamePlayingService(cds, css));
            Assert.That(ex.ParamName, Is.EqualTo("drawingService"));
        }

        [Test]
        public void ShouldThrowOnConstructorIfCardScoringServiceNull()
        {
            var cds = new CardDrawingService();
            var css = default(CardScoringService);
            var ex = Assert.Throws<ArgumentNullException>(() => new GamePlayingService(cds, css));
            Assert.That(ex.ParamName, Is.EqualTo("scoringService"));
        }

        [Test]
        public void ShouldThrowOnPlayGameIfPlayerCountDenied([Values(0, 5)]int count)
        {
            var cds = new FakeDrawingService();
            var css = new FakeScoringService();
            var gps = new GamePlayingService(cds, css);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => gps.PlayGame(count));
            Assert.That(ex.Message, Contains.Substring("Player count must be between 1 and 4."));
            // Note: Only 1 to 4 players allowed since wildcard and 4 hands leaves 0 cards.
        }

        [Test]
        public void ShouldPlayGameIfPlayerCountAllowed1()
        {
            var cds = new FakeDrawingService();
            var css = new FakeScoringService();
            var gps = new GamePlayingService(cds, css);

            var gr = gps.PlayGame(1);
            Assert.That(gr.Wildcard, Is.Not.Null);
            Assert.That(gr.Wildcard.Kind, Is.EqualTo((int)Kinds.Any));
            Assert.That(gr.Wildcard.Suit, Is.EqualTo((int)Suits.Any));
            Assert.That(gr.PlayerResults.Length, Is.EqualTo(1));

            var pr1 = gr.PlayerResults[0];
            Assert.That(pr1.Cards.Length, Is.EqualTo(5));
            Assert.That(pr1.Cards.Count(c => c.Kind == (int)Kinds.Ace), Is.EqualTo(1));
            Assert.That(pr1.Cards.Count(c => c.Suit == (int)Suits.Hearts), Is.EqualTo(5));
            Assert.That(pr1.Player, Is.EqualTo(1));
            Assert.That(pr1.Points, Is.EqualTo(2));
        }

        [Test]
        public void ShouldPlayGameIfPlayerCountAllowed4()
        {
            var cds = new FakeDrawingService();
            var css = new FakeScoringService();
            var gps = new GamePlayingService(cds, css);

            var gr = gps.PlayGame(4);
            Assert.That(gr.Wildcard, Is.Not.Null);
            Assert.That(gr.PlayerResults.Length, Is.EqualTo(4));

            var pr1 = gr.PlayerResults[0];
            Assert.That(pr1.Cards.Length, Is.EqualTo(5));
            Assert.That(pr1.Cards.Count(c => c.Kind == (int)Kinds.Ace), Is.EqualTo(1));
            Assert.That(pr1.Cards.Count(c => c.Suit == (int)Suits.Hearts), Is.EqualTo(5));
            Assert.That(pr1.Player, Is.EqualTo(1));
            Assert.That(pr1.Points, Is.EqualTo(2));

            var pr2 = gr.PlayerResults[1];
            Assert.That(pr2.Cards.Length, Is.EqualTo(5));
            Assert.That(pr2.Cards.Count(c => c.Kind == (int)Kinds.Ace), Is.EqualTo(1));
            Assert.That(pr2.Cards.Count(c => c.Suit == (int)Suits.Diamonds), Is.EqualTo(5));
            Assert.That(pr2.Player, Is.EqualTo(2));
            Assert.That(pr2.Points, Is.EqualTo(2));

            var pr3 = gr.PlayerResults[2];
            Assert.That(pr3.Cards.Length, Is.EqualTo(5));
            Assert.That(pr3.Cards.Count(c => c.Kind == (int)Kinds.Ace), Is.EqualTo(1));
            Assert.That(pr3.Cards.Count(c => c.Suit == (int)Suits.Clubs), Is.EqualTo(5));
            Assert.That(pr3.Player, Is.EqualTo(3));
            Assert.That(pr3.Points, Is.EqualTo(1));

            var pr4 = gr.PlayerResults[3];
            Assert.That(pr4.Cards.Length, Is.EqualTo(5));
            Assert.That(pr4.Cards.Count(c => c.Kind == (int)Kinds.Ace), Is.EqualTo(1));
            Assert.That(pr4.Cards.Count(c => c.Suit == (int)Suits.Spades), Is.EqualTo(5));
            Assert.That(pr4.Player, Is.EqualTo(4));
            Assert.That(pr4.Points, Is.EqualTo(1));
        }
    }
}
