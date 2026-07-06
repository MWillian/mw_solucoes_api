using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MwSolucoes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixHistoryPropertyNamesAndPropertyMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequestHistory_ServiceRequests_ServiceRequestId1",
                table: "ServiceRequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequestHistory_ServiceRequestId1",
                table: "ServiceRequestHistory");

            migrationBuilder.DropColumn(
                name: "ServiceRequestId1",
                table: "ServiceRequestHistory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ServiceRequestId1",
                table: "ServiceRequestHistory",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestHistory_ServiceRequestId1",
                table: "ServiceRequestHistory",
                column: "ServiceRequestId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequestHistory_ServiceRequests_ServiceRequestId1",
                table: "ServiceRequestHistory",
                column: "ServiceRequestId1",
                principalTable: "ServiceRequests",
                principalColumn: "Id");
        }
    }
}
