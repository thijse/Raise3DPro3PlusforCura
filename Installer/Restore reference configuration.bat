REM Restore a clean Cura configuration set.
REM use this between printer profile updates to reset local changes
REM that may override your printer settings

@echo off
setlocal
:PROMPT
SET /P AREYOUSURE=Are you sure (Y/[N])? This will remove/restore all your configuration settings 
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END

PUSHD "%~dp0"
del /f /s /q "%appdata%\cura\4.13" 1>nul
xcopy  "..\Configuration reference" "%appdata%\cura\4.13" /s /Y
POPD


:END
endlocal

