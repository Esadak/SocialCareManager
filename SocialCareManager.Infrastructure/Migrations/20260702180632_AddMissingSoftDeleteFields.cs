using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSoftDeleteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ServiceUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ServiceUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "NextOfKin",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "NextOfKin",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "NextOfKin",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Medications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Medications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Medications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DailyNotes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "DailyNotes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DailyNotes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CarePlans",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CarePlans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CarePlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "NextOfKin");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "NextOfKin");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "NextOfKin");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DailyNotes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "DailyNotes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DailyNotes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CarePlans");
        }
    }
}
