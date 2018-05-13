namespace WeirdCardGame.Data
{
    public class Game
    {
        public int Id { get; set; }
        public int? PlayerId { get; set; }

        public Game()
        {
        }

        public Game(int id, int? playerId)
        {
            Id = id;
            PlayerId = playerId;
        }
    }
}
