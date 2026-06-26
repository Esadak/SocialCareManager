using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCarePlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarePlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<string>(type: "text", nullable: false),
                    Needs = table.Column<string>(type: "text", nullable: false),
                    SupportPlan = table.Column<string>(type: "text", nullable: false),
                    RiskAssessment = table.Column<string>(type: "text", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarePlans_ServiceUsers_ServiceUserId",
                        column: x => x.ServiceUserId,
                        principalTable: "ServiceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarePlans_ServiceUserId",
                table: "CarePlans",
                column: "ServiceUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarePlans");
        }
    }
}
