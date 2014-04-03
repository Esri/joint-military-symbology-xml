# joint-military-symbology-xml #

## Samples ##

This samples folder contains sample output from the tools included in this repo.  This document explains how these files were generated.

Usage for the jmsml console application is:

![](http://i.imgur.com/gPiIq3f.jpg)

### All_Entities.csv and All_Modifiers.csv ###
Generated using the following jmsml command line arguments, to export all of the current contents of the JMSML library:

	jmsml.exe /x="All"

### Air_Entities.csv and Air_Modifiers.csv ###
Generated using the following jmsml command line arguments, to extract just the contents of the Air symbol set from the JMSML library:

	jmsml.exe /x="Air" /s="^Air$"

### LightAir_Entities.csv and LightAir_Modifiers.csv ###
Generated using the following jmsml command line arguments, to extract just the contents of the Air symbol set, where the label and/or category information contains the word "Light".

	jmsml.exe /x="Air" /s="^Air$" /q="Light"


