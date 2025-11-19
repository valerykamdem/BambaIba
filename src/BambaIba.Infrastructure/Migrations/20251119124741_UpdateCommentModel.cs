using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_videos_video_id",
                table: "comments");

            migrationBuilder.RenameColumn(
                name: "video_id",
                table: "comments",
                newName: "media_id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_video_id",
                table: "comments",
                newName: "ix_comments_media_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_videos_media_id",
                table: "comments",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comments_videos_media_id",
                table: "comments");

            migrationBuilder.RenameColumn(
                name: "media_id",
                table: "comments",
                newName: "video_id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_media_id",
                table: "comments",
                newName: "ix_comments_video_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comments_videos_video_id",
                table: "comments",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
