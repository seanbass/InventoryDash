using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryDash.Models
{
    public class WeeklyInventoryDrinksViewModel
    {
        public int? ID { get; set; }
        public int? WeekId { get; set; }
        public int? Year { get; set; }
        public int DrinkId { get; set; } //This is ID from the ingredients table.
        public int? QuantityToGo { get; set; }
        public int? QuantityDineIn { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Income { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
    }
}