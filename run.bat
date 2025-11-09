@echo off
REM Script simples para compilar e executar (Windows Batch)
powershell -ExecutionPolicy Bypass -File "%~dp0run.ps1" %*
if errorlevel 1 exit /b 1

