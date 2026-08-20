using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveColumnForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CHỈ THÊM cột IsActive, KHÔNG XÓA cột Status cũ
            // Cột Status trong database giữ nguyên (không map vào Entity nữa)
            // Cột IsActive mới dùng cho xóa logic
            
            migrationBuilder.DropIndex(
                name: "IX_Flows_Status",
                table: "Flows");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stations",
                type: "bit",
                nullable: false,
                defaultValue: true); // Mặc định = true (hiển thị)

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stages",
                type: "bit",
                nullable: false,
                defaultValue: true); // Mặc định = true (hiển thị)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: chỉ xóa cột IsActive
            // KHÔNG thêm lại cột Status (vì nó không bao giờ bị xóa)
            
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stages");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_Status",
                table: "Flows",
                column: "Status");
        }
    }
}
