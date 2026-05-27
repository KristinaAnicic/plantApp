using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSunlightAndHumidityAttributesToPlaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "humidity_intensity",
                table: "places",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "sunlight_intensity",
                table: "places",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "humidity_intensity",
                table: "places");

            migrationBuilder.DropColumn(
                name: "sunlight_intensity",
                table: "places");
        }
    }
}
