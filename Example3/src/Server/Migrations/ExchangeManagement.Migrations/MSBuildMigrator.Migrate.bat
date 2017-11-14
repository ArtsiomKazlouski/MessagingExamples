@echo off
setlocal
REM Ensure we know whether we are running on a 32it or 64bit system
REM IF "%PROCESSOR_ARCHITECTURE%" == "x86" SET FRAMEWORKDIR=Framework
REM IF "%PROCESSOR_ARCHITECTURE%" == "AMD64" SET FRAMEWORKDIR=Framework64


REM set msbuildemitsolution=1

@setlocal enableextensions
@cd /d "%~dp0"

FluentMigratorTools\Migrate.exe /configPath App.config /connectionString  SubscriptionConnectionString  /provider SqlServer2008 /assembly ExchangeManagement.Migrations.dll 
pause