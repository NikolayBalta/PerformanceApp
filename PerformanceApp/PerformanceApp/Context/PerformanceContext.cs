using Microsoft.EntityFrameworkCore;
using PerformanceApp.Entities;

namespace PerformanceApp.Context
{
    public class PerformanceContext: DbContext
    {
        public PerformanceContext(DbContextOptions<PerformanceContext> options): base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().Property(f => f.Id).ValueGeneratedOnAdd();
        }
        public DbSet<Product> Products { get; set; }
    }
}
