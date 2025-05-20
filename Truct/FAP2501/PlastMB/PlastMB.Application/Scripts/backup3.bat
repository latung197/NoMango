@echo off
setlocal enabledelayedexpansion

rem Set PostgreSQL bin directory path
set PG_BIN=C:\Program Files\PostgreSQL\16\bin

rem Set PostgreSQL database connection parameters
set PG_USER=postgres
set PG_DB=PlastMB

rem Set the backup destination directory
set BACKUP_DIR=D:\Database\Backup
set BACKUP_ECU_DIR=\\192.168.0.233\Database\Backup

rem Set the timestamp for backup
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YY=%dt:~2,2%" & set "YYYY=%dt:~0,4%" & set "MM=%dt:~4,2%" & set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%" & set "Min=%dt:~10,2%" & set "Sec=%dt:~12,2%"

set "datestamp=%YYYY%%MM%%DD%"
set "fullstamp=%YYYY%-%MM%-%DD%_%HH%-%Min%-%Sec%"

rem Set the backup file path with timestamp
set BACKUP_FILE=%BACKUP_DIR%\backup_%PG_DB%_%fullstamp%.sql

rem Create a full backup using pg_dump
pg_dump -w -U %PG_USER% -d %PG_DB% > %BACKUP_FILE% 

copy %BACKUP_FILE% %BACKUP_ECU_DIR%