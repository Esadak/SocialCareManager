using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCarePlanAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CarePlans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CarePlans",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CarePlans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CarePlans");
        }
    }
}
