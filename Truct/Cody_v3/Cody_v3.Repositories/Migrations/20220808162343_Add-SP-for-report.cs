using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class AddSPforreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROC IF EXISTS dbo.USP_RecloserData_GetReport
GO 

SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON
GO
create PROC dbo.USP_RecloserData_GetReport @from DATE=null, @to DATE=null, @departmentId UNIQUEIDENTIFIER=NULL
AS
BEGIN
/*
	author:			Tona Dinh
	Description:	Get overload data for report
	Contact:		dinhtona@gmail.com
*/
	IF @from IS NULL SET @from=GETDATE()
	IF @to IS NULL SET @to = GETDATE()

	;WITH cteMax AS
	(
		SELECT 	
			ROW_NUMBER() OVER (PARTITION BY  d.RecloserId, d.Date ORDER BY  d.I_A desc, t.OrderId DESC)	RN,
			d.RecloserId,
			d.Date,
			d.Time AS Max_Time,
			d.I_A AS Max_I_A
		FROM dbo.RecloserDatas d
		JOIN dbo.Times t ON d.Time=t.TimeName
	)
	, cteAVG AS (
		SELECT 	
			AVG(d.I_A) AS AVG_I_A,
			d.RecloserId,
			d.Date
		FROM dbo.RecloserDatas d
		GROUP BY d.RecloserId,
			d.Date
	)
	, cteCount AS (
		SELECT 	
			 SUM(IIF(D.I_A>R.I_Norms, 1, 0)) AS Count_OverLoad,
			d.RecloserId,
			d.Date
		FROM dbo.RecloserDatas d
		JOIN dbo.Reclosers R ON R.Id = D.RecloserId
		GROUP BY d.RecloserId,
			d.Date
	)

	SELECT ROW_NUMBER() OVER (PARTITION BY 0 ORDER BY R.RecloserQualify,  a.Date, m.Max_Time) AS Row_ID, R.RecloserQualify, R.I_Norms, m.Max_I_A, ROUND(a.AVG_I_A,2) AS AVG_I_A
	, ROUND(m.Max_I_A*100/R.I_Norms,2) AS Percent_I_A
	, c.Count_OverLoad
	, c.Count_OverLoad * 30 AS Minutes_OverLoad
	, m.Max_Time, a.Date
	FROM dbo.RecloserDatas D 
	JOIN dbo.Reclosers R ON R.Id = D.RecloserId
	JOIN dbo.Departments DP ON DP.Id = R.DepartmentId
	JOIN cteMax m ON m.RecloserId = R.Id AND m.Date = D.Date AND m.Max_Time = D.Time AND m.RN=1
	JOIN cteAVG a ON a.RecloserId = R.Id AND a.Date = d.Date 
	LEFT JOIN cteCount c ON c.RecloserId = R.Id AND c.Date = D.Date 
	WHERE a.Date BETWEEN @from AND @to 
	AND (@departmentId IS NULL OR d.Id=@departmentId)
	GROUP BY R.RecloserQualify, R.I_Norms, m.Max_I_A, a.AVG_I_A, m.Max_Time, a.Date, c.Count_OverLoad
	ORDER BY R.RecloserQualify,  a.Date, m.Max_Time

END 

GO


");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"DROP PROC IF EXISTS dbo.USP_RecloserData_GetReport");
		}
    }
}
