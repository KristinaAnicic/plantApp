using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalDueDateAndRemoveDelayFromReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delay_days",
                table: "reminders");

            migrationBuilder.AddColumn<DateTime>(
                name: "original_due_date",
                table: "reminders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "original_due_date",
                table: "reminders");

            migrationBuilder.AddColumn<int>(
                name: "delay_days",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
