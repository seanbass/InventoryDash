using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryDash.DAL;
using InventoryDash.Models;
using System.Dynamic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Web.UI.WebControls;
using System.Globalization;

namespace InventoryDash.Controllers
{
    public class WeeklyInventoryDrinksController : Controller
    {
        private InventoryContext db = new InventoryContext();

        // GET: Weekly Drink Sales Data Entry
        public ActionResult Index()
        {

            int weekOfYear = GetCurrentWeekOfYear();
            DateTime dateNow = DateTime.Now;
            int currentYear = dateNow.Year;
            ViewData["weekOfYear"] = weekOfYear;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = currentYear;

            var beveragesViewModel = GetViewModel(currentYear, weekOfYear);


            return View(beveragesViewModel);
        }



        [HttpGet]
        public ActionResult IndexLoadWeekGet()
        {
            int weekSelected = Convert.ToInt32(TempData["selectedWeek"]);
            int yearSelected = Convert.ToInt32(TempData["selectedYear"]);

            ViewData["weekOfYear"] = weekSelected;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = yearSelected;

            var beveragesViewModel = GetViewModel(yearSelected, weekSelected);


            return View("Index", beveragesViewModel);
        }



        [HttpPost]
        public ActionResult IndexLoadWeek()
        {
            int weekSelected = Convert.ToInt32(Request.Form["weekSelect"]);
            int yearSelected = Convert.ToInt32(Request.Form["yearSelect"]);

            ViewData["weekOfYear"] = weekSelected;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = yearSelected;

            var beveragesViewModel = GetViewModel(yearSelected, weekSelected);


            return View("Index", beveragesViewModel);
        }


        // POST: Index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID, WeekId, Year, DrinkId, QuantityToGo, QuantityDineIn, Cost, Income")] WeeklyInventoryDrinks[] weeklyInventoryDrinks)
        {
            int weekSelected = Convert.ToInt32(Request.Form["weekSelect"]);
            int yearSelected = Convert.ToInt32(Request.Form["yearSelect"]);

            int weekOfYear = weeklyInventoryDrinks[0].WeekId;
            int Year = weeklyInventoryDrinks[0].Year;
            //Cycle through the array of Sandwich information and decide if an entry needs to be made.            

            for (int i = 0; i < weeklyInventoryDrinks.Count(); i++)
            {
                // if(weeklyInventorySandwiches[i].ID != 0 || weeklyInventorySandwiches[i].QuantityDineIn != 0 || weeklyInventorySandwiches[i].QuantityToGo != 0)
                //{
                //Some quantity information was provided
                //Calculate the cost and income values
                int totalQty = Convert.ToInt32(weeklyInventoryDrinks[i].QuantityDineIn) + Convert.ToInt32(weeklyInventoryDrinks[i].QuantityToGo);
                weeklyInventoryDrinks[i].Cost = Convert.ToDecimal(CalculateDrinkCost(weeklyInventoryDrinks[i].DrinkId, totalQty));
                weeklyInventoryDrinks[i].Income = Convert.ToDecimal(CalculateDrinkIncome(weeklyInventoryDrinks[i].DrinkId, totalQty));


                //Determine if a record already exists - 
                if (weeklyInventoryDrinks[i].ID != 0)
                {
                    //Yes, then update the record.
                    var drinkIdToQuery = weeklyInventoryDrinks[i].ID;
                    var record = db.WeeklyInventoryDrinks.SingleOrDefault(x => x.ID == drinkIdToQuery);
                    record.QuantityDineIn = weeklyInventoryDrinks[i].QuantityDineIn;
                    record.QuantityToGo = weeklyInventoryDrinks[i].QuantityToGo;
                    record.Cost = weeklyInventoryDrinks[i].Cost;
                    record.Income = weeklyInventoryDrinks[i].Income;
                    record.WeekId = weeklyInventoryDrinks[i].WeekId;
                    record.Year = weeklyInventoryDrinks[i].Year;
                    db.SaveChanges();
                }
                else
                {
                    //No, add the record.
                    db.WeeklyInventoryDrinks.Add(weeklyInventoryDrinks[i]);
                    db.SaveChanges();
                }
                //}
            }


            TempData["selectedYear"] = yearSelected;
            TempData["selectedWeek"] = weekSelected;
            return RedirectToAction("IndexLoadWeekGet"); // Stored year and week info in temp data, have to go to a get based action next.

        }

        private double? CalculateDrinkIncome(int drinkId, int totalQty)
        {
            //Use the sandwich ID to query the Sandwich model to get the current price
            var drinksList = from s in db.Ingredients
                           .Where(drinks => drinks.Category == InventoryDash.Models.category.beverage)
                             where s.ID == drinkId
                             select s;
            foreach (var a in drinksList)
            {
                return a.Price * totalQty;
            }
            return null;
        }

        private double? CalculateDrinkCost(int drinkId, int totalQty)
        {
            //Get the cost of the drink from the Ingredients table / helper method.
            var drinksList = from s in db.Ingredients
                           .Where(drinks => drinks.Category == InventoryDash.Models.category.beverage)
                             where s.ID == drinkId
                             select s;
            foreach (var a in drinksList)
            {
                return (a.Price / a.NumPortions) * totalQty;
            }
            return null;
        }

        public int GetCurrentWeekOfYear()
        {
            //Getting the week number
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            DateTime date1 = DateTime.Today;
            System.Globalization.Calendar cal = dfi.Calendar;

            int weekOfYear = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);

            return weekOfYear;
        }



        private object GetViewModel(int currentYear, int weekOfYear)
        {
            return (from s in db.Ingredients
                    .Where(drinks => drinks.Category == InventoryDash.Models.category.beverage)
                    from so in db.WeeklyInventoryDrinks
                    .Where(orders => s.ID == orders.DrinkId && orders.WeekId == weekOfYear && orders.Year == currentYear)
                    .DefaultIfEmpty()
                    select new WeeklyInventoryDrinksViewModel
                    {
                        ID = so.ID,
                        WeekId = so.WeekId,
                        Year = so.Year,
                        DrinkId = s.ID,
                        QuantityToGo = so.QuantityToGo,
                        QuantityDineIn = so.QuantityDineIn,
                        Cost = so.Cost,
                        Income = so.Income,
                        Name = s.Name,
                        Price = s.Price,
                    }).ToList();
        }

    }
}