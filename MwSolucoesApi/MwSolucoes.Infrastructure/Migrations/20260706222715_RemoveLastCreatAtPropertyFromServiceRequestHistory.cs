using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MwSolucoes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLastCreatAtPropertyFromServiceRequestHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "ServiceRequestHistory");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "ServiceRequestHistory",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
