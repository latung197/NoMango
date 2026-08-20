using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CassetteSizeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeInt",
                table: "Cassettes",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Cassettes SET SizeInt = CASE UPPER(RTRIM(Size))
                    WHEN 'S' THEN 1
                    WHEN 'S+' THEN 2
                    WHEN 'M' THEN 2
                    WHEN 'M+' THEN 2
                    WHEN 'L' THEN 3
                    WHEN 'LL' THEN 5
                    WHEN 'XL' THEN 4
                    WHEN 'XXL' THEN 5
                    WHEN '1' THEN 1
                    WHEN '2' THEN 2
                    WHEN '3' THEN 3
                    WHEN '4' THEN 4
                    WHEN '5' THEN 5
                    ELSE 1
                END
                """);

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Cassettes");

            migrationBuilder.RenameColumn(
                name: "SizeInt",
                table: "Cassettes",
                newName: "Size");

            migrationBuilder.AlterColumn<int>(
                name: "Size",
                table: "Cassettes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SizeLabel",
                table: "Cassettes",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Cassettes SET SizeLabel = CASE Size
                    WHEN 1 THEN 'S'
                    WHEN 2 THEN 'M'
                    WHEN 3 THEN 'L'
                    WHEN 4 THEN 'XL'
                    WHEN 5 THEN 'XXL'
                    ELSE 'S'
                END
                """);

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Cassettes");

            migrationBuilder.RenameColumn(
                name: "SizeLabel",
                table: "Cassettes",
                newName: "Size");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "Cassettes",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldNullable: true);
        }
    }
}
