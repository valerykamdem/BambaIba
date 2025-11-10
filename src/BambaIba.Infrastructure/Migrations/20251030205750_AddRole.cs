using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BambaIba.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_roles_role_id1",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_role_id1",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "role_id1",
                table: "user_roles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "role_id1",
                table: "user_roles",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id1",
                table: "user_roles",
                column: "role_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_roles_role_id1",
                table: "user_roles",
                column: "role_id1",
                principalTable: "roles",
                principalColumn: "id");
        }
    }
}
