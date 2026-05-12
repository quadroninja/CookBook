using CookBookBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace CookBookBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DishIngredient>()
                .HasOne(di => di.Dish)
                .WithMany(d => d.Ingredients)
                .HasForeignKey(di => di.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DishIngredient>()
                .HasOne(di => di.FoodItem)
                .WithMany(fi => fi.DishesWithThisIngredient)
                .HasForeignKey(di => di.FoodItemId)
                .OnDelete(DeleteBehavior.Restrict);

            if (Database.IsSqlite())
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.GetProperties()
                        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

                    foreach (var property in properties)
                    {
                        property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<decimal, double>(
                            v => (double)v,
                            v => (decimal)v));
                    }
                }
            }

        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var entity = (BaseEntity)entityEntry.Entity;
                if (entityEntry.State == EntityState.Added)
                    entity.CreatedOn = DateTimeOffset.UtcNow;
                if (entityEntry.State == EntityState.Modified)
                    entity.UpdatedOn = DateTimeOffset.UtcNow;
            }

            return base.SaveChangesAsync();
        }

        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }

    }
}
