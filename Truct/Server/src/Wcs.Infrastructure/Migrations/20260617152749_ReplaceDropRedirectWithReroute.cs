using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDropRedirectWithReroute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingDropRedirect",
                table: "FlowTasks");

            migrationBuilder.DropColumn(
                name: "RedirectFromStationCode",
                table: "FlowTasks");

            migrationBuilder.AddColumn<int>(
                name: "RerouteCount",
                table: "FlowTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RerouteJson",
                table: "FlowTasks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RerouteCount",
                table: "FlowTasks");

            migrationBuilder.DropColumn(
                name: "RerouteJson",
                table: "FlowTasks");

            migrationBuilder.AddColumn<bool>(
                name: "PendingDropRedirect",
                table: "FlowTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RedirectFromStationCode",
                table: "FlowTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
