@echo off
setlocal enabledelayedexpansion

rem Folder of ECU Line4 PC
set SOURCE_FOLDER=C:\NK2R4_5\MEASURE
rem Folder copy of server
set DESTINATION_FOLDER_LINE=\\192.168.0.111\e\LINE4
rem File name of Line4
set FILE_LINE=NK2R4_5_Lasor_HU_Info

rem deleclearte content from log text file
type nul > logfile.txt

if exist %DESTINATION_FOLDER_LINE%\ (
  echo Start copy to %DESTINATION_FOLDER_LINE%\>> logfile.txt
) else (
  echo Invalid drive specification - %DESTINATION_FOLDER_LINE%\>> logfile.txt
  Exit /b
)

REM search file in folder ecu source_folder
for /f %%f in ('dir /b %SOURCE_FOLDER%') DO (
	rem Get file name
	set FILE_NAME=%%~nf
	rem Get 21 characters of file name
	set SUB_STR=!FILE_NAME:~0,21!
	rem check match with filename Line4
	IF !SUB_STR! == %FILE_LINE% (
		rem copy file to Line4
		xcopy /f /y %SOURCE_FOLDER%\%%f %DESTINATION_FOLDER_LINE%

		if %errorlevel% equ 0 (
			echo Files were copied without error: %SOURCE_FOLDER%\%%f - %DESTINATION_FOLDER_LINE%\%%f >> logfile.txt
		)
		if %errorlevel% equ 1 (
			echo No files were found to copy.>> logfile.txt
		)
		if %errorlevel% equ 2 (
			echo You pressed CTRL+C to end the copy operation.>> logfile.txt
		)
		if %errorlevel% equ 4 (
			echo Insufficient memory or invalid drive, or command-line syntax.>> logfile.txt
		)
		if %errorlevel% equ 5 (
			echo Disk write error occurred.>> logfile.txt
		)
	)
)
endlocal