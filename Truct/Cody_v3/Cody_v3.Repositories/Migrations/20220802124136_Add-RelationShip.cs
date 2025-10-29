using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddRelationShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RecloserDatas_Department_Recloser_Date_Time",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "Recloser",
                table: "RecloserDatas");

            migrationBuilder.AddColumn<Guid>(
                name: "RecloserId",
                table: "RecloserDatas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RecloserDatas_Department_RecloserId_Date_Time",
                table: "RecloserDatas",
                columns: new[] { "Department", "RecloserId", "Date", "Time" },
                unique: true,
                filter: "[Date] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecloserDatas_RecloserId",
                table: "RecloserDatas",
                column: "RecloserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecloserDatas_Reclosers_RecloserId",
                table: "RecloserDatas",
                column: "RecloserId",
                principalTable: "Reclosers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecloserDatas_Reclosers_RecloserId",
                table: "RecloserDatas");

            migrationBuilder.DropIndex(
                name: "IX_RecloserDatas_Department_RecloserId_Date_Time",
                table: "RecloserDatas");

            migrationBuilder.DropIndex(
                name: "IX_RecloserDatas_RecloserId",
                table: "RecloserDatas");

            migrationBuilder.DropColumn(
                name: "RecloserId",
                table: "RecloserDatas");

            migrationBuilder.AddColumn<string>(
                name: "Recloser",
                table: "RecloserDatas",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RecloserDatas_Department_Recloser_Date_Time",
                table: "RecloserDatas",
                columns: new[] { "Department", "Recloser", "Date", "Time" },
                unique: true,
                filter: "[Date] IS NOT NULL");
        }
    }
}
