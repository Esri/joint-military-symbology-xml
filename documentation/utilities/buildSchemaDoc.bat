@echo off
SETLOCAL

REM core script directories
set BASE_DIR=%~dp0

set INPUT=%1%
set OUTPUT=%2%
set XSLT=%3%

set JAVA_HEAP_MAX=512M
copy ..\..\schema\core.xsd .
copy ..\..\schema\symbolSet.xsd .
cscript %BASE_DIR%\applyXSLT6.js ..\..\schema\core.xsd ..\core.html xs3p.xsl
cscript %BASE_DIR%\applyXSLT6.js ..\..\schema\symbolSet.xsd ..\symbolSet.html xs3p.xsl
cscript %BASE_DIR%\applyXSLT6.js ..\..\schema\base.xsd ..\base.html xs3p.xsl
del core.xsd
del symbolSet.xsd
ENDLOCAL