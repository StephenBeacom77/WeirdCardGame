using WeirdCardGame.Data;

namespace WeirdCardGame.Models
{
    /// <summary>
    ///     Models a scored card in the weird game of cards.
    /// </summary>
    public class ScoredCard : Card
    {
        public ScoredCard(Kinds kind, Suits suit, int points = 0)
            : base(kind, suit)
        {
            Points = points;
        }

        public ScoredCard(int kind, int suit, int points = 0)
            : base(kind, suit)
        {
            Points = points;
        }

        public int Points { get; private set; }
    }
}
