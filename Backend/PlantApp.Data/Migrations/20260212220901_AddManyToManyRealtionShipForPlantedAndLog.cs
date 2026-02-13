using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyRealtionShipForPlantedAndLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "growth_log_planted",
                columns: table => new
                {
                    growth_logs_id = table.Column<int>(type: "integer", nullable: false),
                    planted_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_growth_log_planted", x => new { x.growth_logs_id, x.planted_id });
                    table.ForeignKey(
                        name: "FK_growth_log_planted_growth_logs_growth_logs_id",
                        column: x => x.growth_logs_id,
                        principalTable: "growth_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_growth_log_planted_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_growth_log_planted_planted_id",
                table: "growth_log_planted",
                column: "planted_id");

            migrationBuilder.Sql(@"
                INSERT INTO growth_log_planted (growth_logs_id, planted_id)
                SELECT id AS growth_logs_id, planted_id AS planted_id
                FROM growth_logs
                WHERE planted_id IS NOT NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs");

            migrationBuilder.DropIndex(
                name: "IX_growth_logs_planted_id",
                table: "growth_logs");

            migrationBuilder.DropColumn(
                name: "planted_id",
                table: "growth_logs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "planted_id",
                table: "growth_logs",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE growth_logs g
                SET planted_id = glp.planted_id
                FROM growth_log_planted glp
                WHERE g.id = glp.growth_logs_id
                    AND g.plant_group_id IS NULL
            ");

            migrationBuilder.CreateIndex(
                name: "IX_growth_logs_planted_id",
                table: "growth_logs",
                column: "planted_id");

            migrationBuilder.AddForeignKey(
                name: "FK_growth_logs_planteds_planted_id",
                table: "growth_logs",
                column: "planted_id",
                principalTable: "planteds",
                principalColumn: "id");

            migrationBuilder.DropTable(
                name: "growth_log_planted");
        }
    }
}
