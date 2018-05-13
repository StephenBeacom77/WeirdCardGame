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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasKey(g => new { g.Id });
            modelBuilder.Entity<Game>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Kind>()
                .HasKey(g => new { g.Id });
            modelBuilder.Entity<Kind>()
                .Property(g => g.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Suit>()
                .HasKey(g => new { g.Id });
            modelBuilder.Entity<Suit>()
                .Property(g => g.Id)
                .ValueGeneratedNever();
        }
    }
}
