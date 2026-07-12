using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCarePlanVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarePlans_ServiceUserId",
                table: "CarePlans");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "CarePlans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArchivedBy",
                table: "CarePlans",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreviousVersionId",
                table: "CarePlans",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "CarePlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarePlans_PreviousVersionId",
                table: "CarePlans",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarePlans_ServiceUserId_IsActive",
                table: "CarePlans",
                columns: new[] { "ServiceUserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CarePlans_ServiceUserId_VersionNumber",
                table: "CarePlans",
                columns: new[] { "ServiceUserId", "VersionNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarePlans_CarePlans_PreviousVersionId",
                table: "CarePlans",
                column: "PreviousVersionId",
                principalTable: "CarePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarePlans_CarePlans_PreviousVersionId",
                table: "CarePlans");

            migrationBuilder.DropIndex(
                name: "IX_CarePlans_PreviousVersionId",
                table: "CarePlans");

            migrationBuilder.DropIndex(
                name: "IX_CarePlans_ServiceUserId_IsActive",
                table: "CarePlans");

            migrationBuilder.DropIndex(
                name: "IX_CarePlans_ServiceUserId_VersionNumber",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "ArchivedBy",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "PreviousVersionId",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "CarePlans");

            migrationBuilder.CreateIndex(
                name: "IX_CarePlans_ServiceUserId",
                table: "CarePlans",
                column: "ServiceUserId");
        }
    }
}
