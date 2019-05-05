using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace HS_Feed_Manager.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TvShow> TvShows { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlite("Data Source=local.db");
                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("UpdateLocalTvShows: " + ex, LogLevel.Error);
            }
        }
    }
}
