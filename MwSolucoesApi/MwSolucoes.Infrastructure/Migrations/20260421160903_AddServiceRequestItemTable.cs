using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MwSolucoes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRequestItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceRequestItems",
                columns: table => new
                {
                    ServiceRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaintenanceServiceId = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestItems", x => new { x.ServiceRequestId, x.MaintenanceServiceId });
                    table.ForeignKey(
                        name: "FK_ServiceRequestItems_MaintenanceServices_MaintenanceServiceId",
                        column: x => x.MaintenanceServiceId,
                        principalTable: "MaintenanceServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequestItems_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestItems_MaintenanceServiceId",
                table: "ServiceRequestItems",
                column: "MaintenanceServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequestItems");
        }
    }
}
