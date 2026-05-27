using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "sunlights",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Full sun" },
                    { 2, "Partial shade" },
                    { 3, "Full shade" }
                });

            migrationBuilder.InsertData(
                table: "aspects",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "East-facing" },
                    { 2, "North-facing" },
                    { 3, "South-facing" },
                    { 4, "West-facing" }
                });

            migrationBuilder.InsertData(
                table: "soils",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Loam" },
                    { 2, "Chalk" },
                    { 3, "Sand" },
                    { 4, "Clay" }
                });

            migrationBuilder.InsertData(
                table: "moistures",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Well–drained" },
                    { 2, "Poorly–drained" },
                    { 3, "Moist but well–drained" }
                });

            migrationBuilder.InsertData(
                table: "phs",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Acid" },
                    { 2, "Alkaline" },
                    { 3, "Neutral" }
                });

            migrationBuilder.InsertData(
                table: "exposures",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Sheltered" },
                    { 2, "Exposed" }
                });

            migrationBuilder.InsertData(
                table: "habits",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Bushy" },
                    { 2, "Climbing" },
                    { 3, "Clump forming" },
                    { 4, "Columnar upright" },
                    { 5, "Floating" },
                    { 6, "Matforming" },
                    { 7, "Pendulous weeping" },
                    { 8, "Spreading branched" },
                    { 9, "Submerged" },
                    { 10, "Suckering" },
                    { 11, "Trailing" },
                    { 12, "Tufted" }
                });

            migrationBuilder.InsertData(
                table: "hardiness_levels",
                columns: new[] { "id", "level", "description" },
                values: new object[,]
                {
                    { 1, "H1A", "under glass all year (>15C)" },
                    { 2, "H1B", "can be grown outside in the summer (10 - 15)" },
                    { 3, "H1C", "can be grown outside in the summer (5 - 10)" },
                    { 4, "H2" , "tolerant of low temperatures, but not surviving being frozen (1 to 5)" },
                    { 5, "H3" , "hardy in coastal and relatively mild parts of the UK (-5 to 1)" },
                    { 6, "H4" , "hardy through most of the UK (-10 to -5)" },
                    { 7, "H5" , "hardy in most places throughout the UK even in severe winters (-15 to -10)" },
                    { 8, "H6" , "hardy in all of UK and northern Europe (-20 to -15)" },
                    { 9, "H7" , "hardy in the severest European continental climates (< -20)" }
                });

            migrationBuilder.InsertData(
                table: "spreads",
                columns: new[] { "id", "name", "min_spread", "max_spread", "unit" },
                values: new object[,]
                {
                    { 1, "0-0.1 metre", 0m, 0.1m, "m" },
                    { 2, "0.1-0.5 metres", 0.1m, 0.5m, "m" },
                    { 3, "0.5–1 metres", 0.5m, 1m, "m" },
                    { 4, "1–1.5 metres" , 1m, 1.5m, "m" },
                    { 5, "1.5–2.5 metres" , 1.5m, 2.5m, "m" },
                    { 6, "2.5–4 metres" , 2.5m, 4m, "m" },
                    { 7, "4–8 metres" , 4m, 8m, "m" },
                    { 8, "Wider than 8 metres" , 8m, null, "m" },
                });


            migrationBuilder.InsertData(
                table: "height_types",
                columns: new[] { "id", "name", "min_height", "max_height", "unit" },
                values: new object[,]
                {
                    { 1, "Up to 10cm", 0, 0.1m, "m" },
                    { 2, "0.1-0.5 metres", 0.1m, 0.5m, "m" },
                    { 3, "0.5–1 metres", 0.5m, 1m, "m" },
                    { 4, "1–1.5 metres" , 1m, 1.5m, "m" },
                    { 5, "1.5–2.5 metres" , 1.5m, 2.5m, "m" },
                    { 6, "4–8 metres" , 4m, 8m, "m" },
                    { 7, "2.5–4 metres" , 2.5m, 4m, "m" },
                    { 8, "8–12 metres" , 8m, 12m, "m" },
                    { 9, "Higher than 12 metres" , 12m, null, "m" }
                });

            migrationBuilder.InsertData(
                table: "time_to_full_height",
                columns: new[] { "id", "name", "min_time", "max_time" },
                values: new object[,]
                {
                    { 1, "1 year", 0, 1 },
                    { 2, "1–2 years", 1, 2 },
                    { 3, "2–5 years", 2, 5 },
                    { 4, "5–10 years" , 5, 10 },
                    { 5, "10–20 years" , 10, 20 },
                    { 6, "20–50 years" , 20, 50 },
                    { 7, "more than 50 years" , 50, null }
                });

            migrationBuilder.InsertData(
                table: "plant_statuses",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Healthy" },
                    { 2, "Sick" },
                    { 3, "Dead" },
                    { 4, "Wilting" },
                    { 5, "Growing" },
                    { 6, "Flowering" },
                    { 7, "Fruiting" },
                    { 8, "Seedling" },
                    { 9, "Harvested" },
                    { 10, "Stressed" },
                    { 11, "Transplanted" },
                    { 12, "Dormant" }
                });

            migrationBuilder.InsertData(
                table: "exchange_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Sell" },
                    { 2, "Swap" },
                    { 3, "Free" }
                });

            migrationBuilder.InsertData(
                table: "reasons_of_death",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Unknown" },
                    { 2, "Overwatering" },
                    { 3, "Underwatering" },
                    { 4, "Poor drainage" },
                    { 5, "Lack of light" },
                    { 6, "Too much light" },
                    { 7, "Cold damage" },
                    { 8, "Heat stress" },
                    { 9, "Pests" },
                    { 10, "Fungal infection" },
                    { 11, "Bacterial infection" },
                    { 12, "Viral infection" },
                    { 13, "Nutrient deficiency" },
                    { 14, "Over fertilization" },
                    { 15, "Soil pH imbalance" },
                    { 16, "Transplant shock" },
                    { 17, "Environmental change" },
                    { 18, "Neglect" },
                    { 19, "Mechanical damage" }
                });

            migrationBuilder.InsertData(
                table: "reminder_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Watering" },
                    { 2, "Fertilizing" },
                    { 3, "Pruning" },
                    { 4, "Repotting" },
                    { 5, "Pest control" },
                    { 6, "Harvesting" },
                    { 7, "Check growth" },
                    { 8, "Flower care" },
                    { 9, "Dormancy preparation" },
                    { 10, "Other" }
                });

            migrationBuilder.InsertData(
                table: "seasons",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Spring" },
                    { 2, "Summer" },
                    { 3, "Autumn" },
                    { 4, "Winter" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Moderator" },
                    { 3, "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
