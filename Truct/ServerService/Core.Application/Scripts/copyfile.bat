@echo off
setlocal enabledelayedexpansion

REM Folder chứa file Ecu ở trong buồng kín
set SOURCE_FOLDER=C:\NK2R4_5\MEASURE
REM Folder chứa file Line3 (Đang đặt ở server). Service sẽ quét cái folder này để insert
set DESTINATION_FOLDER_LINE=\\192.168.0.111\d\LINE3
REM Folder chứa file Line4 (Đang đặt ở server). Service sẽ quét cái folder này để insert
set DESTINATION_FOLDER_LINE=\\192.168.0.111\e\LINE4
REM File name của line3
set FILE_LINE_3=NK2R4_5_FQA_Result
REM File name của line4
set FILE_LINE_4=NK2R4_5_Lasor_HU_Info

REM search file in folder ecu source_folder
for /f %%f in ('dir /b %SOURCE_FOLDER%') DO (
	REM Lấy file name
	set FILE_NAME=%%~nf
	REM lấy 18 ký tự tên file
	set SUB_STR=!FILE_NAME:~0,18!
	REM Kiểm tra với filename line 3
	IF !SUB_STR! == %FILE_LINE_3% (
		REM copy file sang line 3
		copy %SOURCE_FOLDER%\%%f %DESTINATION_FOLDER_LINE3%
	) ELSE (
		REM lấy 21 ký tự tên file
		set SUB_STR2=!FILE_NAME:~0,21!
		REM Kiểm tra với filename line 4
		IF !SUB_STR2! == %FILE_LINE_4% (
			REM copy file sang line 4
			copy %SOURCE_FOLDER%\%%f %DESTINATION_FOLDER_LINE4%
		)
	)
	REM echo !FILE_NAME:~0,18! >> D:\test.txt
)
endlocal