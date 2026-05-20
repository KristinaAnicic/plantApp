using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantedPlaceHistoryTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "place_history",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "place_history",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.Sql(@"
                INSERT INTO place_history (planted_id, place_id, start_date, created_at)
                SELECT id AS planted_id,
                       place_id,
                       created_at::timestamp AS start_date,
                       NOW() AS created_at
                FROM planteds
                WHERE place_id IS NOT NULL
                  AND id NOT IN (SELECT planted_id FROM place_history);
            ");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION planted_place_insert_fn()
                RETURNS TRIGGER AS $$
                BEGIN
                    IF NEW.place_id IS NOT NULL THEN
                        INSERT INTO place_history (planted_id, place_id, start_date, created_at)
                        VALUES (NEW.id, NEW.place_id, NEW.created_at::timestamp, NOW());
                    END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER planted_place_insert
                AFTER INSERT ON planteds
                FOR EACH ROW
                EXECUTE FUNCTION planted_place_insert_fn();");

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION planted_place_change_fn()
                RETURNS TRIGGER AS $$
                BEGIN
	                IF NEW.place_id IS DISTINCT FROM OLD.place_id THEN
		                UPDATE place_history
		                SET end_date = NOW(),
			                updated_at = NOW()
		                WHERE planted_id = NEW.id
		                  AND end_date IS NULL;

		                INSERT INTO place_history (planted_id, place_id, start_date, created_at)
		                VALUES (NEW.id, NEW.place_id, NOW(), NOW());

	                END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER planted_place_change
                AFTER UPDATE ON planteds
                FOR EACH ROW
                EXECUTE FUNCTION planted_place_change_fn();");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "start_date",
                table: "place_history",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                table: "place_history",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.Sql(@"
                DROP TRIGGER IF EXISTS planted_place_insert ON planteds;
                DROP FUNCTION IF EXISTS planted_place_insert_fn();
                DROP TRIGGER IF EXISTS planted_place_change ON planteds;
                DROP FUNCTION IF EXISTS planted_place_change_fn();
            ");
        }
    }
}
