namespace InventoryDash.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ingredient",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Price = c.Double(nullable: false),
                    NumPortions = c.Int(nullable: false),
                    Category = c.Int(nullable: false),
                    UsedInSandwich = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Sandwich",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    Price = c.Double(),
                    Meal = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.Order",
                c => new
                {
                    MyID = c.Int(nullable: false, identity: true),
                    IngredientID = c.Int(nullable: false),
                    Cost = c.Double(nullable: false),
                    Portions = c.Double(nullable: false),
                    PortionsRemaining = c.Double(nullable: false),
                    Date = c.DateTime(nullable: false),
                    ExpirationDate = c.DateTime(nullable: false),
                    Notes = c.String(),
                })
                .PrimaryKey(t => t.MyID);

            CreateTable(
                "dbo.WeeklyInventoryDrinks",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WeekId = c.Int(nullable: false),
                    Year = c.Int(nullable: false),
                    DrinkId = c.Int(nullable: false),
                    QuantityToGo = c.Int(nullable: false),
                    QuantityDineIn = c.Int(nullable: false),
                    Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Income = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.WeeklyInventorySandwiches",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    WeekId = c.Int(nullable: false),
                    Year = c.Int(nullable: false),
                    SandwichId = c.Int(nullable: false),
                    QuantityToGo = c.Int(nullable: false),
                    QuantityDineIn = c.Int(nullable: false),
                    MealId = c.Int(),
                    Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                    Income = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.ID);

            CreateTable(
                "dbo.SandwichIngredient",
                c => new
                {
                    Sandwich_ID = c.Int(nullable: false),
                    Ingredient_ID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Sandwich_ID, t.Ingredient_ID })
                .ForeignKey("dbo.Sandwich", t => t.Sandwich_ID, cascadeDelete: true)
                .ForeignKey("dbo.Ingredient", t => t.Ingredient_ID, cascadeDelete: true)
                .Index(t => t.Sandwich_ID)
                .Index(t => t.Ingredient_ID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.SandwichIngredient", "Ingredient_ID", "dbo.Ingredient");
            DropForeignKey("dbo.SandwichIngredient", "Sandwich_ID", "dbo.Sandwich");
            DropIndex("dbo.SandwichIngredient", new[] { "Ingredient_ID" });
            DropIndex("dbo.SandwichIngredient", new[] { "Sandwich_ID" });
            DropTable("dbo.SandwichIngredient");
            DropTable("dbo.WeeklyInventorySandwiches");
            DropTable("dbo.WeeklyInventoryDrinks");
            DropTable("dbo.Order");
            DropTable("dbo.Sandwich");
            DropTable("dbo.Ingredient");
        }
    }
}