using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAttributesAndAddImageFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_planteds_planted_id",
                table: "images");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "reminders",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "nex_due_date",
                table: "reminders",
                newName: "next_due_date");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "planteds",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "planted_id",
                table: "images",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_images_planted_id",
                table: "images",
                newName: "IX_images_user_id");

            migrationBuilder.AddColumn<string>(
                name: "image",
                table: "planteds",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "image_planted",
                columns: table => new
                {
                    images_id = table.Column<int>(type: "integer", nullable: false),
                    planted_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_planted", x => new { x.images_id, x.planted_id });
                    table.ForeignKey(
                        name: "FK_image_planted_images_images_id",
                        column: x => x.images_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_image_planted_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_image_planted_planted_id",
                table: "image_planted",
                column: "planted_id");

            migrationBuilder.AddForeignKey(
                name: "FK_images_users_user_id",
                table: "images",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_users_user_id",
                table: "images");

            migrationBuilder.DropTable(
                name: "image_planted");

            migrationBuilder.DropColumn(
                name: "image",
                table: "planteds");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "reminders",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "next_due_date",
                table: "reminders",
                newName: "nex_due_date");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "planteds",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "images",
                newName: "planted_id");

            migrationBuilder.RenameIndex(
                name: "IX_images_user_id",
                table: "images",
                newName: "IX_images_planted_id");

            migrationBuilder.AddForeignKey(
                name: "FK_images_planteds_planted_id",
                table: "images",
                column: "planted_id",
                principalTable: "planteds",
                principalColumn: "id");
        }
    }
}
