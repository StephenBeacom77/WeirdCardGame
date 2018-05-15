using System;
using NUnit.Framework;
using WeirdCardGame.Data;

namespace WeirdCardGame.UnitTests.Data
{
    [TestFixture]
    public class SuitsTest
    {
        [Test]
        public void ShouldHaveEverySuitInEnum()
        {
            var suits = Enum.GetValues(typeof(Suits)) as Suits[];
            Assert.That(suits.Length, Is.EqualTo(5)); // Any + the 4 real suits
        }

        [Test]
        public void ShouldAddSymbolForEverySuit()
        {
            var suits = Enum.GetValues(typeof(Suits)) as Suits[];
            foreach (var suit in suits)
            {
                var newSuit = new Suit(suit, "X");
                Assert.That(newSuit.Id, Is.EqualTo((int)suit));
                Assert.That(newSuit.Symbol, Is.EqualTo("X"));
            }
        }
    }
}
