using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateReminderHistoryAndFrequencyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "frequency",
                table: "reminders");

            migrationBuilder.AddColumn<int>(
                name: "delay_days",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "frequency_num",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "frequency_type_id",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "planteds",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "frequencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_frequencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reminder_history",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planted_id = table.Column<int>(type: "integer", nullable: true),
                    reminder_type_id = table.Column<int>(type: "integer", nullable: false),
                    frequency_type_id = table.Column<int>(type: "integer", nullable: false),
                    frequency_num = table.Column<int>(type: "integer", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_done = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delay = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminder_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminder_history_frequencies_frequency_type_id",
                        column: x => x.frequency_type_id,
                        principalTable: "frequencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reminder_history_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_reminder_history_reminder_types_reminder_type_id",
                        column: x => x.reminder_type_id,
                        principalTable: "reminder_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reminders_frequency_type_id",
                table: "reminders",
                column: "frequency_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_history_frequency_type_id",
                table: "reminder_history",
                column: "frequency_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_history_planted_id",
                table: "reminder_history",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminder_history_reminder_type_id",
                table: "reminder_history",
                column: "reminder_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_reminders_frequencies_frequency_type_id",
                table: "reminders",
                column: "frequency_type_id",
                principalTable: "frequencies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(
                table: "frequencies",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Day" },
                    { 2, "Week" },
                    { 3, "Month" },
                    { 4, "Year" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reminders_frequencies_frequency_type_id",
                table: "reminders");

            migrationBuilder.DropTable(
                name: "reminder_history");

            migrationBuilder.DropTable(
                name: "frequencies");

            migrationBuilder.DropIndex(
                name: "IX_reminders_frequency_type_id",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "delay_days",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "frequency_num",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "frequency_type_id",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "name",
                table: "planteds");

            migrationBuilder.AddColumn<string>(
                name: "frequency",
                table: "reminders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
