using HS_Feed_Manager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using Serilog;

namespace HS_Feed_Manager.Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<TvShow> TvShows { get; set; }
        public DbSet<SqLiteLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlite("Data Source=local.db");
                base.OnConfiguring(optionsBuilder);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"UpdateLocalTvShows Error!");
            }
        }
    }
}
