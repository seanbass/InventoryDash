using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventoryDash.Models
{
    public class WeeklyInventoryExtras
    {
        public int ID { get; set; }
        public int WeekId { get; set; }
        public int ExtraId { get; set; }
        public int QuantityToGo { get; set; }
        public int QuantityDineIn { get; set; }
        public decimal Cost { get; set; }
        public decimal Income { get; set; }
    }
}