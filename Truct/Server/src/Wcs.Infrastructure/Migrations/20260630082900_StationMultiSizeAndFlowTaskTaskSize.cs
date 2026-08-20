using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StationMultiSizeAndFlowTaskTaskSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sizes",
                table: "Stations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "1");

            migrationBuilder.Sql("UPDATE Stations SET Sizes = CAST(Size AS nvarchar(10))");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Stations");

            migrationBuilder.AddColumn<int>(
                name: "TaskSize",
                table: "FlowTasks",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskSize",
                table: "FlowTasks");

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Stations",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                "UPDATE Stations SET Size = TRY_CAST(PARSENAME(REPLACE(Sizes, ',', '.'), 1) AS int) WHERE Sizes IS NOT NULL");

            migrationBuilder.DropColumn(
                name: "Sizes",
                table: "Stations");
        }
    }
}
