using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferRequestWarehouseQueueFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWipTray",
                table: "TransferRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PendingWarehouseStationCode",
                table: "TransferRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehousePendingKind",
                table: "TransferRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_Status_WarehousePendingKind_PendingWarehouseStationCode_CreatedAt",
                table: "TransferRequests",
                columns: new[] { "Status", "WarehousePendingKind", "PendingWarehouseStationCode", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransferRequests_Status_WarehousePendingKind_PendingWarehouseStationCode_CreatedAt",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "IsWipTray",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "PendingWarehouseStationCode",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "WarehousePendingKind",
                table: "TransferRequests");
        }
    }
}
