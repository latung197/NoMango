using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAmrTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amrs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MapCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    MapName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RobotStatus = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TypeCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Battery = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Exclude = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ExcludeStr = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OnLine = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    PodCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    PodDir = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PosX = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PosY = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    StatusStr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Stop = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    StopStr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amrs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amrs_Code",
                table: "Amrs",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amrs");
        }
    }
}
