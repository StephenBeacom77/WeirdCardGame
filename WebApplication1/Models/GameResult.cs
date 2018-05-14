namespace WeirdCardGame.Models
{
    public class GameResult
    {
        public Card Wildcard { get; set; }
        public PlayerResult[] PlayerResults { get; set; }
    }
}
