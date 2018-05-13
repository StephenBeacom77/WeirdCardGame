using Microsoft.EntityFrameworkCore;

namespace WeirdCardGame.Data
{
    public class GameContext : DbContext
    {
        public DbSet<Kind> Kinds { get; private set; }
        public DbSet<Suit> Suits { get; private set; }
        public DbSet<Game> Games { get; private set; }

        public GameContext(DbContextOptions<GameContext> dbOptions)
            : base(dbOptions)
        {

        }
    }
}
