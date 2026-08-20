using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlowTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FromStationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToStationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RobotCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentStep = table.Column<int>(type: "int", nullable: false),
                    RcsTaskId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlowTaskHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromStep = table.Column<int>(type: "int", nullable: true),
                    ToStep = table.Column<int>(type: "int", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowTaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowTaskHistory_FlowTasks_FlowTaskId",
                        column: x => x.FlowTaskId,
                        principalTable: "FlowTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlowTaskWaitingSets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FlowTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NeedType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsMet = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlowTaskWaitingSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlowTaskWaitingSets_FlowTasks_FlowTaskId",
                        column: x => x.FlowTaskId,
                        principalTable: "FlowTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlowTaskHistory_ExecutedAt",
                table: "FlowTaskHistory",
                column: "ExecutedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTaskHistory_FlowTaskId",
                table: "FlowTaskHistory",
                column: "FlowTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_CreatedAt",
                table: "FlowTasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_CurrentStep",
                table: "FlowTasks",
                column: "CurrentStep");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_FromStationCode",
                table: "FlowTasks",
                column: "FromStationCode");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_RobotCode",
                table: "FlowTasks",
                column: "RobotCode");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_Status",
                table: "FlowTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_TaskId",
                table: "FlowTasks",
                column: "TaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_ToStationCode",
                table: "FlowTasks",
                column: "ToStationCode");

            migrationBuilder.CreateIndex(
                name: "IX_FlowTaskWaitingSets_FlowTaskId_NeedType",
                table: "FlowTaskWaitingSets",
                columns: new[] { "FlowTaskId", "NeedType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlowTaskHistory");

            migrationBuilder.DropTable(
                name: "FlowTaskWaitingSets");

            migrationBuilder.DropTable(
                name: "FlowTasks");
        }
    }
}
