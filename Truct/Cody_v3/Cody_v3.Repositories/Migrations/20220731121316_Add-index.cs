using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class Addindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "RecloserDatas",
                type: "Date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecloserDatas_Department_Recloser_Date_Time",
                table: "RecloserDatas",
                columns: new[] { "Department", "Recloser", "Date", "Time" },
                unique: true,
                filter: "[Date] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecloserDatas_Department_Recloser_Date_Time",
                table: "RecloserDatas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "RecloserDatas",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "Date",
                oldNullable: true);
        }
    }
}
