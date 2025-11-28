using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVideoQualityProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_video_qualities_videos_video_id",
                table: "video_qualities");

            migrationBuilder.RenameColumn(
                name: "video_id",
                table: "video_qualities",
                newName: "media_id");

            migrationBuilder.RenameIndex(
                name: "ix_video_qualities_video_id_quality",
                table: "video_qualities",
                newName: "ix_video_qualities_media_id_quality");

            migrationBuilder.AddForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities",
                column: "media_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_video_qualities_videos_media_id",
                table: "video_qualities");

            migrationBuilder.RenameColumn(
                name: "media_id",
                table: "video_qualities",
                newName: "video_id");

            migrationBuilder.RenameIndex(
                name: "ix_video_qualities_media_id_quality",
                table: "video_qualities",
                newName: "ix_video_qualities_video_id_quality");

            migrationBuilder.AddForeignKey(
                name: "fk_video_qualities_videos_video_id",
                table: "video_qualities",
                column: "video_id",
                principalTable: "media",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
