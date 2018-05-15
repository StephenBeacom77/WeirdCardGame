using WeirdCardGame.Data;

namespace WeirdCardGame.Models
{
    /// <summary>
    ///     Models a card in the weird game of cards.
    /// </summary>
    public class Card
    {
        public Card(Kinds kind, Suits suit)
        {
            Kind = (int)kind;
            Suit = (int)suit;
        }

        public Card(int kind, int suit)
        {
            Kind = kind;
            Suit = suit;
        }

        public int Kind { get; private set; }
        public int Suit { get; private set; }
    }
}
