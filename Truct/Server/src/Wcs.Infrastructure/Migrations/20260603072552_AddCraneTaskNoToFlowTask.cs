using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCraneTaskNoToFlowTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CraneTaskNo",
                table: "FlowTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowTasks_CraneTaskNo",
                table: "FlowTasks",
                column: "CraneTaskNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlowTasks_CraneTaskNo",
                table: "FlowTasks");

            migrationBuilder.DropColumn(
                name: "CraneTaskNo",
                table: "FlowTasks");
        }
    }
}
