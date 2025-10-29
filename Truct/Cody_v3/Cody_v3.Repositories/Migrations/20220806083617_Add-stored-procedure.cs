using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class Addstoredprocedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"

            CREATE PROC dbo.USP_RecloserData_Import @json NVARCHAR(max)
			AS
			BEGIN
				SET NOCOUNT ON;
				DECLARE @temp TABLE (
					[Date] date ,
					[Time] nvarchar(20),
					[U_kV] decimal(18, 2),
					[I_A] decimal(18, 2) ,
					[P_MW] decimal(18, 2) ,
					[Q_MVar] decimal(18, 2) ,
					[COS_φ] decimal(18, 2) ,		
					SheetName nvarchar(100)  ,
					[RecloserId] UNIQUEIDENTIFIER,
					ExcelFileName NVARCHAR(1024),
					RequestIP NVARCHAR(256),
					RequestPCName NVARCHAR(256)
				)
				INSERT INTO @temp
				(
					Date,
					Time,
					U_kV,
					I_A,
					P_MW,
					Q_MVar,
					COS_φ,
					SheetName,
					RecloserId,
					ExcelFileName,
					RequestIP,
					RequestPCName
				)
				SELECT
						   Date,
						   Time,
						   U_kV,
						   I_A,
						   P_MW,
						   Q_MVar,
						   COS_φ,
						   SheetName,
						   b.Id		,
						   ExcelFileName,
						   RequestIP,
						   RequestPCName
						FROM OPENJSON(@json)
						WITH(
				--		--https://docs.microsoft.com/en-us/sql/relational-databases/json/json-data-sql-server?view=sql-server-ver16
				--		--firstName NVARCHAR(50) '$.info.name',
				--		--lastName NVARCHAR(50) '$.info.surname',
				--		--age INT,
				--		--dateOfBirth DATETIME2 '$.dob'
							[Date] date ,
							[Time] nvarchar(20),
							[U_kV] decimal(18, 2),
							[I_A] decimal(18, 2) ,
							[P_MW] decimal(18, 2) ,
							[Q_MVar] decimal(18, 2) ,
							[COS_φ] decimal(18, 2) ,		
							SheetName nvarchar(100) ,
							Recloser NVARCHAR(500),
							ExcelFileName NVARCHAR(1024),
							RequestIP NVARCHAR(256),
							RequestPCName NVARCHAR(256)
						)a
						JOIN dbo.Reclosers b ON a.Recloser=b.RecloserQualify
						WHERE	a.Time NOT IN ('min','max')

				SET NOCOUNT OFF;

				;MERGE dbo.RecloserDatas AS ds
				USING @temp AS st -- Include the columns Name
					ON ds.SheetName = st.SheetName
					AND ds.RecloserId= st.RecloserId
					AND ds.[Date]= st.[Date]
					AND ds.[Time]=st.[Time]
		
				-- It's not about matching, You have to add the expressions
				WHEN MATCHED THEN  
					UPDATE 
					SET 
						[U_kV]=st.[U_kV],
						[I_A]=st.[I_A],
						[P_MW]=st.[P_MW],
						[Q_MVar]=st.[Q_MVar],
						[COS_φ]=st.[COS_φ],
						ds.UpdatedDate=GETDATE(),
						ExcelFileName=st.ExcelFileName,
						ds.UpdatedIPAddress=st.RequestIP,
						ds.UpdatedPCName=st.RequestPCName

				WHEN NOT MATCHED THEN  
					INSERT 
						(
							Date,
							Time,
							U_kV,
							I_A,
							P_MW,
							Q_MVar,
							COS_φ,		
							SheetName,		
							RecloserId,
							ExcelFileName,
							CreatedIPAddress,
							CreatedPCName
						)	
					VALUES(
							st.Date,
							st.Time,
							st.U_kV,
							st.I_A,
							st.P_MW,
							st.Q_MVar,
							st.COS_φ,		
							st.SheetName,		
							st.RecloserId,
							st.ExcelFileName,
							st.RequestIP,
							st.RequestPCName
					);
			END

			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROC dbo.USP_RecloserData_Import");
        }
    }
}
