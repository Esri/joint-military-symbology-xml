# Importing Raw Data into JMSML #

The following paragraphs document the steps needed to take comma separated raw data and import it and convert it into useful information in JMSML.  Originally, this raw input data was manually "scraped" or copied and pasted from a draft Word document version of 2525D's Appendix A, and then cleaned up and enhanced by hand.  In the future, such raw data might simply be just comma separated text, manually created and maintained, and then imported into JMSML.  The format of 2525's Appendix A, going forward, may of course be very different than it was when this capability was written.

Note that this import function creates a new set of Entity, EntityTypes, and EntitySubTypes from the aforementioned raw data, in a new file (not appended to the existing JMSML xml files).  Additional editing needs to take place, identified below, to make the imported data truly useful.

While it may be useful for some users to understand how this raw data can be imported, it is hoped that future ongoing maintenance of this repository, and its contents, will obviate or minimize the need for importing raw data.  It is anticipated that, moving forward, the export tools will be used far more frequently than the import tools.

## Prerequisites ##

The following procedure is dependent on the existence of manually generated CSV input data. Specifically, SampleEntityTable.csv and SampleModifierTable.csv. These files are built from the tables in Appendix A of 2525 and instructions for creating them follow.

Appendix A of 2525D has tables for each symbol set, entity, and modifier.  The procedure to build csv files for import into JMSML, from this Appendix A information, is:

- [ ] 1. Create an excel spreadsheet
- [ ] 2. Copy and paste the contents of the Appendix A Entity and Modifier tables into Excel
- [ ] 3. Add/populate:  SymbolSet, GeometryType, and modifier columns (as needed)
- [ ] 4. Copy/"flatten" the missing parent fields (See below)
- [ ] 5. Change “,”(commas) to “-“ (dashes) - because of CSV format
- [ ] 6. Fix any non-ASCII special characters that will also be messed up in the csv 
- [ ] 7. Save the Excel File as a .csv


**Format From Standard:**

Military (110000)
 	Fixed Wing (110100)
 	 	Medical Evacuation (MEDEVAC) (110101)
 	 	Attack/Strike (110102)
 	 	Bomber (110103)
 	 	Fighter (110104)
 
**Excel Format (Entities):**


SymbolSet	|Entity		|EntityType		|EntitySubType					|Code	|GeometryType
---------	|-------	|-------		|-------						|-------|-------
01			|Military	|				|								|110000	|Point
01			|Military	|Fixed Wing		| 								|110100	|Point
01			|Military	|Fixed Wing		|Medical Evacuation (MEDEVAC)	|110101	|Point
01			|Military	|Fixed Wing		|Attack/Strike					|110102	|Point
01			|Military	|Fixed Wing		|Bomber							|110103	|Point
01			|Military	|Fixed Wing		|Fighter						|110104	|Point


**Excel Format (Modifiers):**

Name			|SymbolSet	|ModifierNumber	|Category				|Code
-------			|------		|--------		|----------				|----------
Not Applicable	|01			|1				|None					|00
Attack/Strike	|01			|1				|Military Aircraft Type	|01
Bomber			|01			|1				|Military Aircraft Type	|02
Cargo			|01			|1				|Aircraft Type			|03
Fighter			|01			|1				|Military Aircraft Type	|04


The following import procedure should deal with replacing characters that XML doesn't like, but if they MUST be there, surround those characters/phrases with ![CDATA[XXXXX]]. Characters to avoid inside 2525 text data are:  ' (apostrophe or single quote), & (ampersand), < (less than), > (greater than), and " (double quote).

The following references may be of use: [http://en.wikipedia.org/wiki/List\_of\_XML\_and\_HTML\_character\_entity\_references](http://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references)

[http://www.dvteclipse.com/documentation/svlinter/How\_to\_use\_special\_characters\_in\_XML.3F.html](http://www.dvteclipse.com/documentation/svlinter/How_to_use_special_characters_in_XML.3F.html)

## Preparation ##

- [ ] 1. Install Microsoft Visual Studio Professional (2012 or later)
- [ ] 2. [Optional] Install Aptana Studio 3 or another XML editor
- [ ] 3. Install GitHub for Windows
- [ ] 4. Fork this repo
- [ ] 5. Clone this repo locally 
- [ ] 6. Download the latest version of MIL-STD-2525 (for reference)
- [ ] 7. [Optional for Windows 7] Install the SVG Viewer Extension for Windows Explorer

## Creating a new XML ##

- [ ] 1. Open JointMilitarySymbologyLibrary.sln in Microsoft Visual Studio
- [ ] 2. Build the solution (select Build > Build Solution from the menu bar)
- [	] 3. Open a command console/prompt.
- [ ] 4. Navigate to /source/JointMilitarySymbologyLibraryCS/jmsml/bin/Debug/
- [	] 5. Type jmsml.exe /i="**entity_table_csv**" /m="**modifier_table_csv**" /s="**x**" /lc="**y**", where:
	- **entity_table_csv** = path to raw entity data in csv format (SampleEntityTable.csv, for example)
	- **modifier_table_csv** = path to raw modifier data in csv format (SampleModifierTable.csv, for example)
	- **x** = Numeric symbol set code, used to filter out which rows are imported
	- **y** = Single letter legacy coding schema value for the specified symbol set

## Add the new XML to the Solution (Choose only one first step) ##

- [ ] 1. Formatted [Optional]:
  - [ ] 1. Navigate to /source/JointMilitarySymbologyLibraryCS/jmsml/bin/Debug/ in Windows Explorer
  - [ ] 2. Select SymbolSet_**x**.xml, where **x** = the symbol set code
  - [ ] 3. Open in Aptana Studio 3
  - [ ] 4. Select Source > Format from the menu bar
  - [ ] 5. Save as /instance/jmsml_d_**label**.xml, where **label** = the new symbol set name
  - [ ] 6. Close Aptana Studio 3
- [ ] 1. Unformatted:
  - [ ] 1. In Windows Explorer, navigate to source/JointMilitarySymbologyLibraryCS/jmsml/bin/Debug/
  - [ ] 2. Move SymbolSet_**x**.xml (where **x** = the symbol set code) to /instance and rename as jmsml_d_**label**.xml, where **label** = the new symbol set name
- [ ] 2. Select Solution Items > Add > Existing Item in the Visual Studio Solution Explorer

## Populate the XML with 2525 data ##

Each XML has seven separate portions: XML data, SymbolSetCode, LegacyCodingSchemeCode, Entities, SectorOneModifiers, SectorTwoModifiers, LegacySymbols.  A user needs to follow these steps to edit the new xml file, to provide the information not created during the import process.

- [ ] 1. XML Data
  - [ ] 1. xmlns="http://disa.mil/JointMilSyML.xsd"
  - [ ] 2. ID="SS_SYMBOL_SET".  This needs to be a unique ID, matching a SymbolSetRef in the base XML.
  - [ ] 3. Label="Symbol Set".  User-friendly name for this symbol set.
  - [ ] 4. Dimension="DIMENSION".  The ID of the actual dimension that this symbol set belongs to.
- [ ] 2. SymbolSetCode: This comes from MIL-STD-2525D, Table A-III (Page 46).
- [ ] 3. LegacyCodingSchemeCode: This comes from the previous version of MIL-STD-2525 that you are trying to implement legacy support for. In MIL-STD-2525C, it can be found in Table A-I, (Page 51), character position 1. If a Symbol Set is composed of two or more code schemes (i.e. Activities), you may list all of them.
  - [ ] 1. Name="2525C".  Legacy support is currently limited to 2525C.
- [ ] 4. Entities: Entities are broken down into three components; Entity, EntityType, and EntitySubType. Each one has a corresponding two digit code which are the two identifying numbers from the SIDC, the first two (XX####) for Entity, the middle two for EntityType (##XX##), and the last two for EntitySubType (####XX). For any entry, there should be:
  - [ ] 1. ID: UNIQUE_ID.  This needs to be unique within this symbol set.
  - [ ] 2. Label: Description.  Taken from the standard. These are in title case in Appendix A, but listed in all-caps in the appendices themselves.
  - [ ] 3. Remarks: Remarks.  Taken from the standard. These should be in sentence case.
  - [ ] 4. Graphic: XXXXXXXX.svg.  The appropriate svg file.
  - [ ] 5. Icon: ICON_TYPE.  Taken from the standard, defaults to MAIN.
    - [ ] 1. If the entity is a place holder/part of the hierarchy for 2525, the standard will indicate this in the appendices and there will not be an svg file available for it. The Graphic attribute is left off and the Icon attribute is set to "NA" in these cases.
    - [ ] 2. If FULL_FRAME, Graphic must be replaced by four frame-specific graphics (Other options are CircleGraphic and CurveGraphic, used only if a future icon should be introduced that touches the edges of these unique frame shapes.):
      - [ ] 1. CloverGraphic: XXXXXXXX_0.svg
      - [ ] 2. RectangleGraphic: XXXXXXXX_1.svg
      - [ ] 3. SquareGraphic: XXXXXXXX_2.svg
      - [ ] 4. DiamondGraphic: XXXXXXXX_3.svg
  - [ ] 6. GeometryType: POINT, LINE, or AREA.  Taken from the standard, defaults to POINT.
  - [ ] 7. Standard: STANDARD_TYPE (MILSTD_2525, ALL (default))
- [ ] 5. Sector Modifiers: Broken down into two categories, Sectors One and Two (Top and Bottom, or Left and Right). Each one has a corresponding two digit code which are the two identifying numbers from the SIDC (##XX#). The first two numbers (XX###) signifies the Symbol Set, and the last number (####X) signifies the sector (1 or 2). For any entry, there should be:
  - [ ] 1. ID: UNIQUE_ID.  This needs to be unique within this symbol set.
  - [ ] 2. Label: Description. Taken from the standard. Title case.
  - [ ] 3. Remarks: Remarks. Taken from the standard. Sentence case.
  - [ ] 4. Category: Category.  Taken from the standard – found in the entries in the various appendices – not in the tables in Appendix A.
  - [ ] 5. Graphic: XXXXX.svg.  The appropriate svg file.
  - [ ] 6. LimitUseTo: UNIQUE_ID.  For some modifiers, use is limited to a specific set of entities. This comes from the standard. These are a string containing a space delimited list of unique IDs, referencing entities, entity types, and entity subtypes elsewhere in the same XML file.
  - [ ] 7. Standard: STANDARD_TYPE (MILSTD_2525, ALL (default))
- [ ] 6. LegacySymbols:    This uses the Unique IDs from Entities and Modifiers above to build the proper symbol using old 2525 rules.
  - [ ] 1. ID: UNIQUE_ID.  This needs to be unique within this symbol set.
  - [ ] 2. EntityID: UNIQUE_ENTITY_ID (If necessary, omit otherwise)
  - [ ] 3. EntityTypeID: UNIQUE_ENTITYTYPE_ID (If necessary, omit otherwise)
  - [ ] 4. EntitySubTypeID: UNIQUE_ENTITYSUBTYPE_ID (If necessary, omit otherwise)
  - [ ] 5. ModifierOneID: UNIQUE_MOD1_ID (If necessary, omit otherwise)
  - [ ] 6. ModifierTwoID: UNIQUE_MOD2_ID (If necessary, omit otherwise)
  - [ ] 7. Remarks: Remarks. Sentence case.
  - [ ] 8. Standard: STANDARD_TYPE (MILSTD_2525, ALL (default))
- [ ] 7. LegacyFunctionCode: This uses the six digit Function ID from the version of MIL-STD-2525 you are referencing (2525C for now).
  - [ ] 1. Name: 2525C.  2525C is the only one currently supported.

## Additional Schema Help ##

The following documents have been generated from the XML schema and can provide a more formal source of documentation and guidance.

Documentation for the [Core](core.html) schema.
Documentation for the [Base](base.html) (Library) schema.
Documentation for the [SymbolSet](symbolSet.html) schema.

