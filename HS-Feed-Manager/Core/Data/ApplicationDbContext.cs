using HS_Feed_Manager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;

namespace HS_Feed_Manager.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TvShow> TvShows { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Test.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
