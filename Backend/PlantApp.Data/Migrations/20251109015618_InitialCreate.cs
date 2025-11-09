using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlantApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspects",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exchange_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exposures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exposures", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fragrances",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fragrances", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "habits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habits", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hardiness_levels",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    level = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hardiness_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "height_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    min_height = table.Column<decimal>(type: "numeric", nullable: false),
                    max_height = table.Column<decimal>(type: "numeric", nullable: true),
                    unit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_height_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "moistures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moistures", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "phs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plant_families",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_families", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plant_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reasons_of_death",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reasons_of_death", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reminder_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminder_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "soils",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_soils", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "spreads",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    min_spread = table.Column<decimal>(type: "numeric", nullable: false),
                    max_spread = table.Column<decimal>(type: "numeric", nullable: true),
                    unit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_spreads", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sunlights",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sunlights", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "time_to_full_height",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    min_time = table.Column<int>(type: "integer", nullable: false),
                    max_time = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_time_to_full_height", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                    table.ForeignKey(
                        name: "FK_cities_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    contact = table.Column<string>(type: "text", nullable: true),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<char>(type: "character(1)", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plants",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    botanical_name = table.Column<string>(type: "text", nullable: false),
                    common_name = table.Column<string>(type: "text", nullable: false),
                    synonym_parent_plant_id = table.Column<int>(type: "integer", nullable: true),
                    fragrance_id = table.Column<int>(type: "integer", nullable: true),
                    hardiness_level_id = table.Column<int>(type: "integer", nullable: true),
                    is_specie = table.Column<bool>(type: "boolean", nullable: true),
                    is_genus = table.Column<bool>(type: "boolean", nullable: true),
                    is_plant_for_pollinators = table.Column<bool>(type: "boolean", nullable: true),
                    is_low_maintenance = table.Column<bool>(type: "boolean", nullable: true),
                    is_drought_resistant = table.Column<bool>(type: "boolean", nullable: true),
                    spread_type_id = table.Column<int>(type: "integer", nullable: true),
                    height_type_id = table.Column<int>(type: "integer", nullable: true),
                    time_to_full_height_id = table.Column<int>(type: "integer", nullable: false),
                    toxicity = table.Column<string>(type: "text", nullable: true),
                    cultivation = table.Column<string>(type: "text", nullable: true),
                    pest_resistance = table.Column<string>(type: "text", nullable: true),
                    disease_resistance = table.Column<string>(type: "text", nullable: true),
                    pruning = table.Column<string>(type: "text", nullable: true),
                    propagation = table.Column<string>(type: "text", nullable: true),
                    family_id = table.Column<int>(type: "integer", nullable: true),
                    entity_description = table.Column<string>(type: "text", nullable: true),
                    genus_description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plants", x => x.id);
                    table.ForeignKey(
                        name: "FK_plants_fragrances_fragrance_id",
                        column: x => x.fragrance_id,
                        principalTable: "fragrances",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_hardiness_levels_hardiness_level_id",
                        column: x => x.hardiness_level_id,
                        principalTable: "hardiness_levels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_height_types_height_type_id",
                        column: x => x.height_type_id,
                        principalTable: "height_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_plant_families_family_id",
                        column: x => x.family_id,
                        principalTable: "plant_families",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_plants_synonym_parent_plant_id",
                        column: x => x.synonym_parent_plant_id,
                        principalTable: "plants",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_spreads_spread_type_id",
                        column: x => x.spread_type_id,
                        principalTable: "spreads",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plants_time_to_full_height_time_to_full_height_id",
                        column: x => x.time_to_full_height_id,
                        principalTable: "time_to_full_height",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "places",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_places", x => x.id);
                    table.ForeignKey(
                        name: "FK_places_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_places_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_places_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspect_plant",
                columns: table => new
                {
                    aspects_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspect_plant", x => new { x.aspects_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_aspect_plant_aspects_aspects_id",
                        column: x => x.aspects_id,
                        principalTable: "aspects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aspect_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "exposure_plant",
                columns: table => new
                {
                    exposures_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exposure_plant", x => new { x.exposures_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_exposure_plant_exposures_exposures_id",
                        column: x => x.exposures_id,
                        principalTable: "exposures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exposure_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "habit_plant",
                columns: table => new
                {
                    habits_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_habit_plant", x => new { x.habits_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_habit_plant_habits_habits_id",
                        column: x => x.habits_id,
                        principalTable: "habits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_habit_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "moisture_plant",
                columns: table => new
                {
                    moistures_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moisture_plant", x => new { x.moistures_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_moisture_plant_moistures_moistures_id",
                        column: x => x.moistures_id,
                        principalTable: "moistures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_moisture_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ph_plant",
                columns: table => new
                {
                    phs_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ph_plant", x => new { x.phs_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_ph_plant_phs_phs_id",
                        column: x => x.phs_id,
                        principalTable: "phs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ph_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_season",
                columns: table => new
                {
                    plants_id = table.Column<int>(type: "integer", nullable: false),
                    seasons_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_season", x => new { x.plants_id, x.seasons_id });
                    table.ForeignKey(
                        name: "FK_plant_season_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_season_seasons_seasons_id",
                        column: x => x.seasons_id,
                        principalTable: "seasons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_soil_type",
                columns: table => new
                {
                    plants_id = table.Column<int>(type: "integer", nullable: false),
                    soil_types_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_soil_type", x => new { x.plants_id, x.soil_types_id });
                    table.ForeignKey(
                        name: "FK_plant_soil_type_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_soil_type_soils_soil_types_id",
                        column: x => x.soil_types_id,
                        principalTable: "soils",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plant_sunlight",
                columns: table => new
                {
                    plants_id = table.Column<int>(type: "integer", nullable: false),
                    sunlights_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_sunlight", x => new { x.plants_id, x.sunlights_id });
                    table.ForeignKey(
                        name: "FK_plant_sunlight_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_sunlight_sunlights_sunlights_id",
                        column: x => x.sunlights_id,
                        principalTable: "sunlights",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "planteds",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    plant_id = table.Column<int>(type: "integer", nullable: false),
                    date_planted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    is_outside = table.Column<bool>(type: "boolean", nullable: false),
                    plant_status_id = table.Column<int>(type: "integer", nullable: false),
                    reason_of_death_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_planteds", x => x.id);
                    table.ForeignKey(
                        name: "FK_planteds_places_place_id",
                        column: x => x.place_id,
                        principalTable: "places",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planteds_plant_statuses_plant_status_id",
                        column: x => x.plant_status_id,
                        principalTable: "plant_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planteds_plants_plant_id",
                        column: x => x.plant_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_planteds_reasons_of_death_reason_of_death_id",
                        column: x => x.reason_of_death_id,
                        principalTable: "reasons_of_death",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "growth_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planted_id = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    plant_status_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_growth_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_growth_logs_plant_statuses_plant_status_id",
                        column: x => x.plant_status_id,
                        principalTable: "plant_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_growth_logs_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    copyright = table.Column<string>(type: "text", nullable: true),
                    planted_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_images_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "plant_exchanges",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    planted_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    plant_status = table.Column<string>(type: "text", nullable: false),
                    contact = table.Column<string>(type: "text", nullable: false),
                    main_image = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    exchange_type_id = table.Column<int>(type: "integer", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    exchange_for = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: true),
                    shipping = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plant_exchanges", x => x.id);
                    table.ForeignKey(
                        name: "FK_plant_exchanges_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_exchanges_exchange_types_exchange_type_id",
                        column: x => x.exchange_type_id,
                        principalTable: "exchange_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plant_exchanges_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_plant_exchanges_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reminders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    planted_id = table.Column<int>(type: "integer", nullable: false),
                    reminder_type_id = table.Column<int>(type: "integer", nullable: false),
                    frequency = table.Column<string>(type: "text", nullable: false),
                    nex_due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_reminders_planteds_planted_id",
                        column: x => x.planted_id,
                        principalTable: "planteds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reminders_reminder_types_reminder_type_id",
                        column: x => x.reminder_type_id,
                        principalTable: "reminder_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "growth_log_image",
                columns: table => new
                {
                    growth_logs_id = table.Column<int>(type: "integer", nullable: false),
                    images_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_growth_log_image", x => new { x.growth_logs_id, x.images_id });
                    table.ForeignKey(
                        name: "FK_growth_log_image_growth_logs_growth_logs_id",
                        column: x => x.growth_logs_id,
                        principalTable: "growth_logs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_growth_log_image_images_images_id",
                        column: x => x.images_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "image_plant",
                columns: table => new
                {
                    images_id = table.Column<int>(type: "integer", nullable: false),
                    plants_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_plant", x => new { x.images_id, x.plants_id });
                    table.ForeignKey(
                        name: "FK_image_plant_images_images_id",
                        column: x => x.images_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_image_plant_plants_plants_id",
                        column: x => x.plants_id,
                        principalTable: "plants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "image_plant_exchange",
                columns: table => new
                {
                    images_id = table.Column<int>(type: "integer", nullable: false),
                    plant_exchanges_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_plant_exchange", x => new { x.images_id, x.plant_exchanges_id });
                    table.ForeignKey(
                        name: "FK_image_plant_exchange_images_images_id",
                        column: x => x.images_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_image_plant_exchange_plant_exchanges_plant_exchanges_id",
                        column: x => x.plant_exchanges_id,
                        principalTable: "plant_exchanges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_ratings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    plant_exchange_id = table.Column<int>(type: "integer", nullable: false),
                    rater_id = table.Column<int>(type: "integer", nullable: false),
                    rated_id = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_ratings", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_ratings_plant_exchanges_plant_exchange_id",
                        column: x => x.plant_exchange_id,
                        principalTable: "plant_exchanges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_ratings_users_rated_id",
                        column: x => x.rated_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_ratings_users_rater_id",
                        column: x => x.rater_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aspect_plant_plants_id",
                table: "aspect_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_cities_country_id",
                table: "cities",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_exposure_plant_plants_id",
                table: "exposure_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_growth_log_image_images_id",
                table: "growth_log_image",
                column: "images_id");

            migrationBuilder.CreateIndex(
                name: "IX_growth_logs_plant_status_id",
                table: "growth_logs",
                column: "plant_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_growth_logs_planted_id",
                table: "growth_logs",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_habit_plant_plants_id",
                table: "habit_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_plant_plants_id",
                table: "image_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_image_plant_exchange_plant_exchanges_id",
                table: "image_plant_exchange",
                column: "plant_exchanges_id");

            migrationBuilder.CreateIndex(
                name: "IX_images_planted_id",
                table: "images",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_moisture_plant_plants_id",
                table: "moisture_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_ph_plant_plants_id",
                table: "ph_plant",
                column: "plants_id");

            migrationBuilder.CreateIndex(
                name: "IX_places_city_id",
                table: "places",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_places_country_id",
                table: "places",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_places_user_id",
                table: "places",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_exchanges_country_id",
                table: "plant_exchanges",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_exchanges_exchange_type_id",
                table: "plant_exchanges",
                column: "exchange_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_exchanges_planted_id",
                table: "plant_exchanges",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_exchanges_user_id",
                table: "plant_exchanges",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_season_seasons_id",
                table: "plant_season",
                column: "seasons_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_soil_type_soil_types_id",
                table: "plant_soil_type",
                column: "soil_types_id");

            migrationBuilder.CreateIndex(
                name: "IX_plant_sunlight_sunlights_id",
                table: "plant_sunlight",
                column: "sunlights_id");

            migrationBuilder.CreateIndex(
                name: "IX_planteds_place_id",
                table: "planteds",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "IX_planteds_plant_id",
                table: "planteds",
                column: "plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_planteds_plant_status_id",
                table: "planteds",
                column: "plant_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_planteds_reason_of_death_id",
                table: "planteds",
                column: "reason_of_death_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_family_id",
                table: "plants",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_fragrance_id",
                table: "plants",
                column: "fragrance_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_hardiness_level_id",
                table: "plants",
                column: "hardiness_level_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_height_type_id",
                table: "plants",
                column: "height_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_spread_type_id",
                table: "plants",
                column: "spread_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_synonym_parent_plant_id",
                table: "plants",
                column: "synonym_parent_plant_id");

            migrationBuilder.CreateIndex(
                name: "IX_plants_time_to_full_height_id",
                table: "plants",
                column: "time_to_full_height_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminders_planted_id",
                table: "reminders",
                column: "planted_id");

            migrationBuilder.CreateIndex(
                name: "IX_reminders_reminder_type_id",
                table: "reminders",
                column: "reminder_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ratings_plant_exchange_id",
                table: "user_ratings",
                column: "plant_exchange_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ratings_rated_id",
                table: "user_ratings",
                column: "rated_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_ratings_rater_id",
                table: "user_ratings",
                column: "rater_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspect_plant");

            migrationBuilder.DropTable(
                name: "exposure_plant");

            migrationBuilder.DropTable(
                name: "growth_log_image");

            migrationBuilder.DropTable(
                name: "habit_plant");

            migrationBuilder.DropTable(
                name: "image_plant");

            migrationBuilder.DropTable(
                name: "image_plant_exchange");

            migrationBuilder.DropTable(
                name: "moisture_plant");

            migrationBuilder.DropTable(
                name: "ph_plant");

            migrationBuilder.DropTable(
                name: "plant_season");

            migrationBuilder.DropTable(
                name: "plant_soil_type");

            migrationBuilder.DropTable(
                name: "plant_sunlight");

            migrationBuilder.DropTable(
                name: "reminders");

            migrationBuilder.DropTable(
                name: "user_ratings");

            migrationBuilder.DropTable(
                name: "aspects");

            migrationBuilder.DropTable(
                name: "exposures");

            migrationBuilder.DropTable(
                name: "growth_logs");

            migrationBuilder.DropTable(
                name: "habits");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "moistures");

            migrationBuilder.DropTable(
                name: "phs");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropTable(
                name: "soils");

            migrationBuilder.DropTable(
                name: "sunlights");

            migrationBuilder.DropTable(
                name: "reminder_types");

            migrationBuilder.DropTable(
                name: "plant_exchanges");

            migrationBuilder.DropTable(
                name: "exchange_types");

            migrationBuilder.DropTable(
                name: "planteds");

            migrationBuilder.DropTable(
                name: "places");

            migrationBuilder.DropTable(
                name: "plant_statuses");

            migrationBuilder.DropTable(
                name: "plants");

            migrationBuilder.DropTable(
                name: "reasons_of_death");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "fragrances");

            migrationBuilder.DropTable(
                name: "hardiness_levels");

            migrationBuilder.DropTable(
                name: "height_types");

            migrationBuilder.DropTable(
                name: "plant_families");

            migrationBuilder.DropTable(
                name: "spreads");

            migrationBuilder.DropTable(
                name: "time_to_full_height");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
