using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyNoteAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DailyNotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "DailyNotes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DailyNotes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "DailyNotes");
        }
    }
}
