using WeirdCardGame.Data;

namespace WeirdCardGame.Models
{
    /// <summary>
    ///     Models a card in the weird game of cards.
    /// </summary>
    public partial class Card
    {
        public Card(Kinds kind, Suits suit, int points = 0)
        {
            Kind = (int)kind;
            Suit = (int)suit;
            Points = points;
        }

        public Card(int kind, int suit, int points = 0)
        {
            Kind = kind;
            Suit = suit;
            Points = points;
        }

        public int Kind { get; private set; }
        public int Suit { get; private set; }
        public int Points { get; private set; }
    }
}
