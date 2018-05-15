using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeirdCardGame.Data;
using WeirdCardGame.Models;
using WeirdCardGame.Services;

namespace WeirdCardGame.Controllers
{
    /// <summary>
    ///     Controller for game of cards with weird rules.
    /// </summary>
    [Route("api/[controller]")]
    public sealed class CardGameController : Controller
    {
        private readonly GameContext _gameContext;
        private readonly ICardDrawingService _drawingService;
        private readonly ICardScoringService _scoringService;
        private readonly IGamePlayingService _playingService;
        private readonly IGameHistoryService _historyService;

        public CardGameController(
            GameContext gameContext,
            ICardDrawingService drawingService,
            ICardScoringService scoringService,
            IGamePlayingService playingService,
            IGameHistoryService historyService)
        {
            _gameContext = gameContext
                ?? throw new ArgumentNullException(nameof(gameContext));
            _drawingService = drawingService
                ?? throw new ArgumentNullException(nameof(drawingService));
            _scoringService = scoringService
                ?? throw new ArgumentNullException(nameof(scoringService));
            _playingService = playingService
                ?? throw new ArgumentNullException(nameof(playingService));
            _historyService = historyService
                ?? throw new ArgumentNullException(nameof(historyService));
        }

        /// <summary>
        ///     Get the cards that help demonstrate the rules of the game.
        /// </summary>
        [HttpGet("[action]")]
        public Card[] GetRuleCards()
        {
            var cards = new Card[]
            {
                new Card(Kinds.Ace, Suits.Any),
                new Card(Kinds.King, Suits.Any),
                new Card(Kinds.Queen, Suits.Any),
                new Card(Kinds.Jack, Suits.Any),
                new Card(Kinds.Ten, Suits.Any),
                new Card(Kinds.Any, Suits.Any),
            };
            return _scoringService.GetScoredCards(cards, null).ToArray();
        }

        /// <summary>
        ///     Get the kinds of cards available in the game.
        /// </summary>
        [HttpGet("[action]")]
        public Kind[] GetCardKinds()
        {
            return _gameContext.Kinds.AsNoTracking().ToArray();
        }

        /// <summary>
        ///     Get the suits of cards available in the game.
        /// </summary>
        [HttpGet("[action]")]
        public Suit[] GetCardSuits()
        {
            return _gameContext.Suits.AsNoTracking().ToArray();
        }

        /// <summary>
        ///     Play one round of the game with the specified number of players.
        /// </summary>
        [HttpGet("[action]")]
        public GameResult PlayGame(int playerCount)
        {
            var gameResult = _playingService.PlayGame(playerCount);
            _historyService.Save(gameResult);
            return gameResult;
        }

        /// <summary>
        ///     Get the next round to be played.
        /// </summary>
        [HttpGet("[action]")]
        public int GetNextRound()
        {
            return _gameContext.Games.Any() ? _gameContext.Games.Max(g => g.Id) + 1 : 1;
        }

        /// <summary>
        ///     Get the winners of games played.
        /// </summary>
        [HttpGet("[action]")]
        public Game[] GetWinnersList()
        {
            return _gameContext.Games.AsNoTracking().ToArray();
        }
    }
}
