using System;

namespace WeirdCardGame.Data
{
    public class GameContextSeeder
    {
        private readonly GameContext _gameContext;

        public GameContextSeeder(GameContext gameContext)
        {
            _gameContext = gameContext ??
                throw new ArgumentNullException(nameof(gameContext));
        }

        public void AddCardData(GameContext context)
        {
            AddKindData(context);
            AddSuitData(context);
        }

        private void AddKindData(GameContext context)
        {
            context.Kinds.RemoveRange(context.Kinds);
            context.Kinds.Add(new Kind(Kinds.Any, "?"));
            context.Kinds.Add(new Kind(Kinds.Ace, "A"));
            context.Kinds.Add(new Kind(Kinds.Two, "2"));
            context.Kinds.Add(new Kind(Kinds.Three, "3"));
            context.Kinds.Add(new Kind(Kinds.Four, "4"));
            context.Kinds.Add(new Kind(Kinds.Five, "5"));
            context.Kinds.Add(new Kind(Kinds.Six, "6"));
            context.Kinds.Add(new Kind(Kinds.Seven, "7"));
            context.Kinds.Add(new Kind(Kinds.Eight, "8"));
            context.Kinds.Add(new Kind(Kinds.Nine, "9"));
            context.Kinds.Add(new Kind(Kinds.Ten, "10"));
            context.Kinds.Add(new Kind(Kinds.Jack, "J"));
            context.Kinds.Add(new Kind(Kinds.Queen, "Q"));
            context.Kinds.Add(new Kind(Kinds.King, "K"));
            context.SaveChanges();
        }

        private void AddSuitData(GameContext context)
        {
            context.Suits.Add(new Suit(Suits.Any, "?"));
            context.Suits.Add(new Suit(Suits.Hearts, "\u2665"));
            context.Suits.Add(new Suit(Suits.Clubs, "\u2663"));
            context.Suits.Add(new Suit(Suits.Diamonds, "\u2666"));
            context.Suits.Add(new Suit(Suits.Spades, "\u2660"));
            context.SaveChanges();
        }
    }
}
