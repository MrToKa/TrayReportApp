using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrayReportApp.Models;

namespace TrayReportApp.Data
{
    public class TrayReportAppDbContext : DbContext
    {
        public TrayReportAppDbContext()
        {
        }

        public TrayReportAppDbContext(DbContextOptions<TrayReportAppDbContext> options)
            : base(options)
        {
        }

        private string BuildConnectionString()
        {
            IConfigurationRoot? configuration = new ConfigurationBuilder()
                .SetBasePath(@"C:\Users\TOKA\source\repos\TrayReportApp\TrayReportApp")
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(BuildConnectionString(),
options => options.UseAdminDatabase("postgres"));
        }

        public DbSet<Cable> Cables { get; set; }
        public DbSet<CableType> CableTypes { get; set; }
        public DbSet<Support> Supports { get; set; }
        public DbSet<Tray> Trays { get; set; }
        public DbSet<TrayCable> TraysCables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrayCable>()
                .HasKey(tc => new { tc.CableId, tc.TrayId });

            base.OnModelCreating(modelBuilder);
        }
    }
}