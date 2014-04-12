# image-conversion-utilities

## Purpose

* Converts a set of Scalable Vector Graphic (SVG) files (arranged in a directory tree)
* Recreates the original folder structure/tree of the source folder in the destination folder

## Important Notes/Caveats/Limitations

* Windows-only
    * These utilities were written as DOS batch (.bat) files to be able to run relatively easily (with minimum configuration) 
    * I.e. without needing to install additional software (besides the SVG converter)
* No spaces in paths
    * Because of the batch file constraints (mainly needing to replace text in variables in a DOS batch file), there can't be any spaces in the paths
    * So, for instance, don't install your converter to "Program Files" or put the files to convert in "My Documents"
* Need to install a third party SVG converter utility
    * A utility such as Inkscape to perform the actual conversion of each file

## Instructions 

* Ensure that an appropriate SVG converter is installed
    * This utility has been tested with Inkscape, but several others are available
    * Inkscape is available at: http://www.inkscape.org/en/download/
    * It is recommended that you use the [.7zip version](http://downloads.sourceforge.net/inkscape/inkscape-0.48.4-1-win32.7z) which allows you to extract anywhere (so you don't put a space in the path)
* Select the desired .bat that matches your desired conversion method
    * e.g. `ConvertTree-SVGtoEMF.bat` to convert an SVG tree to Enhanced Metafile Format (EMF) 
* IMPORTANT: Modify the converter .bat command file to match your local paths
    *  Open and edit the desired .bat file, find the "IMPORTANT/TODO" section at the top, and edit the paths:
    *  (1) Converter location: full path to SVG converter
    *  (2) Source SVG Root Folder
    *  (3) Desired Destination Folder
    *  See [sample converter .bat file for more information](./ConvertTree-SVGtoEMF.bat)
*  Run the desired .bat file
    *  Open a Command Prompt
    *  `cd {local-path-to-bat}`
    *  Run the .bat, e.g. ConvertTree-SVGtoEMF.bat
    *  If you wish to capture the command-line output for later analysis of errors add the following to the command above: `ConverterOutput.txt 2>&1`
    *  (Full Example)  `ConvertTree-SVGtoEMF.bat > ConverterOutput.txt 2>&1` 
    *  *(May take several minutes depending on the number of files)*
    *  Check the output for errors
*  Check the destination folder for the converted image files


