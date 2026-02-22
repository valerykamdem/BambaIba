using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MediaAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_media_media_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_media_media_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_playlist_items_media_media_id",
                table: "playlist_items");

            migrationBuilder.DropForeignKey(
                name: "fk_transcode_jobs_media_video_id",
                table: "transcode_jobs");

            migrationBuilder.DropForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities");

            migrationBuilder.DropForeignKey(
                name: "fk_views_media_video_id",
                table: "views");

            migrationBuilder.RenameTable(
                name: "media",
                newName: "media_assets");

            migrationBuilder.AlterColumn<string>(
                name: "discriminator",
                table: "media_assets",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5)",
                oldMaxLength: 5);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_media_assets_media_id",
                table: "comments",
                column: "media_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_likes_media_assets_media_id",
                table: "likes",
                column: "media_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_playlist_items_media_assets_media_id",
                table: "playlist_items",
                column: "media_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transcode_jobs_media_assets_video_id",
                table: "transcode_jobs",
                column: "video_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities",
                column: "media_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_views_media_assets_video_id",
                table: "views",
                column: "video_id",
                principalTable: "media_assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_media_assets_media_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_media_assets_media_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_playlist_items_media_assets_media_id",
                table: "playlist_items");

            migrationBuilder.DropForeignKey(
                name: "fk_transcode_jobs_media_assets_video_id",
                table: "transcode_jobs");

            migrationBuilder.DropForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities");

            migrationBuilder.DropForeignKey(
                name: "fk_views_media_assets_video_id",
                table: "views");

            migrationBuilder.DropTable(
                name: "media_assets");

            migrationBuilder.CreateTable(
                name: "media",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    comment_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    deleted_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    discriminator = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    dislike_count = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    language = table.Column<string>(type: "text", nullable: false),
                    like_count = table.Column<int>(type: "integer", nullable: false),
                    play_count = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    speaker = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    storage_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tags = table.Column<string>(type: "text", nullable: false),
                    thumbnail_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    topic = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_comments_media_media_id",
                table: "comments",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_likes_media_media_id",
                table: "likes",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_playlist_items_media_media_id",
                table: "playlist_items",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transcode_jobs_media_video_id",
                table: "transcode_jobs",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_views_media_video_id",
                table: "views",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
