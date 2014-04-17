:: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
:: IMPORTANT/TODO: you must set/correct paths below
::
:: IMPORTANT CONSTRAINT: 
:: 1. Must be FULL PATHs (because the replacement below depends on)
:: 2. No spaces in the folder/file names 
::
:: SET {TODO_NO_SPACES} to full path of 
:: 1. input .csv 
:: 2. desired output .style
::
:: Input csv format: filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,notes
::
:: For more information on this conversion process see:
:: https://github.com/williamscraigm/csv2ArcGISStyle 
:: !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

:: Military-Activities-Source-Icons.csv
:: Military-Air-Source-Icons.csv
:: Military-ControlMeasures-Source-Icons.csv
:: Military-Cyberspace-Source-Icons.csv
:: Military-Land-Source-Icons.csv
:: Military-Sea-Source-Icons.csv
:: Military-Sigint-Source-Icons.csv
:: Military-Space-Source-Icons.csv
:: Military-Weather-Source-Icons.csv

csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Activities-Source-Icons.csv {TODO_NO_SPACES}\Military-Activities.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Air-Source-Icons.csv {TODO_NO_SPACES}\Military-Air.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-ControlMeasures-Source-Icons.csv {TODO_NO_SPACES}\Military-ControlMeasures.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Cyberspace-Source-Icons.csv {TODO_NO_SPACES}\Military-Cyberspace.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Land-Source-Icons.csv {TODO_NO_SPACES}\Military-Land.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Sea-Source-Icons.csv {TODO_NO_SPACES}\Military-Sea.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Sigint-Source-Icons.csv {TODO_NO_SPACES}\Military-Sigint.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Weather-Source-Icons.csv {TODO_NO_SPACES}\Military-Weather.style
csv2ArcGISStyle.exe {TODO_NO_SPACES}\Military-Space-Source-Icons.csv {TODO_NO_SPACES}\Military-Space.style


