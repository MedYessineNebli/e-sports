using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tournois.Migrations;

/// <inheritdoc />
public partial class _20261116162413_AddTournamentEntities : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Games",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Games", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Teams",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Teams", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tournaments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                GameId = table.Column<int>(type: "int", nullable: false),
                StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tournaments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Tournaments_Games_GameId",
                    column: x => x.GameId,
                    principalTable: "Games",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Players",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Gamertag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                TeamId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Players", x => x.Id);
                table.ForeignKey(
                    name: "FK_Players_Teams_TeamId",
                    column: x => x.TeamId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Matches",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TournamentId = table.Column<int>(type: "int", nullable: false),
                TeamAId = table.Column<int>(type: "int", nullable: false),
                TeamBId = table.Column<int>(type: "int", nullable: false),
                WinnerTeamId = table.Column<int>(type: "int", nullable: true),
                ScoreTeamA = table.Column<int>(type: "int", nullable: false),
                ScoreTeamB = table.Column<int>(type: "int", nullable: false),
                ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Matches", x => x.Id);
                table.ForeignKey(
                    name: "FK_Matches_Teams_TeamAId",
                    column: x => x.TeamAId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Matches_Teams_TeamBId",
                    column: x => x.TeamBId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Matches_Teams_WinnerTeamId",
                    column: x => x.WinnerTeamId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Matches_Tournaments_TournamentId",
                    column: x => x.TournamentId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "TournamentRegistrations",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                TournamentId = table.Column<int>(type: "int", nullable: false),
                TeamId = table.Column<int>(type: "int", nullable: false),
                RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TournamentRegistrations", x => x.Id);
                table.ForeignKey(
                    name: "FK_TournamentRegistrations_Teams_TeamId",
                    column: x => x.TeamId,
                    principalTable: "Teams",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TournamentRegistrations_Tournaments_TournamentId",
                    column: x => x.TournamentId,
                    principalTable: "Tournaments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.InsertData(
            table: "Games",
            columns: new[] { "Id", "Description", "Name" },
            values: new object[,]
            {
                { 1, "MOBA compétitif", "League of Legends" },
                { 2, "FPS tactique", "Counter-Strike 2" },
                { 3, "Football automobile", "Rocket League" }
            });

        migrationBuilder.InsertData(
            table: "Teams",
            columns: new[] { "Id", "Name" },
            values: new object[,]
            {
                { 1, "Atlas Esports" },
                { 2, "Nova Gaming" },
                { 3, "Phoenix Squad" },
                { 4, "Titan United" }
            });

        migrationBuilder.InsertData(
            table: "Players",
            columns: new[] { "Id", "Gamertag", "TeamId" },
            values: new object[,]
            {
                { 1, "AtlasOne", 1 },
                { 2, "AtlasTwo", 1 },
                { 3, "NovaOne", 2 },
                { 4, "NovaTwo", 2 },
                { 5, "PhoenixOne", 3 },
                { 6, "TitanOne", 4 }
            });

        migrationBuilder.InsertData(
            table: "Tournaments",
            columns: new[] { "Id", "EndDate", "GameId", "Name", "StartDate" },
            values: new object[,]
            {
                { 1, new DateTime(2026, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Summer Championship", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                { 2, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Winter Cup", new DateTime(2026, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
            });

        migrationBuilder.InsertData(
            table: "Matches",
            columns: new[] { "Id", "PlayedAt", "ScheduledAt", "ScoreTeamA", "ScoreTeamB", "TeamAId", "TeamBId", "TournamentId", "WinnerTeamId" },
            values: new object[,]
            {
                { 1, new DateTime(2026, 6, 10, 18, 10, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 6, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, 1, 2, 1, 1 },
                { 2, null, new DateTime(2026, 12, 5, 18, 0, 0, 0, DateTimeKind.Unspecified), 0, 0, 2, 4, 2, null }
            });

        migrationBuilder.InsertData(
            table: "TournamentRegistrations",
            columns: new[] { "Id", "RegisteredAt", "TeamId", "TournamentId" },
            values: new object[,]
            {
                { 1, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1 },
                { 2, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                { 3, new DateTime(2026, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1 },
                { 4, new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2 },
                { 5, new DateTime(2026, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2 }
            });

        migrationBuilder.CreateIndex(
            name: "IX_Games_Name",
            table: "Games",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Matches_TeamAId",
            table: "Matches",
            column: "TeamAId");

        migrationBuilder.CreateIndex(
            name: "IX_Matches_TeamBId",
            table: "Matches",
            column: "TeamBId");

        migrationBuilder.CreateIndex(
            name: "IX_Matches_TournamentId",
            table: "Matches",
            column: "TournamentId");

        migrationBuilder.CreateIndex(
            name: "IX_Matches_WinnerTeamId",
            table: "Matches",
            column: "WinnerTeamId");

        migrationBuilder.CreateIndex(
            name: "IX_Players_TeamId",
            table: "Players",
            column: "TeamId");

        migrationBuilder.CreateIndex(
            name: "IX_Teams_Name",
            table: "Teams",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_TournamentRegistrations_TeamId",
            table: "TournamentRegistrations",
            column: "TeamId");

        migrationBuilder.CreateIndex(
            name: "IX_TournamentRegistrations_TournamentId_TeamId",
            table: "TournamentRegistrations",
            columns: new[] { "TournamentId", "TeamId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tournaments_GameId",
            table: "Tournaments",
            column: "GameId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Matches");

        migrationBuilder.DropTable(
            name: "Players");

        migrationBuilder.DropTable(
            name: "TournamentRegistrations");

        migrationBuilder.DropTable(
            name: "Teams");

        migrationBuilder.DropTable(
            name: "Tournaments");

        migrationBuilder.DropTable(
            name: "Games");
    }
}
