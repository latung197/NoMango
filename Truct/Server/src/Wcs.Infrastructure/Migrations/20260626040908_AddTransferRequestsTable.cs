using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FromStageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ToStageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    FromStationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ToStationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsEmptyTray = table.Column<bool>(type: "bit", nullable: false),
                    CassetteCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Product = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    StorageStageCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RobotCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CranePositionsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    MatchedRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FlowTaskId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_ExpiresAt",
                table: "TransferRequests",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_FromStageCode_ToStageCode_Size_Status",
                table: "TransferRequests",
                columns: new[] { "FromStageCode", "ToStageCode", "Size", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_Status",
                table: "TransferRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferRequests");
        }
    }
}
