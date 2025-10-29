using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddnewcolumnExcelFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Department",
                table: "RecloserDatas",
                newName: "SheetName");

            migrationBuilder.RenameIndex(
                name: "IX_RecloserDatas_Department_RecloserId_Date_Time",
                table: "RecloserDatas",
                newName: "IX_RecloserDatas_SheetName_RecloserId_Date_Time");

            migrationBuilder.AddColumn<string>(
                name: "ExcelFileName",
                table: "RecloserDatas",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcelFileName",
                table: "RecloserDatas");

            migrationBuilder.RenameColumn(
                name: "SheetName",
                table: "RecloserDatas",
                newName: "Department");

            migrationBuilder.RenameIndex(
                name: "IX_RecloserDatas_SheetName_RecloserId_Date_Time",
                table: "RecloserDatas",
                newName: "IX_RecloserDatas_Department_RecloserId_Date_Time");
        }
    }
}
