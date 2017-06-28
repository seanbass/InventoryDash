using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InventoryDash.Models
{
    public enum category
    {
        beverage, bread, condiment, dairy, produce, protein, togo
    }

    public class Ingredient
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int NumPortions { get; set; }
        public category Category { get; set; }
        public bool UsedInSandwich { get; set; }

        public virtual ICollection<Sandwich> Sandwiches { get; set; }
    }

}