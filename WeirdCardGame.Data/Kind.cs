namespace WeirdCardGame.Data
{
    /// <summary>
    ///     Models info for a card kind in the weird game of cards.
    /// </summary>
    public class Kind
    {
        public int Id { get; set; }
        public string Symbol { get; set; }

        public Kind()
        {
        }

        public Kind(Kinds key, string symbol)
        {
            Id = (int)key;
            Symbol = symbol;
        }
    }
}
