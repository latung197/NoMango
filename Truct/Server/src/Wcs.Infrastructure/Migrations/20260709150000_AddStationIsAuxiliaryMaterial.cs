using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Wcs.Infrastructure.Data;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(WcsDbContext))]
    [Migration("20260709150000_AddStationIsAuxiliaryMaterial")]
    public partial class AddStationIsAuxiliaryMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAuxiliaryMaterial",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuxiliaryMaterial",
                table: "Stations");
        }
    }
}
