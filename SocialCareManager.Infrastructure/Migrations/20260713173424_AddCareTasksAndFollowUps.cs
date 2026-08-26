using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialCareManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCareTasksAndFollowUps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DueAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedTo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StartedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelledBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CancellationReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Recurrence = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RecurrenceInterval = table.Column<int>(type: "integer", nullable: false),
                    RecurrenceEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareTasks_CareTasks_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "CareTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CareTasks_ServiceUsers_ServiceUserId",
                        column: x => x.ServiceUserId,
                        principalTable: "ServiceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CareTaskFollowUps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CareTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    FollowedUpAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FollowedUpBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareTaskFollowUps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareTaskFollowUps_CareTasks_CareTaskId",
                        column: x => x.CareTaskId,
                        principalTable: "CareTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareTaskFollowUps_CareTaskId",
                table: "CareTaskFollowUps",
                column: "CareTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CareTaskFollowUps_FollowedUpAt",
                table: "CareTaskFollowUps",
                column: "FollowedUpAt");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_AssignedTo",
                table: "CareTasks",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_DueAt",
                table: "CareTasks",
                column: "DueAt");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_ParentTaskId",
                table: "CareTasks",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_Priority",
                table: "CareTasks",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_ServiceUserId",
                table: "CareTasks",
                column: "ServiceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_ServiceUserId_DueAt",
                table: "CareTasks",
                columns: new[] { "ServiceUserId", "DueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_ServiceUserId_Status",
                table: "CareTasks",
                columns: new[] { "ServiceUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_Status",
                table: "CareTasks",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareTaskFollowUps");

            migrationBuilder.DropTable(
                name: "CareTasks");
        }
    }
}
