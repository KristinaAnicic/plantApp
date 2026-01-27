using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaceToGrowthLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "place_id",
                table: "growth_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_growth_logs_place_id",
                table: "growth_logs",
                column: "place_id");

            migrationBuilder.AddForeignKey(
                name: "FK_growth_logs_places_place_id",
                table: "growth_logs",
                column: "place_id",
                principalTable: "places",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_growth_logs_places_place_id",
                table: "growth_logs");

            migrationBuilder.DropIndex(
                name: "IX_growth_logs_place_id",
                table: "growth_logs");

            migrationBuilder.DropColumn(
                name: "place_id",
                table: "growth_logs");
        }
    }
}
