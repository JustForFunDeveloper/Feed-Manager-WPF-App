using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HS_Feed_Manager.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TvShows",
                columns: table => new
                {
                    TvShowId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    EpisodeCount = table.Column<int>(nullable: false),
                    AutoDownloadStatus = table.Column<int>(nullable: false),
                    LocalEpisodesCount = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    LatestDownload = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShows", x => x.TvShowId);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    EpisodeNumber = table.Column<double>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    DownloadDate = table.Column<DateTime>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    LocalPath = table.Column<string>(nullable: true),
                    TvShowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "TvShowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes",
                column: "TvShowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "TvShows");
        }
    }
}
