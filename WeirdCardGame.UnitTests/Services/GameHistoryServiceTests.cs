using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WeirdCardGame.Data;
using WeirdCardGame.Models;
using WeirdCardGame.Services;

namespace WeirdCardGame.UnitTests.Services
{
    [TestFixture]
    public class GameHistoryServiceTests
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
        public void ShouldThrowOnSaveGameAsGameResultIsNull()
        {
            var ghs = new GameHistoryService(_gameContext);
            var ex = Assert.Throws<ArgumentNullException>(() => ghs.Save(null));
            Assert.That(ex.ParamName, Is.EqualTo("gameResult"));
        }

        [Test]
        public void ShouldThrowOnSaveGameAsPlayerResultArrayIsNull()
        {
            var ghs = new GameHistoryService(_gameContext);
            var gameResult = new GameResult() { PlayerResults = null };
            var ex = Assert.Throws<ArgumentNullException>(() => ghs.Save(gameResult));
            Assert.That(ex.ParamName, Is.EqualTo("gameResult.PlayerResults"));
        }

        [Test]
        public void ShouldThrowOnSaveGameAsPlayerResultZeroIsNull()
        {
            var ghs = new GameHistoryService(_gameContext);
            var gameResult = new GameResult() { PlayerResults = new PlayerResult[1] };
            var ex = Assert.Throws<ArgumentNullException>(() => ghs.Save(gameResult));
            Assert.That(ex.ParamName, Is.EqualTo("gameResult.PlayerResults.0"));
        }

        [Test]
        public void ShouldSaveGameWithPlayerIdNotNullAsResultHasSingleHighScore()
        {
            var ghs = new GameHistoryService(_gameContext);
            var gameResult = new GameResult
            {
                PlayerResults = new PlayerResult[]
                {
                    GetPlayerResult(player: 234, points: 111),
                    GetPlayerResult(player: 123, points: 222),
                }
            };

            var savedGame = ghs.Save(gameResult);
            Assert.That(savedGame.Id, Is.GreaterThan(0));
            Assert.That(savedGame.PlayerId, Is.EqualTo(234));

            var foundGame = _gameContext.Games.Single(x => x.Id == savedGame.Id);
            Assert.That(savedGame.PlayerId, Is.EqualTo(savedGame.PlayerId));
        }

        [Test]
        public void ShouldSaveGameWithPlayerIdNullAsResultHasMultipleHighScores()
        {
            var ghs = new GameHistoryService(_gameContext);
            var gameResult = new GameResult
            {
                PlayerResults = new PlayerResult[]
                {
                    GetPlayerResult(player: 234, points: 111),
                    GetPlayerResult(player: 123, points: 111),
                }
            };

            var savedGame = ghs.Save(gameResult);
            Assert.That(savedGame.Id, Is.GreaterThan(0));
            Assert.That(savedGame.PlayerId, Is.EqualTo(null));

            var foundGame = _gameContext.Games.Single(x => x.Id == savedGame.Id);
            Assert.That(savedGame.PlayerId, Is.EqualTo(savedGame.PlayerId));
        }

        private static PlayerResult GetPlayerResult(int player, int points)
        {
            return new PlayerResult { Player = player, Points = points };
        }
    }
}
