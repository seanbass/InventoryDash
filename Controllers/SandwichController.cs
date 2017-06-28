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
using System.Configuration;
using System.Data.SqlClient;

namespace InventoryDash.Controllers
{
    public class SandwichController : Controller
    {
        private InventoryContext db = new InventoryContext();
        public int currentID;

        // GET: Sandwich
        public ActionResult Index()
        {
            return View(db.Sandwiches.ToList());
        }

        // GET: Sandwich/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sandwich sandwich = db.Sandwiches.Find(id);
            if (sandwich == null)
            {
                return HttpNotFound();
            }
            return View(sandwich);
        }

        // GET: Sandwich/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sandwich/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Price,Meal")] Sandwich sandwich)
        {
            if (ModelState.IsValid)
            {
                db.Sandwiches.Add(sandwich);
                db.SaveChanges();
                db.Entry(sandwich).GetDatabaseValues();
                this.currentID = sandwich.ID;
                return RedirectToAction("AddIngredients", new { id = sandwich.ID });
            }

            return View(sandwich);
        }

        // GET: Sandwich/AddIngredients
        public ActionResult AddIngredients(int id)
        {
            ViewBag.ID = id;
            return View(db.Ingredients.ToList());
        }

        // POST: Sandwich/AddIngredients
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddIngredients(int[] ingredient, string sandwichId)
        {
            if (ModelState.IsValid)
            {
                string conString = ConfigurationManager.ConnectionStrings["InventoryContext"].ConnectionString.ToString();
                SqlConnection conn = null;
                conn = new SqlConnection(conString);
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@var1", sandwichId);
                    cmd.CommandText = "DELETE FROM SandwichIngredient WHERE Sandwich_ID=@var1";

                    //cmd.Parameters.AddWithValue("@var2", ingredient[i]);
                    int rowsAffected = cmd.ExecuteNonQuery();
                }
                for (int i = 0; i < ingredient.Count(); i++)
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO SandwichIngredient(Sandwich_ID,Ingredient_ID) Values (@var1, @var2)";
                        cmd.Parameters.AddWithValue("@var1", sandwichId);
                        cmd.Parameters.AddWithValue("@var2", ingredient[i]);
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }
            return View();
        }

        public double GetCost(int id)
        {
            double output = 0.0f;
            Sandwich sandwich = db.Sandwiches.Find(id);
            foreach (Ingredient ing in sandwich.Ingredients)
            {
                output += ing.Price / ing.NumPortions;
            }
            return output;
        }

        // GET: Sandwich/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sandwich sandwich = db.Sandwiches.Find(id);
            if (sandwich == null)
            {
                return HttpNotFound();
            }
            return View(sandwich);
        }

        // POST: Sandwich/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Price,Meal,Ingredients")] Sandwich sandwich)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sandwich).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AddIngredients", new { id = sandwich.ID });
            }
            return View(sandwich);
        }

        // GET: Sandwich/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sandwich sandwich = db.Sandwiches.Find(id);
            if (sandwich == null)
            {
                return HttpNotFound();
            }
            return View(sandwich);
        }

        // POST: Sandwich/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sandwich sandwich = db.Sandwiches.Find(id);
            db.Sandwiches.Remove(sandwich);
            db.SaveChanges();
            string conString = ConfigurationManager.ConnectionStrings["InventoryContext"].ConnectionString.ToString();
            SqlConnection conn = null;
            conn = new SqlConnection(conString);
            conn.Open();
            //int sandwichId = id;

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@var1", id);
                cmd.CommandText = "DELETE FROM SandwichIngredient WHERE Sandwich_ID=@var1";

                //cmd.Parameters.AddWithValue("@var2", ingredient[i]);
                int rowsAffected = cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}