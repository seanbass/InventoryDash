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
    public class InventoryController : Controller
    {
        private InventoryContext db = new InventoryContext();

        // GET: Weekly Inventory Data Entry
        public ActionResult Index()
        {

            int weekOfYear = GetCurrentWeekOfYear();
            DateTime dateNow = DateTime.Now;
            int currentYear = dateNow.Year;
            ViewData["weekOfYear"] = weekOfYear;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = currentYear;

            var sandwichesViewModel = getSandwichesViewModel(currentYear, weekOfYear);

            ViewData["summaryInfo"] = GetListForSummary(currentYear, weekOfYear);

            return View(sandwichesViewModel);
        }



        [HttpGet]
        public ActionResult IndexLoadWeekGet()
        {
            int weekSelected = Convert.ToInt32(TempData["selectedWeek"]);
            int yearSelected = Convert.ToInt32(TempData["selectedYear"]);

            ViewData["weekOfYear"] = weekSelected;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = yearSelected;

            var sandwichesViewModel = getSandwichesViewModel(yearSelected, weekSelected);


            ViewData["summaryInfo"] = GetListForSummary(yearSelected, weekSelected);

            return View("Index", sandwichesViewModel);
        }

        private List<WeeklyInventorySandwichesViewModel> GetListForSummary(int year, int week)
        {
            List<WeeklyInventorySandwichesViewModel> summaryInfo = new List<WeeklyInventorySandwichesViewModel>();

            //Query the database for all sandwiches
            var sandwichesList = (from s in db.Sandwiches
                                  select s).ToList();
            //Cycle through and see if there are weeklyInventorySandwiches entries for each sandwich for the selected year and week
            foreach (var sandwich in sandwichesList)
            {
                var sandwichSales = (from ss in db.WeeklyInventorySandwiches
                                     where ss.SandwichId == sandwich.ID && ss.Year == year && ss.WeekId == week
                                     select ss).ToList();
                if (sandwichSales.Count > 0)
                {
                    //If so, then there are one or two depending on if the sandwich is a breakfast, lunch, or both
                    //Add the data for each sandwich record found, adding together the sandwiches that are both breakfast and lunch
                    int totalQtyDineIn = 0;
                    int totalQtyToGo = 0;
                    decimal totalCost = 0;
                    decimal totalIncome = 0;
                    foreach (var sandwichSaleRecord in sandwichSales)
                    {
                        totalCost += sandwichSaleRecord.Cost;
                        totalIncome += sandwichSaleRecord.Income;
                        totalQtyDineIn += sandwichSaleRecord.QuantityDineIn;
                        totalQtyToGo += sandwichSaleRecord.QuantityToGo;
                    }
                    //Add the information to the list.
                    WeeklyInventorySandwichesViewModel newRecord = new WeeklyInventorySandwichesViewModel()
                    {
                        Name = sandwich.Name,
                        QuantityToGo = totalQtyToGo,
                        QuantityDineIn = totalQtyDineIn,
                        Income = totalIncome,
                        Cost = totalCost,
                        SandwichId = sandwich.ID
                    };
                    summaryInfo.Add(newRecord);
                }
            }

            return summaryInfo;
        }

        [HttpPost]
        public ActionResult IndexLoadWeek()
        {
            int weekSelected = Convert.ToInt32(Request.Form["weekSelect"]);

            int yearSelected = Convert.ToInt32(Request.Form["yearSelect"]);

            ViewData["weekOfYear"] = weekSelected;
            ViewData["startingYear"] = 2016;
            ViewData["selectedYear"] = yearSelected;

            var sandwichesViewModel = getSandwichesViewModel(yearSelected, weekSelected);

            return View("Index", sandwichesViewModel);
        }


        // POST: Index page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID, WeekId, Year, SandwichId, MealId, QuantityToGo, QuantityDineIn, Cost, Income")] WeeklyInventorySandwiches[] weeklyInventorySandwiches)
        {
            int weekSelected = Convert.ToInt32(Request.Form["weekSelect"]);

            int yearSelected = Convert.ToInt32(Request.Form["yearSelect"]);

            int weekOfYear = weeklyInventorySandwiches[0].WeekId;
            int Year = weeklyInventorySandwiches[0].Year;
            //Cycle through the array of Sandwich information and decide if an entry needs to be made.            

            for (int i = 0; i < weeklyInventorySandwiches.Count(); i++)
            {

                //Calculate the cost and income values
                int totalQty = weeklyInventorySandwiches[i].QuantityDineIn + weeklyInventorySandwiches[i].QuantityToGo;
                weeklyInventorySandwiches[i].Cost = Convert.ToDecimal(CalculateSandwichCost(weeklyInventorySandwiches[i].SandwichId, totalQty));
                weeklyInventorySandwiches[i].Income = Convert.ToDecimal(CalculateSandwichIncome(weeklyInventorySandwiches[i].SandwichId, totalQty));


                //Determine if a record already exists - 

                if (weeklyInventorySandwiches[i].ID != 0)
                {
                    //Yes, then update the record.
                    var sandwichIdToQuery = weeklyInventorySandwiches[i].ID;
                    var record = db.WeeklyInventorySandwiches.SingleOrDefault(x => x.ID == sandwichIdToQuery);
                    record.QuantityDineIn = weeklyInventorySandwiches[i].QuantityDineIn;
                    record.QuantityToGo = weeklyInventorySandwiches[i].QuantityToGo;
                    record.Cost = weeklyInventorySandwiches[i].Cost;
                    record.Income = weeklyInventorySandwiches[i].Income;
                    record.MealId = weeklyInventorySandwiches[i].MealId;
                    record.WeekId = weeklyInventorySandwiches[i].WeekId;
                    record.Year = weeklyInventorySandwiches[i].Year;
                    db.SaveChanges();
                }
                else
                {
                    //No, add the record.
                    db.WeeklyInventorySandwiches.Add(weeklyInventorySandwiches[i]);
                    db.SaveChanges();
                }

            }
            //Ensure sandwiches available in multiple meals will show on the list
            //If a weekly inventory record is created for one meal and not the others, then
            // unless records are created for the other meals, then the sandwich will no longer
            // be displayed in the other meal's lists.
            //Get the list of sandwiches marked for both meals
            // For each sandwich, see if there are weeklyInventorySandwich entries for both
            //  meals. If there is not an entry for one of the meals, add it with 0 quantities.
            var sandwichesForBoth = (from s in db.Sandwiches
                                     where s.Meal == InventoryDash.Models.meal.both
                                     select s).ToList();

            foreach (var a in sandwichesForBoth)
            {
                var result = db.WeeklyInventorySandwiches.SingleOrDefault(x => x.SandwichId == a.ID && x.MealId == InventoryDash.Models.meal.breakfast && x.WeekId == weekOfYear && x.Year == Year);
                if (result == null)
                { // There are no records in breakfast, so add one
                    WeeklyInventorySandwiches newRecord = new WeeklyInventorySandwiches() { Cost = 0, Income = 0, MealId = InventoryDash.Models.meal.breakfast, QuantityDineIn = 0, QuantityToGo = 0, SandwichId = a.ID, WeekId = weekOfYear, Year = Year };
                    db.WeeklyInventorySandwiches.Add(newRecord);
                    db.SaveChanges();
                }

                result = db.WeeklyInventorySandwiches.SingleOrDefault(x => x.SandwichId == a.ID && x.MealId == InventoryDash.Models.meal.lunch && x.WeekId == weekOfYear && x.Year == Year);
                if (result == null)
                { // There are no records in breakfast, so add one
                    WeeklyInventorySandwiches newRecord = new WeeklyInventorySandwiches() { Cost = 0, Income = 0, MealId = InventoryDash.Models.meal.lunch, QuantityDineIn = 0, QuantityToGo = 0, SandwichId = a.ID, WeekId = weekOfYear, Year = Year };
                    db.WeeklyInventorySandwiches.Add(newRecord);
                    db.SaveChanges();
                }

            }

            TempData["selectedYear"] = yearSelected;
            TempData["selectedWeek"] = weekSelected;
            return RedirectToAction("IndexLoadWeekGet");

        }


        private double? CalculateSandwichIncome(int sandwichId, int totalQty)
        {
            //Use the sandwich ID to query the Sandwich model to get the current price
            var sandwich = from s in db.Sandwiches
                           where s.ID == sandwichId
                           select s;
            foreach (var a in sandwich)
            {
                return a.Price * totalQty;
            }
            return null;
        }


        private double? CalculateSandwichCost(int sandwichId, int totalQty)
        {
            var sandwich = from s in db.Sandwiches
                           where s.ID == sandwichId
                           select s;
            foreach (var a in sandwich)
            {
                return a.GetCost() * totalQty;
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


        private object getSandwichesViewModel(int selectedYear, int selectedWeek)
        {
            return (from s in db.Sandwiches
                    from so in db.WeeklyInventorySandwiches
                    .Where(orders => s.ID == orders.SandwichId && orders.WeekId == selectedWeek && orders.Year == selectedYear)
                    .DefaultIfEmpty()
                    select new WeeklyInventorySandwichesViewModel
                    {
                        ID = so.ID,
                        WeekId = so.WeekId,
                        Year = so.Year,
                        SandwichId = s.ID,
                        QuantityToGo = so.QuantityToGo,
                        QuantityDineIn = so.QuantityDineIn,
                        MealId = so.MealId,
                        Cost = so.Cost,
                        Income = so.Income,
                        Name = s.Name,
                        Price = s.Price,
                        Meal = s.Meal
                    }).ToList();
        }

    }
}