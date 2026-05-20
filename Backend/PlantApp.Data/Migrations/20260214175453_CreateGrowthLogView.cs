using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateGrowthLogView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW vw_planted_growth_overview AS
                SELECT planted.Id as planted_Id, 
	                place.sunlight_intensity as sunlight_intensity, 
	                place.humidity_intensity as humidity_intensity,
	                planted.is_outside as is_Outside,
	                family.name as family,
	                COALESCE(plant.hardiness_level_id::float, 1.0) as hardiness,
	                gl.plant_status_id as plant_Status_Id,	
	                ARRAY(
                        SELECT s.id
                        FROM plant_sunlight ps
                        JOIN sunlights s on ps.sunlights_id = s.id
                        WHERE ps.plants_id = plant.id
                    )as sunlight_list,
	                ARRAY(
                        SELECT m.id
                        FROM moisture_plant pm
                        JOIN moistures m on pm.moistures_id = m.id
                        WHERE pm.plants_id = plant.id
                    )AS moisture_list,
                    ARRAY(
                        SELECT se.id
                        FROM plant_season pse
                        JOIN seasons se ON pse.seasons_id = se.id
                        WHERE pse.plants_id = plant.id
                    )as seasons,	
	                COALESCE(plant.is_low_maintenance, false)  as low_maintenance,
                    COALESCE(plant.is_drought_resistant, false) as drought_resistant,
	                EXTRACT(MONTH FROM gl.observation_date)::int as month,
	                GREATEST(gl.observation_date - planted.date_planted,0)  as days_since_planted,
	                COALESCE((
                        SELECT AVG(rh.delay)
                        FROM reminder_history rh
                        WHERE rh.planted_id = planted.id
                          AND rh.due_date <= gl.observation_date
                    ), 0.0) as reminder_delay  

                FROM growth_logs gl
                JOIN growth_log_planted glp on glp.growth_logs_id = gl.id
                JOIN planteds planted on glp.planted_id = planted.id
                JOIN plants plant on plant.id = planted.plant_id
                JOIN plant_families family on plant.family_id = family.id
                CROSS JOIN LATERAL (
                    SELECT ph.place_id
                    FROM place_history ph
                    WHERE ph.planted_id = planted.id
                    ORDER BY 
                        CASE 
                            WHEN ph.start_date <= gl.observation_date 
                                 AND (ph.end_date IS NULL OR ph.end_date >= gl.observation_date) THEN 0
                            ELSE 1
                        END,
                        ph.start_date ASC
                    LIMIT 1
                ) ph_latest
                JOIN places place on ph_latest.place_id = place.id
                WHERE gl.deleted_at IS NULL;
                ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_planted_growth_overview;");
        }
    }
}
