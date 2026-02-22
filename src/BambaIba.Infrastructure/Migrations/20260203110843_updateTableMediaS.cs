using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTableMediaS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_media_stats_media_id",
                table: "media_stats");

            migrationBuilder.CreateIndex(
                name: "ix_media_stats_media_id",
                table: "media_stats",
                column: "media_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_media_stats_media_id",
                table: "media_stats");

            migrationBuilder.CreateIndex(
                name: "ix_media_stats_media_id",
                table: "media_stats",
                column: "media_id");
        }
    }
}
