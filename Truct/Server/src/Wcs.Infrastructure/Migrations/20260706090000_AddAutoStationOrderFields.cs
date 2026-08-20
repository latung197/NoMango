using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Wcs.Infrastructure.Data;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(WcsDbContext))]
    [Migration("20260706090000_AddAutoStationOrderFields")]
    public partial class AddAutoStationOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "TransferRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AutoReceiveEnabled",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoSendEnabled",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoSendIsEmptyTray",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AutoSendToStage",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "TransferRequests");

            migrationBuilder.DropColumn(
                name: "AutoReceiveEnabled",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "AutoSendEnabled",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "AutoSendIsEmptyTray",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "AutoSendToStage",
                table: "Stations");
        }
    }
}
