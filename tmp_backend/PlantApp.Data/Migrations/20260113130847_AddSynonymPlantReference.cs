using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSynonymPlantReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_plants_plants_synonym_parent_plant_id",
                table: "plants");

            migrationBuilder.AddForeignKey(
                name: "FK_plants_plants_synonym_parent_plant_id",
                table: "plants",
                column: "synonym_parent_plant_id",
                principalTable: "plants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_plants_plants_synonym_parent_plant_id",
                table: "plants");

            migrationBuilder.AddForeignKey(
                name: "FK_plants_plants_synonym_parent_plant_id",
                table: "plants",
                column: "synonym_parent_plant_id",
                principalTable: "plants",
                principalColumn: "id");
        }
    }
}
