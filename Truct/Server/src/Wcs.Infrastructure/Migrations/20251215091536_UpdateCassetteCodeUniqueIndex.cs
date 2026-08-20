using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCassetteCodeUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropIndex(
            //     name: "IX_Flows_Status",
            //     table: "Flows");

            migrationBuilder.DropIndex(
                name: "IX_Cassettes_Code",
                table: "Cassettes");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Flows",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Create filtered unique index: Code is only unique when IsActive = 1
            // This allows multiple records with the same Code if IsActive = 0 (soft deleted)
            migrationBuilder.Sql(@"
                CREATE UNIQUE NONCLUSTERED INDEX IX_Cassettes_Code_IsActive
                ON Cassettes(Code)
                WHERE IsActive = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop filtered unique index
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS IX_Cassettes_Code_IsActive ON Cassettes;
            ");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Flows");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Flows_Status",
            //     table: "Flows",
            //     column: "Status");

            // Restore original unique index (without filter)
            migrationBuilder.CreateIndex(
                name: "IX_Cassettes_Code",
                table: "Cassettes",
                column: "Code",
                unique: true);
        }
    }
}
