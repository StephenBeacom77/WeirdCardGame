namespace WeirdCardGame.Data
{
    /// <summary>
    ///     Models the suits of card in the weird game of cards.
    /// </summary>
    public enum Suits
    {
        None = 0,
        Hearts = 1,
        Clubs = 2,
        Diamonds = 3,
        Spades = 4
    }

    public class Suit
    {
        public int Id;
        public string Symbol;
    }
}
