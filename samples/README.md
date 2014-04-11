# joint-military-symbology-xml #

## Samples ##

This samples folder contains sample output from the tools included in this repo.  This document explains how these files were generated.

Usage for the jmsml console application is:

![](usage.jpg)

### All_Entities.csv and All_Modifiers.csv ###
Generated using the following jmsml command line arguments, to export all of the current contents of the JMSML library:

	jmsml.exe /x="All"

### Air_Entities.csv and Air_Modifiers.csv ###
Generated using the following jmsml command line arguments, to extract just the contents of the Air symbol set from the JMSML library:

	jmsml.exe /x="Air" /s="^Air$"

### LightAir_Entities.csv and LightAir_Modifiers.csv ###
Generated using the following jmsml command line arguments, to extract just the contents of the Air symbol set, where the label and/or category information contains the word "Light".

	jmsml.exe /x="Air" /s="^Air$" /q="Light"

### Coded Domain Tables ###
The coded domain tables are comma separated files that have been exported from the JMSML library, for use by developers who need a straightforward dump of its contents.  It contains the label attribute and SIDC code for each element in the standard, exported to multiple files.  The /b switch value should be the location (folder) in which you want the files placed.

The optional /e switch causes the domain value export to add Esri specific output to these files.

	jmsml.exe /b=".'
	jmsml.exe /b="." /e

### Coded Domain Entity/Modifier Tables ###
Coded domain tables for entities and modifiers can be generated with or without the same optional filters/regular expressions documented above (with the /s, /q, /a, /l, and /p switches) by specifying the /xd switch.

	jmsml.exe /xd="All_Coded_Domain"
	jmsml.exe /xd="Air_Coded_Domain" /s="^Air$"
