using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddReclosertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedIPAddress",
                table: "RecloserDatas",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedPCName",
                table: "RecloserDatas",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedIPAddress",
                table: "RecloserDatas",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedPCName",
                table: "RecloserDatas",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecloserDatas_Date",
                table: "RecloserDatas",
                column: "Date")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecloserDatas_Date",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "CreatedIPAddress",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "CreatedPCName",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "UpdatedIPAddress",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "UpdatedPCName",
                table: "RecloserDatas");
        }
    }
}
