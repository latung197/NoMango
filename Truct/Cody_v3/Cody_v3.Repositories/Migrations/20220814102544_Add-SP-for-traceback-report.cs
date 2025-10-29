using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddSPfortracebackreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    SET QUOTED_IDENTIFIER ON
					SET ANSI_NULLS ON
					GO
					DROP PROC IF EXISTS  dbo.USP_RecloserData_GetForTraceBack
					GO

					CREATE PROC dbo.USP_RecloserData_GetForTraceBack @from DATE=null, @to DATE=null, @departmentId UNIQUEIDENTIFIER=NULL
					AS
					BEGIN
					/*
						author:			Tona Dinh
						Description:	Get data for trace back
						Contact:		dinhtona@gmail.com
					*/
						IF @from IS NULL SET @from=GETDATE()
						IF @to IS NULL SET @to = GETDATE()

						SELECT d.*,
						R.RecloserQualify,
						DP.DepartmentName
						FROM dbo.RecloserDatas D 
						JOIN dbo.Reclosers R ON R.Id = D.RecloserId
						JOIN dbo.Departments DP ON DP.Id = R.DepartmentId	
						JOIN dbo.Times t ON d.Time= t.TimeName
						WHERE d.Date BETWEEN @from AND @to 
						AND (@departmentId IS NULL OR @departmentId=0x0 OR d.Id=@departmentId )	
						ORDER BY R.RecloserQualify,  d.Date, t.OrderId

					END 

					GO


"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql( @"DROP PROC IF EXISTS  dbo.USP_RecloserData_GetForTraceBack");
        }
    }
}
