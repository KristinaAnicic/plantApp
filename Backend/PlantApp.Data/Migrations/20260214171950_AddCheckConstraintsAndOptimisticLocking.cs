using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintsAndOptimisticLocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "plants",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserRating_Comment_Length",
                table: "user_ratings",
                sql: "char_length(\"comment\") BETWEEN 10 AND 500");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Place_HumidityIntensity_Range",
                table: "places",
                sql: "humidity_intensity BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Place_SunlightIntensity_Range",
                table: "places",
                sql: "sunlight_intensity BETWEEN 1 AND 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UserRating_Comment_Length",
                table: "user_ratings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Place_HumidityIntensity_Range",
                table: "places");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Place_SunlightIntensity_Range",
                table: "places");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "plants");
        }
    }
}
