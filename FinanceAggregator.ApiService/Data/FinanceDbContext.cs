using System;
using Microsoft.EntityFrameworkCore;
using FinanceAggregator.Shared;

namespace FinanceAggregator.ApiService.Data
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Trade> Trades => Set<Trade>();
        public DbSet<Candle> Candles => Set<Candle>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Trade>().HasIndex(t => t.Timestamp);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Fixes the "Cannot write DateTime with Kind=Unspecified" error in Postgres
            configurationBuilder.Properties<DateTime>().HaveConversion<DateTimeToUtcConverter>();
        }

        // Add this nested helper class or put it in a separate file
        public class DateTimeToUtcConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
        {
            public DateTimeToUtcConverter()
                : base(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
            { }
        }
    }
}
