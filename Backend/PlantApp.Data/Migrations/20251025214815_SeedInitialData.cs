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
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Full sun", DateTime.UtcNow },
                    { 2, "Partial shade", DateTime.UtcNow },
                    { 3, "Full shade" , DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "aspects",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "East-facing", DateTime.UtcNow },
                    { 2, "North-facing", DateTime.UtcNow },
                    { 3, "South-facing", DateTime.UtcNow },
                    { 4, "West-facing", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "soils",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Loam", DateTime.UtcNow },
                    { 2, "Chalk", DateTime.UtcNow },
                    { 3, "Sand", DateTime.UtcNow },
                    { 4, "Clay", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "moistures",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Well–drained", DateTime.UtcNow },
                    { 2, "Poorly–drained", DateTime.UtcNow },
                    { 3, "Moist but well–drained", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "phs",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Acid", DateTime.UtcNow },
                    { 2, "Alkaline", DateTime.UtcNow },
                    { 3, "Neutral", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "exposures",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Sheltered", DateTime.UtcNow },
                    { 2, "Exposed", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "habits",
                columns: new[] { "id", "name", "created_at" },
                values: new object[,]
                {
                    { 1, "Bushy", DateTime.UtcNow },
                    { 2, "Climbing", DateTime.UtcNow },
                    { 3, "Clump forming", DateTime.UtcNow },
                    { 4, "Columnar upright", DateTime.UtcNow },
                    { 5, "Floating", DateTime.UtcNow },
                    { 6, "Matforming", DateTime.UtcNow },
                    { 7, "Pendulous weeping", DateTime.UtcNow },
                    { 8, "Spreading branched", DateTime.UtcNow },
                    { 9, "Submerged", DateTime.UtcNow },
                    { 10, "Suckering", DateTime.UtcNow },
                    { 11, "Trailing", DateTime.UtcNow },
                    { 12, "Tufted", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "hardiness_levels",
                columns: new[] { "id", "level", "description", "created_at" },
                values: new object[,]
                {
                    { 1, "H1A", "under glass all year (>15C)", DateTime.UtcNow },
                    { 2, "H1B", "can be grown outside in the summer (10 - 15)", DateTime.UtcNow },
                    { 3, "H1C", "can be grown outside in the summer (5 - 10)", DateTime.UtcNow },
                    { 4, "H2" , "tolerant of low temperatures, but not surviving being frozen (1 to 5)", DateTime.UtcNow },
                    { 5, "H3" , "hardy in coastal and relatively mild parts of the UK (-5 to 1)", DateTime.UtcNow },
                    { 6, "H4" , "hardy through most of the UK (-10 to -5)", DateTime.UtcNow },
                    { 7, "H5" , "hardy in most places throughout the UK even in severe winters (-15 to -10)", DateTime.UtcNow },
                    { 8, "H6" , "hardy in all of UK and northern Europe (-20 to -15)", DateTime.UtcNow },
                    { 9, "H7" , "hardy in the severest European continental climates (< -20)", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "spreads",
                columns: new[] { "id", "type", "min_spread", "max_spread", "unit", "created_at" },
                values: new object[,]
                {
                    { 1, "0-0.1 metre", 0m, 0.1m, "m", DateTime.UtcNow },
                    { 2, "0.1-0.5 metres", 0.1m, 0.5m, "m", DateTime.UtcNow },
                    { 3, "0.5–1 metres", 0.5m, 1m, "m", DateTime.UtcNow },
                    { 4, "1–1.5 metres" , 1m, 1.5m, "m", DateTime.UtcNow },
                    { 5, "1.5–2.5 metres" , 1.5m, 2.5m, "m", DateTime.UtcNow },
                    { 6, "2.5–4 metres" , 2.5m, 4m, "m", DateTime.UtcNow },
                    { 7, "4–8 metres" , 4m, 8m, "m", DateTime.UtcNow },
                    { 8, "Wider than 8 metres" , 8m, null, "m", DateTime.UtcNow },
                });


            migrationBuilder.InsertData(
                table: "height_types",
                columns: new[] { "id", "type", "min_height", "max_height", "unit", "created_at" },
                values: new object[,]
                {
                    { 1, "Up to 10cm", 0, 10m, "cm", DateTime.UtcNow },
                    { 2, "0.1-0.5 metres", 0.1m, 0.5m, "m", DateTime.UtcNow },
                    { 3, "0.5–1 metres", 0.5m, 1m, "m", DateTime.UtcNow },
                    { 4, "1–1.5 metres" , 1m, 1.5m, "m", DateTime.UtcNow },
                    { 5, "1.5–2.5 metres" , 1.5m, 2.5m, "m", DateTime.UtcNow },
                    { 6, "4–8 metres" , 4m, 8m, "m", DateTime.UtcNow },
                    { 7, "2.5–4 metres" , 2.5m, 4m, "m", DateTime.UtcNow },
                    { 8, "8–12 metres" , 8m, 12m, "m", DateTime.UtcNow },
                    { 9, "Higher than 12 metres" , 12m, null, "m", DateTime.UtcNow }
                });

            migrationBuilder.InsertData(
                table: "time_to_full_height",
                columns: new[] { "id", "time", "min_time", "max_time", "created_at" },
                values: new object[,]
                {
                    { 1, "1 year", 0, 1, DateTime.UtcNow },
                    { 2, "1–2 years", 1, 2, DateTime.UtcNow },
                    { 3, "2–5 years", 2, 5, DateTime.UtcNow },
                    { 4, "5–10 years" , 5, 10, DateTime.UtcNow },
                    { 5, "10–20 years" , 10, 20, DateTime.UtcNow },
                    { 6, "20–50 years" , 20, 50, DateTime.UtcNow },
                    { 7, "more than 50 years" , 50, null, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
