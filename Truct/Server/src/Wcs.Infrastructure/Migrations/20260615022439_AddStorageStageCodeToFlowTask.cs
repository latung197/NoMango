using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageStageCodeToFlowTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StorageStageCode",
                table: "FlowTasks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageStageCode",
                table: "FlowTasks");
        }
    }
}
