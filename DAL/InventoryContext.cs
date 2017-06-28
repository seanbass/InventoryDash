using InventoryDash.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InventoryDash.DAL
{
    public class InventoryContext : DbContext
    {

        public InventoryContext() : base("InventoryContext")
        {

        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Sandwich> Sandwiches { get; set; }

        public DbSet<WeeklyInventorySandwiches> WeeklyInventorySandwiches { get; set; }
        public DbSet<WeeklyInventoryDrinks> WeeklyInventoryDrinks { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}