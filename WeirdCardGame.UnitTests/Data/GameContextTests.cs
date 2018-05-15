using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WeirdCardGame.Data;

namespace WeirdCardGame.UnitTests.Data
{
    [TestFixture]
    public class GameContextTests
    {
        GameContext _gameContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<GameContext>()
                            .UseInMemoryDatabase(databaseName: "Test")
                            .Options;
            _gameContext = new GameContext(options);
        }

        [Test]
        public void ShouldAddGameWithGeneratedIdAndPlayerId(
            [Values(123, null)]int? playerId)
        {
            var game = _gameContext.Games.Add(new Game() { PlayerId = playerId });
            Assert.That(game.Entity.Id, Is.GreaterThan(0));
            Assert.That(game.Entity.PlayerId, Is.EqualTo(playerId));
        }

        [Test]
        public void ShouldAddKindWithIdAndSymbol(
            [Values(Kinds.Any, Kinds.Ace)] Kinds kindValue,
            [Values("A", "-", "12345")] string kindSymbol)
        {
            var kind = _gameContext.Kinds.Add(new Kind(kindValue, kindSymbol));
            Assert.That(kind.Entity.Id, Is.EqualTo((int)kindValue));
            Assert.That(kind.Entity.Symbol, Is.EqualTo(kindSymbol));
        }

        [Test]
        public void ShouldAddSuitWithIdAndSymbol(
            [Values(Suits.Any, Suits.Hearts)] Suits suitValue,
            [Values("A", "-", "12345")] string suitSymbol)
        {
            var suit = _gameContext.Suits.Add(new Suit(suitValue, suitSymbol));
            Assert.That(suit.Entity.Id, Is.EqualTo((int)suitValue));
            Assert.That(suit.Entity.Symbol, Is.EqualTo(suitSymbol));
        }
    }
}
