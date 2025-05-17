rem @echo off
setlocal enabledelayedexpansion

rem Folder of ECU Line4 PC
set SOURCE_FOLDER=C:\NK2R4_5\MEASURE
rem Folder copy of server
set DESTINATION_FOLDER_LINE=\\192.168.0.111\e\LINE4
rem File name of Line4
set FILE_LINE=NK2R4_5_Lasor_HU_Info

REM search file in folder ecu source_folder
for /f %%f in ('dir /b %SOURCE_FOLDER%') DO (
	rem Get file name
	set FILE_NAME=%%~nf
	rem Get 21 characters of file name
	set SUB_STR=!FILE_NAME:~0,21!
	rem check match with filename Line4
	IF !SUB_STR! == %FILE_LINE% (
		rem copy file to Line4
		copy %SOURCE_FOLDER%\%%f %DESTINATION_FOLDER_LINE%
	)
)
endlocal