using Microsoft.EntityFrameworkCore;

namespace ZBase.Persistence {
    public class PlayerDbContext : DbContext {
        public DbSet<IpBanModel> IpBans { get; set; }
        public DbSet<PlayerModel> Players { get; set; }

        public PlayerDbContext() {
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=Database.s3db");
        }
    }
}