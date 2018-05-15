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
    public class CardScoringServiceTests
    {
        [Test]
        public void ShouldThrowOnGetScoredCardsIfCardsNull()
        {
            var css = new CardScoringService();

            var scoredCards = css.GetScoredCards(null, null);

            var ex = Assert.Throws<ArgumentNullException>(() => scoredCards.ToArray());
            Assert.That(ex.ParamName, Is.EqualTo("cards"));
        }

        [Test]
        public void ShouldGetScoredCardsIfWildcardNull(
            [Values(Suits.Any, Suits.Clubs, Suits.Diamonds, Suits.Hearts, Suits.Spades)] Suits suit)
        {
            var css = new CardScoringService();
            var cards = GetSuitOfCards(suit);

            var scoredCards = css.GetScoredCards(cards);
            foreach(var scoredCard in scoredCards)
            {
                Assert.That((Suits)scoredCard.Suit, Is.EqualTo(suit));
                var expectedPoints = GetPointsForKind((Kinds)scoredCard.Kind);
                Assert.That(scoredCard.Points, Is.EqualTo(expectedPoints));
            }
        }

        [Test]
        public void ShouldGetScoredCardsIfWildcardSuitsAreMatching(
            [Values(Suits.Clubs, Suits.Diamonds, Suits.Hearts, Suits.Spades)] Suits suit)
        {
            var css = new CardScoringService();
            var cards = GetSuitOfCards(suit);
            var wildcard = new Card(Kinds.Any, suit);

            var scoredCards = css.GetScoredCards(cards, wildcard);
            foreach (var scoredCard in scoredCards)
            {
                Assert.That((Suits)scoredCard.Suit, Is.EqualTo(suit));
                var expectedPoints = GetPointsForKind((Kinds)scoredCard.Kind) * 2;
                Assert.That(scoredCard.Points, Is.EqualTo(expectedPoints));
            }
        }

        [Test]
        public void ShouldGetScoredCardsIfWildcardSuitsNotMatching(
            [Values(Suits.Clubs, Suits.Diamonds, Suits.Hearts, Suits.Spades)] Suits suit)
        {
            var css = new CardScoringService();
            var cards = GetSuitOfCards(suit);
            var wildcard = new Card(Kinds.Any, Suits.Any);

            var scoredCards = css.GetScoredCards(cards, wildcard);
            foreach (var scoredCard in scoredCards)
            {
                Assert.That((Suits)scoredCard.Suit, Is.EqualTo(suit));
                var expectedPoints = GetPointsForKind((Kinds)scoredCard.Kind);
                Assert.That(scoredCard.Points, Is.EqualTo(expectedPoints));
            }
        }

        private static int GetPointsForKind(Kinds kind)
        {
            switch (kind)
            {
                case Kinds.Ace:
                    return CardScoringService.PointsForAce;
                case Kinds.Ten:
                    return CardScoringService.PointsForTen;
                case Kinds.King:
                    return CardScoringService.PointsForKing;
                case Kinds.Queen:
                    return CardScoringService.PointsForQueen;
                case Kinds.Jack:
                    return CardScoringService.PointsForJack;
            }
            return CardScoringService.PointsForOther;
        }

        private static Card[] GetSuitOfCards(Suits suit)
        {
            var kinds = Enum.GetValues(typeof(Kinds)) as Kinds[];
            var cards = new List<Card>();
            foreach (var kind in kinds)
            {
                if (kind == Kinds.Any) continue;
                cards.Add(new Card(kind, suit));
            }
            return cards.ToArray();
        }
    }
}
