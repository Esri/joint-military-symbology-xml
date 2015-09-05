# joint-military-symbology-xml #

## Samples ##

This samples folder contains sample output from the tools included in this repo.  This document explains how these files were generated.

Usage for the jmsml console application is:

![](usage.jpg)

### All_Entities.csv. All_Modifier_Ones.csv and All_Modifier_Twos,csv ###
Generate a simple output of entities and modifiers using the following jmsml command line arguments, to export all of the current contents of the JMSML library:

	jmsml.exe /xe="All"

### Air_Entities.csv, Air_Modifier_Ones.csv, and Air_Modifier_Twos.csv ###
Generate a simple output of entities and modifiers using the following jmsml command line arguments, to extract just the contents of the Air symbol set from the JMSML library:

	jmsml.exe /xe="Air" /s="^Air$"

### LightAir_Entities.csv, LightAir_Modifier_Ones.csv and LightAir_Modifier_Twos.csv ###
Generate a simple output of entities and modifiers using the following jmsml command line arguments, to extract just the contents of the Air symbol set, where the label and/or category information contains the word "Light".

	jmsml.exe /xe="Air" /s="^Air$" /q="Light"

### Coded Domain Tables ###
The coded domain tables are comma separated files that have been exported from the JMSML library, for use by developers who need a straightforward dump of its contents.  It contains the label attribute and SIDC code for each element in the standard, exported to multiple files.  The /b switch value should be the location (folder) in which you want the files placed.

The optional /e switch causes the coded domain table export to add additional data validation information to the output.

	jmsml.exe /b=".'
	jmsml.exe /b="." /e

### Coded Domain Entity/Modifier Tables ###
Coded domain tables for entities and modifiers can be generated with or without the same optional filters/regular expressions documented above (with the /s, /q, /a, /l, and /p switches) by specifying the /xs="DOMAIN" switch.

	jmsml.exe /xe="All_Coded_Domain" /xas="DOMAIN"
	jmsml.exe /xe="Air_Coded_Domain" /s="^Air$" /xas="DOMAIN"

Coded domain tables for frames, amplifiers, and HQ/TF/FD can also be generated using /xas="DOMAIN" with the /xf, /xa, and /xh switches, respectively, which are further documented below.

### Amplifier Value Domains ###
Coded domain tables for miscellaneous text amplifiers can be exported from the data within JMSML.  These domains are defined where the military symbology standards call for a discrete list of allowed values for specific amplifiers.  These include, but are not limited to, the following amplifiers:

- Reinforced or reduced
- Evaluation rating (reliability and credibility)
- Combat effectiveness
- SIGINT mobilities
- Speed units


    jmsml.exe /xavd="Coded_Domain"

### Image File, Name, Category, Tags Tables ###
The information exported in these files includes that which can be used to generate ArcGIS Style files.  The information includes the path and name of each graphic/image file for a symbol, its name, the category of icon it belongs to, and a semicolon delimited list of tag values that users can use to find a given symbol.  Use the /xas="IMAGE" switch to enable this type of output.

The /s, /q, /a, /l, and /p switches documented above are also respected.  /+ can be used to tell the exporter to append the modifier output to the end of the entity output.

	jmsml.exe /xe="Military-Air-Source-Icons" /s="Air" /xas="IMAGE"
	jmsml.exe /xe="Military-Air-Source-Icons" /s="Air" /+ /xas="IMAGE"

Frame image file/name/category/tags information can be exported with the /xf switch.  Use optional switches /qc, /qd, and /qi to provide regular expressions to filter on Context, Dimension, and Standard Identity respectively.

	jmsml.exe /xf="Military-Frame-Source-Icons" /xas="IMAGE"
	jmsml.exe /xf="Military-Reality-Frame-Source-Icons" /qc="Reality" /xas="IMAGE"

The /-source switch can be used to disable the export of source image file information, for production purposes.

Amplifiers (Echelon, Mobility, and Auxiliary Equipment) can be exported with the /xa switch.  The /-source switch can be used to disable the export of source file information and the /+ switch can be used to force output to be appended to an existing export file.

	jmsml.exe /xa="Military-Amplifier-Source-Icons" /xas="IMAGE"
	jmsml.exe /xa="Military-Frame-And-Amplifier-Icons" /-source /+ /xas="IMAGE"

HQTFFD (Headquarter, Task Force, and Feint/Dummy) amplifiers can be exported with the /xh switch.  The /-source switch can be used to disable the export of source file information and the /+ switch can be used to force output to be appended to an existing export file.

	jmsml.exe /xh="Military-Frame-And-Amplifier-Icons" /+ /xas="IMAGE" 

### Legacy Support ###
JMSML contains legacy data that can be used to map 2525C SIDCs to their equivalent 2525D SIDCs, and conversely, 2525D SIDCs to their equivalent 2525C SIDCs, where relevant.

Please note that there are far more possible 2525D symbols that can be created then there are specific 2525C symbols for them, so most symbols built with 2525D will not have corresponding 2525C symbols available.  In some cases, multiple 2525C SIDCs map to a single 2525D SIDC.

Use the /xl switch to specify a file name for exporting all legacy symbols (represented by 2525C SIDCs) to their corresponding 2525D SIDCs.  A remark of "pass" indicates the given 2525C SIDC was used to create a JMSML symbol, and the 2525D SIDC that results was then used to create a second symbol, whose 2525C SIDC is equivalent to the original input.

In other words, this export function takes a 2525C SIDC, converts it to 2525D, then takes that 2525D SIDC and converts it back to arrive at the original 2525C SIDC input.

A remark of pass(multiple) indicates the test was successful, but that in converting 2525D to 2525C, more than one possible 2525C SIDC was the result, one of those results being equal to the original input.

A remark of retired indicates the test resulted in finding that the specified 2525C SIDC has been formally retired by the SSMC, and therefore has no recommended 2525D equivalent.

	jmsml.exe /xl="LegacyMappingTableCtoD"

### Schemas ###

Military feature attribute schemas, which consist of Fields and Subtypes, can be exported from JMMSL data.  These export files are then used with tools in the military features data repository to automatically build a military feature template geodatabase.

    jmsml.exe /xschemas="./military_feature_schemas/"

The test folder [here](../source/JointMilitarySymbologyLibraryCS/jmsml/test) contains the current baseline for these tables.  The export function performs a line by line comparison test between each line of each newly created csv file and each line of the corresponding current baseline file, and creates an annotated version of each file [here](./military_feature_schemas/comparison_test_results/), highlighting any differences.  A product engineer can use these test results to identify deltas in the output and ascertain if those deltas are legitimate and intended differences in versions of that output.