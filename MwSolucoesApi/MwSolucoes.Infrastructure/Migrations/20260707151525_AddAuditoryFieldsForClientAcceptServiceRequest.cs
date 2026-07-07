using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MwSolucoes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoryFieldsForClientAcceptServiceRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "ServiceRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedIpAddress",
                table: "ServiceRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcceptedUserAgent",
                table: "ServiceRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServiceRequestHistory",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "AcceptedIpAddress",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "AcceptedUserAgent",
                table: "ServiceRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ServiceRequestHistory",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
