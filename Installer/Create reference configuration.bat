REM Create a clean Cura configuration set.
REM use this between printer profile updates to reset local changes
REM that may override your printer settings

PUSHD "%~dp0"

mkdir "..\Configuration reference"
del /f /s /q "..\Configuration reference" 1>nul

xcopy  "%appdata%\cura\4.13"  "..\Configuration reference" /s /Y
POPD