using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusToIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cassettes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Cassettes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Cassettes");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Cassettes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
