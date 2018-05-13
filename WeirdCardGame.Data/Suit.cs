namespace WeirdCardGame.Data
{
    /// <summary>
    ///     Models info for a card suit in the weird game of cards.
    /// </summary>
    public class Suit
    {
        public int Id { get; set; }
        public string Symbol { get; set; }

        public Suit()
        {
        }

        public Suit(Suits key, string symbol)
        {
            Id = (int)key;
            Symbol = symbol;
        }
    }
}
