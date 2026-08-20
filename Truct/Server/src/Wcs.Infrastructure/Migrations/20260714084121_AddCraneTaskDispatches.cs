using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCraneTaskDispatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CraneTaskDispatches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskNo = table.Column<int>(type: "int", nullable: false),
                    FlowTaskId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompleteStatus = table.Column<int>(type: "int", nullable: true),
                    ErrorCode = table.Column<int>(type: "int", nullable: true),
                    ErrorNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletingAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CraneTaskDispatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CraneTaskDispatches_CreatedAt",
                table: "CraneTaskDispatches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CraneTaskDispatches_FlowTaskId",
                table: "CraneTaskDispatches",
                column: "FlowTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CraneTaskDispatches_IdempotencyKey_Active",
                table: "CraneTaskDispatches",
                column: "IdempotencyKey",
                unique: true,
                filter: "[Status] IN (0, 1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_CraneTaskDispatches_Status",
                table: "CraneTaskDispatches",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CraneTaskDispatches_TaskNo_Active",
                table: "CraneTaskDispatches",
                column: "TaskNo",
                unique: true,
                filter: "[Status] IN (0, 1, 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CraneTaskDispatches");
        }
    }
}
