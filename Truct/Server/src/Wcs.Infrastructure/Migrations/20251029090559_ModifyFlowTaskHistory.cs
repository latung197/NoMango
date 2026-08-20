using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFlowTaskHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionTimeMs",
                table: "FlowTaskHistory");

            migrationBuilder.DropColumn(
                name: "ToStep",
                table: "FlowTaskHistory");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "FlowTaskHistory",
                newName: "LogMessage");

            migrationBuilder.RenameColumn(
                name: "FromStep",
                table: "FlowTaskHistory",
                newName: "Step");

            migrationBuilder.RenameColumn(
                name: "ExecutedAt",
                table: "FlowTaskHistory",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_FlowTaskHistory_ExecutedAt",
                table: "FlowTaskHistory",
                newName: "IX_FlowTaskHistory_CreatedAt");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "FlowTasks",
                type: "int",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Event",
                table: "FlowTaskHistory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Event",
                table: "FlowTaskHistory");

            migrationBuilder.RenameColumn(
                name: "Step",
                table: "FlowTaskHistory",
                newName: "FromStep");

            migrationBuilder.RenameColumn(
                name: "LogMessage",
                table: "FlowTaskHistory",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "FlowTaskHistory",
                newName: "ExecutedAt");

            migrationBuilder.RenameIndex(
                name: "IX_FlowTaskHistory_CreatedAt",
                table: "FlowTaskHistory",
                newName: "IX_FlowTaskHistory_ExecutedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "FlowTasks",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<int>(
                name: "ExecutionTimeMs",
                table: "FlowTaskHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToStep",
                table: "FlowTaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
