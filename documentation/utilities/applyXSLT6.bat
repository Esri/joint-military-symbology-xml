@echo off
SETLOCAL

REM core script directories
set BASE_DIR=%~dp0

set INPUT=%1%
set OUTPUT=%2%
set XSLT=%3%

set JAVA_HEAP_MAX=512M

cscript %BASE_DIR%\applyXSLT6.js %INPUT% %OUTPUT% %XSLT%

ENDLOCAL