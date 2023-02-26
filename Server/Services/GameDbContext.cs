using Microsoft.EntityFrameworkCore;
using Npgsql;
using Server.Model.Db;

namespace Server.Services
{
    public class GameDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public GameDbContext(DbContextOptions<GameDbContext> options,
            IConfiguration configuration) : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<PlayerRole>("player_role");
            
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            //optionsBuilder.Options.ContextType.Map
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PlayerData>()
            //    .Property(e => e.Role)
            //    .HasConversion(
            //        v => v.ToString(),
            //        v => (PlayerRole) Enum.Parse(typeof(PlayerRole), v));
        }

        public DbSet<PlayerData> Players { get; set; }
        public DbSet<GameData> Games { get; set; }
        public DbSet<Stats> Statistics { get; set; }
    }
}
