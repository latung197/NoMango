using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCraneDispatchQrFinishedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QrFinishedClearedAt",
                table: "CraneTaskDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QrFinishedHadCachedQr",
                table: "CraneTaskDispatches",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "QrFinishedRaisedAt",
                table: "CraneTaskDispatches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QrFinishedRaisedValue",
                table: "CraneTaskDispatches",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrFinishedClearedAt",
                table: "CraneTaskDispatches");

            migrationBuilder.DropColumn(
                name: "QrFinishedHadCachedQr",
                table: "CraneTaskDispatches");

            migrationBuilder.DropColumn(
                name: "QrFinishedRaisedAt",
                table: "CraneTaskDispatches");

            migrationBuilder.DropColumn(
                name: "QrFinishedRaisedValue",
                table: "CraneTaskDispatches");
        }
    }
}
