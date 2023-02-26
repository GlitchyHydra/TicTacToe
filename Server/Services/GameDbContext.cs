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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<PlayerData> Players { get; set; }
        public DbSet<GameData> Games { get; set; }
        public DbSet<Stats> Statistics { get; set; }
    }
}
