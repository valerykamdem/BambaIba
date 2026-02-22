using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMediaStatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comment_count",
                table: "media_assets");

            migrationBuilder.DropColumn(
                name: "dislike_count",
                table: "media_assets");

            migrationBuilder.DropColumn(
                name: "like_count",
                table: "media_assets");

            migrationBuilder.DropColumn(
                name: "play_count",
                table: "media_assets");

            migrationBuilder.CreateTable(
                name: "media_stats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    media_id = table.Column<Guid>(type: "uuid", nullable: false),
                    like_count = table.Column<int>(type: "integer", nullable: false),
                    dislike_count = table.Column<int>(type: "integer", nullable: false),
                    play_count = table.Column<int>(type: "integer", nullable: false),
                    comment_count = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_stats", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_stats_media_assets_media_id",
                        column: x => x.media_id,
                        principalTable: "media_assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_media_stats_media_id",
                table: "media_stats",
                column: "media_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media_stats");

            migrationBuilder.AddColumn<int>(
                name: "comment_count",
                table: "media_assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "dislike_count",
                table: "media_assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "like_count",
                table: "media_assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "play_count",
                table: "media_assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
