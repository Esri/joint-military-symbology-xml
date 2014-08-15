@ECHO OFF

:: License Apache V2 - Adapted from Craig Williams script:
:: https://github.com/williamscraigm/csv2ArcGISStyle/blob/master/SVGtoEMF.bat 
:: Adaptations were to create the same folder structure in the destination folder
:: This was needed to handle a set of source svgs with a complex folder structure that
:: needed to be maintained

:: This script converts SVG files to EMF/PNG.  Run as admin
:: For Inkscape help, see http://inkscape.org/doc/inkscape-man.html
:: For an alternative to Inkscape that works just as well, see http://code.google.com/p/svg2emf/

:: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
:: IMPORTANT/TODO: you must set/correct paths below
::
:: IMPORTANT CONSTRAINT: 
:: 1. Must be FULL PATHs (because the replacement below depends on)
:: 2. No spaces in the folder/file names (doing text replacement of variables in DOS is just too impossible)

:: INPUTS/SETTINGS:
:: (1) Converter location:
SET converter=C:\{TODO_NO_SPACES}\inkscape\inkscape.com

:: (2) Source SVG Root Folder
SET source_folder=C:\{TODO_NO_SPACES_PATH_TO_SVGs}

:: (3) Desired Destination Folder (this does not need to exist - it will be created if not)
SET destination_folder=C:\{TODO_NO_SPACES_DESIRED_PATH_TO_OUTPUT_FILES}

:: (4) (If desired) change height / width of png see TODO below:
:: "TODO: SET THE DESIRED HEIGHT/WIDTH HERE (currently 128 pixels)" 

:: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

:: it is the default, but just in case, this enables the needed feature "goto :eof" (i.e. "return")
setlocal ENABLEEXTENSIONS

:: Check the converter destination folder exists
if exist %converter% goto prereqs_converter_exists_ok

echo "Required Converter does not exist: %converter%"
goto :EOF

:prereqs_converter_exists_ok

:: Check source folder exists
if exist %source_folder% goto prereqs_exists_ok

echo "Required Source Folder does not exist: %source_folder%"
goto :EOF

:prereqs_exists_ok

:: IMPORTANT: The destination folders must exist in order for conversion below to work 
:: (so we create them in advance)
call :CreateDestinationFolders %source_folder% %destination_folder%

:: Note: Use "/r" option if you want to convert a folder and all subfolders (recursion)
:: do ExportImage for each svg found:
for /r "%source_folder%" %%i in (*.svg) do call :ExportImage %converter% %%i %source_folder% %destination_folder%

:: Done
echo Successfully Completed!
goto :EOF

:: ----------------------------------------------------------------------------
:: ExportImage - export image, maintaining same folder structure
:: 
:: %1 - Converter executable
:: %2 - Source Image (full path)
:: %3 - Source Folder
:: %4 - Destination Folder 
:: ----------------------------------------------------------------------------

:ExportImage
if "" == "%1" goto :EOF
if "" == "%2" goto :EOF
if "" == "%3" goto :EOF
if "" == "%4" goto :EOF

setlocal ENABLEEXTENSIONS
setlocal ENABLEDELAYEDEXPANSION

:: Check destination folder exists
if exist %4 goto exists_ok

echo "Required Folder does not exist: %4"
goto :EOF

:exists_ok

:: Text Replacement with variables in DOS - oy vey
set original=%3
set replacement=%4
set source_file=%2
set new_file=!source_file:%original%=%replacement%!
set new_file_png=%new_file:.svg=.png%
echo "Exporting %source_file% ==> %new_file_png%"

:: DEBUG:
::    echo "%1" "%2" "%3" "%4" "%new_file_png%"

:: IMPORTANT: Now actually call the exporter/converter here:
:: TODO: SET THE DESIRED HEIGHT/WIDTH HERE (currently 64 pixels):
"%1" "%2" --export-width=128 --export-height=128 --export-png="%new_file_png%" 

endlocal & goto :EOF

:: ----------------------------------------------------------------------------
:: CreateDestinationFolders - Creates Required Destination Folders 
::                            (based on source folder)
:: %1 - Source Folder
:: %2 - Destination Folder
:: ----------------------------------------------------------------------------

:CreateDestinationFolders

:: get structure of source and recreate in destination folder

if "" == "%1" goto :EOF
if "" == "%2" goto :EOF

setlocal ENABLEEXTENSIONS

echo "Creating Folder Structure of %1 in %2"

echo "Creating Dest Folder Root: %2"
mkdir "%2"

for /f "delims=" %%i in ('dir /ad/s/b %1') do call :CreateReplacementFolder %1 %2 %%i

endlocal & goto :EOF
:: ----------------------------------------------------------------------------

:: ----------------------------------------------------------------------------
:: CreateReplacementFolder - replaces the source folder root with the destination
::                           and creates the folder
::
:: %1 - Source Folder Root
:: %2 - Destination Folder Root
:: %3 - Current Folder
:: ----------------------------------------------------------------------------

:CreateReplacementFolder

setlocal ENABLEEXTENSIONS
setlocal ENABLEDELAYEDEXPANSION

:: Text Replacement with variables in DOS - oy vey
set original=%1
set replacement=%2
set source_folder=%3
set new_folder=!source_folder:%original%=%replacement%!
echo "Creating New Folder for Copying: %source_folder% ==> %new_folder%"

mkdir "%new_folder%"

endlocal & goto :EOF