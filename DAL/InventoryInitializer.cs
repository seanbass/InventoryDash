using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using InventoryDash.Models;

namespace InventoryDash.DAL
{
    //public class InventoryInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<InventoryContext>
    public class InventoryInitializer : System.Data.Entity.DropCreateDatabaseAlways<InventoryContext>
    {
        protected override void Seed(InventoryContext context)
        {
            var ingredients = new List<Ingredient>
            {
            new Ingredient { Name = "white bread", Category = category.bread, Price=5, NumPortions=20 },
            new Ingredient { Name = "butter", Category = category.dairy, Price = 6, NumPortions=24},
            new Ingredient { Name = "pepperjack cheese", Category = category.dairy, Price=8, NumPortions=20}
            };
            ingredients.ForEach(s => context.Ingredients.Add(s));
            context.SaveChanges();

            var sandwiches = new List<Sandwich>
            {
            new Sandwich {Name="White bread toast & butter... add jam", Price=1.50, Ingredients = new List<Ingredient> { new Ingredient { Name = "white bread"}, new Ingredient { Name = "butter"}, new Ingredient { Name = "jam"} } },
            new Sandwich {Name="Fried egg, cheddar or pepper jack cheese", Price=3.00, Ingredients = new List<Ingredient> { new Ingredient { Name = "English muffin"}, new Ingredient { Name = "cheddar cheese"}, new Ingredient { Name = "pepper jack cheese"} } }
            };
            sandwiches.ForEach(s => context.Sandwiches.Add(s));
            context.SaveChanges();


        }
    }
}