using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsernameAndModifyUserRatingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_ratings_plant_exchanges_plant_exchange_id",
                table: "user_ratings");

            migrationBuilder.DropIndex(
                name: "IX_user_ratings_plant_exchange_id",
                table: "user_ratings");

            migrationBuilder.DropColumn(
                name: "plant_exchange_id",
                table: "user_ratings");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "username",
                table: "users");

            migrationBuilder.AddColumn<int>(
                name: "plant_exchange_id",
                table: "user_ratings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_user_ratings_plant_exchange_id",
                table: "user_ratings",
                column: "plant_exchange_id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_ratings_plant_exchanges_plant_exchange_id",
                table: "user_ratings",
                column: "plant_exchange_id",
                principalTable: "plant_exchanges",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
