@echo off
setlocal enabledelayedexpansion

rem Set PostgreSQL database connection parameters
set PG_USER=postgres
set PG_DB=HitachiAstemo

rem Set the backup destination directory
set BACKUP_DIR=D:\Backup
set BACKUP_ECU_DIR=D:\Backup2

rem Set the timestamp for backup
set TIMESTAMP=!date:~10,4!!date:~4,2!!date:~7,2!_!time:~0,2!!time:~3,2!

rem Set the backup file path with timestamp
set BACKUP_FILE=%BACKUP_DIR%\backup_%PG_DB%_%TIMESTAMP%.sql

rem Create a full backup using pg_dump
pg_dump -U %PG_USER% -d %PG_DB% > %BACKUP_FILE% 

copy %BACKUP_FILE% %BACKUP_ECU_DIR%