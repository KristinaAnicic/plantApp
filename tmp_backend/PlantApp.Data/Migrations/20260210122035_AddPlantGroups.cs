using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs");

            migrationBuilder.AddColumn<int>(
                name: "plant_group_id",
                table: "planteds",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "planted_id",
                table: "growth_logs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "plant_group_id",
                table: "growth_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "place_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planted_id = table.Column<int>(type: "integer", nullable: false),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_place_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_place_history_places_place_id",
                        column: x => x.place_id,
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_place_history_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_plant_groups_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_planteds_plant_group_id",
                table: "planteds",
                column: "plant_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_growth_logs_plant_group_id",
                table: "growth_logs",
                column: "plant_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_place_history_place_id",
                table: "place_history",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "IX_place_history_planted_id",
                table: "place_history",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_groups_user_id",
                table: "plant_groups",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_growth_logs_plant_groups_plant_group_id",
                table: "growth_logs",
                column: "plant_group_id",
                principalTable: "plant_groups",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs",
                column: "planted_id",
                principalTable: "planteds",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_planteds_plant_groups_plant_group_id",
                table: "planteds",
                column: "plant_group_id",
                principalTable: "plant_groups",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_growth_logs_plant_groups_plant_group_id",
                table: "growth_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs");

            migrationBuilder.DropForeignKey(
                name: "FK_planteds_plant_groups_plant_group_id",
                table: "planteds");

            migrationBuilder.DropTable(
                name: "place_history");

            migrationBuilder.DropTable(
                name: "plant_groups");

            migrationBuilder.DropIndex(
                name: "IX_planteds_plant_group_id",
                table: "planteds");

            migrationBuilder.DropIndex(
                name: "IX_growth_logs_plant_group_id",
                table: "growth_logs");

            migrationBuilder.DropColumn(
                name: "plant_group_id",
                table: "planteds");

            migrationBuilder.DropColumn(
                name: "plant_group_id",
                table: "growth_logs");

            migrationBuilder.AlterColumn<int>(
                name: "planted_id",
                table: "growth_logs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs",
                column: "planted_id",
                principalTable: "planteds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
