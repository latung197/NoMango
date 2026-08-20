using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryCode",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOrder",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FlowTaskId",
                table: "Requests",
                type: "uniqueidentifier",
                maxLength: 50,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryCode",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "DeliveryOrder",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "FlowTaskId",
                table: "Requests");
        }
    }
}
