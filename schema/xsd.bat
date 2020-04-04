REM Purpose: Used to recreate .NET source code binding when schema changes 

REM IMPORTANT: Change Path below to your version/location of xsd.exe

SET FXTOOLS_PATH=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools

"%FXTOOLS_PATH%\xsd.exe" base.xsd /c /l:CS /n:JointMilitarySymbologyLibrary /o:"..\source\JointMilitarySymbologyLibraryCS"

pause
