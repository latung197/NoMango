using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddDefaultTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP TABLE IF EXISTS dbo.Times

                CREATE TABLE Times(
	                Id INT IDENTITY NOT NULL,
	                TimeName NVARCHAR(10) PRIMARY KEY NOT NULL,
	                OrderId FLOAT null 
                )
                GO

                INSERT INTO dbo.Times
                (
                    TimeName,
	                OrderId
                )
                VALUES
                ('0h30',1),
                ('1h',2),
                ('1h30',3),
                ('2h',4),
                ('2h30',5),
                ('3h',6),
                ('3h30',7),
                ('4h',8),
                ('4h30',9),
                ('5h',10),
                ('5h30',11),
                ('6h',12),
                ('6h30',13),
                ('7h',14),
                ('7h30',15),
                ('8h',16),
                ('8h30',17),
                ('9h',18),
                ('9h30',19),
                ('10h',20),
                ('10h30',21),
                ('11h',22),
                ('11h30',23),
                ('12h',24),
                ('12h30',25),
                ('13h',26),
                ('13h30',27),
                ('14h',28),
                ('14h30',29),
                ('15h',30),
                ('15h30',31),
                ('16h',32),
                ('16h30',33),
                ('17h',34),
                ('17h30',35),
                ('18h',36),
                ('18h30',37),
                ('19h',38),
                ('19h30',39),
                ('20h',40),
                ('20h30',41),
                ('21h',42),
                ('21h30',43),
                ('22h',44),
                ('22h30',45),
                ('23h',46),
                ('23h30',47),
                ('24h',48)

            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TABLE IF EXISTS dbo.Times");
        }
    }
}
