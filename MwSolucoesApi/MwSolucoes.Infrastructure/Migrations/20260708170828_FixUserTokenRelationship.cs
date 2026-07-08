using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MwSolucoes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserTokenRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId1",
                table: "UserTokens");

            migrationBuilder.DropIndex(
                name: "IX_UserTokens_UserId1",
                table: "UserTokens");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "UserTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId1",
                table: "UserTokens",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId1",
                table: "UserTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
