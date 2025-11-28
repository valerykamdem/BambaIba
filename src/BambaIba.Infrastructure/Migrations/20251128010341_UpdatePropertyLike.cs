using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_likes_videos_video_id",
                table: "likes");

            migrationBuilder.RenameColumn(
                name: "video_id",
                table: "likes",
                newName: "media_id");

            migrationBuilder.RenameIndex(
                name: "ix_likes_video_id_user_id",
                table: "likes",
                newName: "ix_likes_media_id_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_likes_videos_media_id",
                table: "likes",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_likes_videos_media_id",
                table: "likes");

            migrationBuilder.RenameColumn(
                name: "media_id",
                table: "likes",
                newName: "video_id");

            migrationBuilder.RenameIndex(
                name: "ix_likes_media_id_user_id",
                table: "likes",
                newName: "ix_likes_video_id_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_likes_videos_video_id",
                table: "likes",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
