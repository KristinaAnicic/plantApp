using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDateOfDeathAttributeToPlanted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_of_death",
                table: "planteds",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_of_death",
                table: "planteds");
        }
    }
}
