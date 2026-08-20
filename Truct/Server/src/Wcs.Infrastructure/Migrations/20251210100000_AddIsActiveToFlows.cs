using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToFlows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add IsActive column to Flows table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE object_id = OBJECT_ID(N'[dbo].[Flows]') 
                               AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [dbo].[Flows]
                    ADD [IsActive] bit NOT NULL DEFAULT 1;
                END
            ");

            // Create index on IsActive
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes 
                               WHERE name='IX_Flows_IsActive' 
                               AND object_id = OBJECT_ID(N'[dbo].[Flows]'))
                BEGIN
                    CREATE INDEX [IX_Flows_IsActive] ON [dbo].[Flows]([IsActive]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop index
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes 
                           WHERE name='IX_Flows_IsActive' 
                           AND object_id = OBJECT_ID(N'[dbo].[Flows]'))
                BEGIN
                    DROP INDEX [IX_Flows_IsActive] ON [dbo].[Flows];
                END
            ");

            // Drop column
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[Flows]') 
                           AND name = 'IsActive')
                BEGIN
                    ALTER TABLE [dbo].[Flows]
                    DROP COLUMN [IsActive];
                END
            ");
        }
    }
}


