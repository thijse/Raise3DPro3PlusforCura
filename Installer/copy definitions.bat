REM instead of building an installer everytime you want to test a parameter
REM you can use this batchfile to copy the profiles from the repository to 
REM the local appdata directory.

PUSHD "%~dp0"
xcopy "..\Cura\Current\Release" "%appdata%\cura\4.13" /s /Y
POPD