using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InventoryDash.Models
{
    public class Order
    {
        [Key]
        public int MyID { get; set; }
        public int IngredientID { get; set; }
        public double Cost { get; set; }
        public double Portions { get; set; }
        public double PortionsRemaining { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
    }
}