using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace InventoryDash.Models
{
    public enum meal
    {
        breakfast, lunch, both
    }

    public class Sandwich
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        [Display(Name = "Cost")]
        public string CurrentCost; //current cost should only be set from within GetCost method. It should be read only immidiately after calling this method.
        public meal Meal { get; set; }
        [Display(Name = "Ingredients")]
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        public double GetCost()
        {

            double temp = 0.0;
            double output;
            //Sandwich sandwich = db.Sandwiches.Find(id);
            foreach (Ingredient ing in this.Ingredients)
            {
                temp += ing.Price / ing.NumPortions;
            }
            output = temp * 100;
            output = Math.Round(output);
            output /= 100;
            CurrentCost = "$" + output.ToString();
            return temp;
        }

        public string PrintCost()
        {
            double temp = 0.0;
            double output;
            //Sandwich sandwich = db.Sandwiches.Find(id);
            foreach (Ingredient ing in this.Ingredients)
            {
                temp += ing.Price / ing.NumPortions;
            }
            output = temp * 100;
            output = Math.Round(output);
            output /= 100;
            CurrentCost = "$" + output.ToString();
            return CurrentCost;
        }

        public string GetIngredients(Sandwich sandwich)
        {
            StringBuilder ingredientList = new StringBuilder();
            foreach (Ingredient i in sandwich.Ingredients)
            {
                string ingredientName = i.Name;
                ingredientList.Append(ingredientName);
                ingredientList.Append(", ");
            }
            int index = ingredientList.Length;
            if (index >= 2)
            {
                ingredientList.Remove(index - 2, 2);
            }
            return ingredientList.ToString();
        }
    }
}