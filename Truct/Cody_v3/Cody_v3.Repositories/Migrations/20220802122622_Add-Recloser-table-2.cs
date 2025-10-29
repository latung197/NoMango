using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddReclosertable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reclosers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecloserCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    RecloserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RecloserQualify = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    I_Norms = table.Column<int>(type: "int", nullable: false),
                    LineType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedIPAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    CreatedPCName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UpdatedIPAddress = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    UpdatedPCName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclosers", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reclosers");
        }
    }
}
