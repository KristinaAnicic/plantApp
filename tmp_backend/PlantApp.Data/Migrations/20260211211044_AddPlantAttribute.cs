using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "plant_attribute_type_id",
                table: "plants",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "plant_attribute_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_attribute_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plant_season_attributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_id = table.Column<int>(type: "integer", nullable: false),
                    season_id = table.Column<int>(type: "integer", nullable: false),
                    plant_attribute_type_id = table.Column<int>(type: "integer", nullable: false),
                    colour = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_season_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_plant_season_attributes_plant_attribute_types_plant_attribu~",
                        column: x => x.plant_attribute_type_id,
                        principalTable: "plant_attribute_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_season_attributes_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_season_attributes_seasons_season_id",
                        column: x => x.season_id,
                        principalTable: "seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_plants_plant_attribute_type_id",
                table: "plants",
                column: "plant_attribute_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_season_attributes_plant_attribute_type_id",
                table: "plant_season_attributes",
                column: "plant_attribute_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_season_attributes_plant_id",
                table: "plant_season_attributes",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_season_attributes_season_id",
                table: "plant_season_attributes",
                column: "season_id");

            migrationBuilder.AddForeignKey(
                name: "FK_plants_plant_attribute_types_plant_attribute_type_id",
                table: "plants",
                column: "plant_attribute_type_id",
                principalTable: "plant_attribute_types",
                principalColumn: "id");

            migrationBuilder.InsertData(
                table: "plant_attribute_types",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Foliage" },
                    { 2, "Stem" },
                    { 3, "Fruit" },
                    { 4, "Flower" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_plants_plant_attribute_types_plant_attribute_type_id",
                table: "plants");

            migrationBuilder.DropTable(
                name: "plant_season_attributes");

            migrationBuilder.DropTable(
                name: "plant_attribute_types");

            migrationBuilder.DropIndex(
                name: "IX_plants_plant_attribute_type_id",
                table: "plants");

            migrationBuilder.DropColumn(
                name: "plant_attribute_type_id",
                table: "plants");
        }
    }
}
