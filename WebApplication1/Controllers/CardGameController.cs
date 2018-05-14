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
    /// <remarks>
    ///     todo: fill in the readme file
    ///         - using VS2017, EF Core (memory mode), nUnit, Moq
    ///         - brief notes on
    ///             - clear winner versus tie - only clear winner saved to db
    ///             - this means that any tie should be shown as such to be consistent
    ///     todo: remove unused code
    ///     todo: component for drawing card in ts/angular
    ///     todo: client side service for talking to server
    ///     todo: package the solution by cloning to a different path!
    ///     
    ///     todo: add some unit tests for server code of significance
    ///         - indicate in readme that not all code is tested due to lack of time
    ///             - indicate what else should be tested?
    ///         - CardGameController
    ///             - GetRuleCards
    ///                 - single test to check for all rule cards
    ///             - PlayGame
    ///                 - multiple tests to check for different player counts
    ///                     - allowed: 1, 2, 9, 10
    ///                     - invalid: 0, 11
    ///                 - probably need a CardScoringService to score cards on kind and suit
    ///                     - it has GetScoredCard(card, wildcard)
    ///                         - it has logic from GetPointsForKind and GetScoredCard
    ///                     - it can be tested and mocked for other tests
    ///             - Deck
    ///                 - test that it has 52 cards
    ///                     - by drawing them one by one
    ///                         - check off every combo of kind and suit with zero points
    ///                         - the 53rd draw should fail
    /// </remarks>
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
        ///     Get the winners of games played.
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public Game[] GetWinnersList()
        {
            return _gameContext.Games.AsNoTracking().ToArray();
        }
    }
}
