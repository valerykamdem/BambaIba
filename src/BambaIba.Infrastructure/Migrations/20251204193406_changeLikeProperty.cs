using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeLikeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_videos_media_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_videos_media_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_transcode_jobs_videos_video_id",
                table: "transcode_jobs");

            migrationBuilder.DropForeignKey(
                name: "fk_views_videos_video_id",
                table: "views");

            migrationBuilder.RenameColumn(
                name: "is_like",
                table: "likes",
                newName: "is_liked");

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
                name: "fk_transcode_jobs_media_video_id",
                table: "transcode_jobs",
                column: "video_id",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_media_media_id",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_media_media_id",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_transcode_jobs_media_video_id",
                table: "transcode_jobs");

            migrationBuilder.DropForeignKey(
                name: "fk_views_media_video_id",
                table: "views");

            migrationBuilder.RenameColumn(
                name: "is_liked",
                table: "likes",
                newName: "is_like");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_videos_media_id",
                table: "comments",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_likes_videos_media_id",
                table: "likes",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transcode_jobs_videos_video_id",
                table: "transcode_jobs",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_views_videos_video_id",
                table: "views",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
