using System;
using NUnit.Framework;
using WeirdCardGame.Data;

namespace WeirdCardGame.UnitTests.Data
{
    [TestFixture]
    public class KindsTest
    {
        [Test]
        public void ShouldHaveEveryKindInEnum()
        {
            var kinds = Enum.GetValues(typeof(Kinds)) as Kinds[];
            Assert.That(kinds.Length, Is.EqualTo(14)); // Any + the 13 real kinds
        }

        [Test]
        public void ShouldAddSymbolForEveryKind()
        {
            var kinds = Enum.GetValues(typeof(Kinds)) as Kinds[];
            foreach (var suit in kinds)
            {
                var newKind = new Kind(suit, "X");
                Assert.That(newKind.Id, Is.EqualTo((int)suit));
                Assert.That(newKind.Symbol, Is.EqualTo("X"));
            }
        }
    }
}
