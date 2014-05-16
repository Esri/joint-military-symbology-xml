Prerequisites

The following procedure is dependent on the existence of manually generated CSV input data. Specifically, SampleEntityTable.csv and SampleModifierTable.csv. These files are built from the tables in Appendix A of 2525 and instructions for creating them are here: [Creating CSV Entity and Modifier files from the Standard](onenote:http://devinfo/SiteDirectory/DefenseAndIntelligence/Operations%20Team/Notebook/Operations%20Team/Symbology.one#Creating%20XML%20Source%20Data%20for%20Andy's%20Repo§ion-id=%7B84D7F293-6815-482C-AF0F-45D575C49983%7D&page-id=%7B3A2775C0-4259-43DC-A5A3-6BE8AA7939BA%7D&object-id=%7BABAC29F7-520C-0E0F-3C2B-0413F84D05E9%7D&4E).

The following import procedure should deal with replacing characters that XML doesn't like, but if they MUST be there, surround those characters/phrases with ![CDATA[XXXXX]]. Characters to avoid inside 2525 and APP-6 text data are:  ' (apostrophe or single quote), & (ampersand), < (less than), > (greater than), and " (double quote).

The following references may be of use: [http://en.wikipedia.org/wiki/List\_of\_XML\_and\_HTML\_character\_entity\_references](http://en.wikipedia.org/wiki/List_of_XML_and_HTML_character_entity_references)

[http://www.dvteclipse.com/documentation/svlinter/How\_to\_use\_special\_characters\_in\_XML.3F.html](http://www.dvteclipse.com/documentation/svlinter/How_to_use_special_characters_in_XML.3F.html)

Preparation

- [ ] 1. Install Microsoft Visual Studio Professional (2012 or later)
- [ ] 2. [Optional] Install Aptana Studio 3
- [ ] 3. Install GitHub for Windows
- [ ] 4. Fork the Military Symbology Repository
- [ ] 5. Clone the Military Symbology Repo locally 
- [ ] 6. Download latest version of MIL-STD-2525 and Symbol SVGs
- [ ] 7. [Optional for Windows 7] Install SVG Viewer Extension for Windows Explorer from here

Creating a new XML

- [ ] 1. Open JointMilitarySymbologyLibrary.sln in Microsoft Visual Studio
- [ ] 2. Navigate to jmsml > Properties > Debug > Start Options > Command line arguments from the Solution Explorer
- [ ] 3. Type /i="SampleEntityTable.csv" /m="SampleModifierTable.csv" /s="X" /lc="Y"1
- [ ] 4. Ensure jmsml is the StartUp Project by right clicking on it in the Solution Explorer and selecting, "Set as StartUp Project".
- [ ] 5. Click Start on the toolbar

Add the new XML to the Solution (Choose only one first step)

- [ ] 1. Formatted [Optional]:
  - [ ] 1. Navigate to /source/JointMilitarySymbologyLibraryCS/jmsml/bin/Debug/ in Windows Explorer
  - [ ] 2. Select SymbolSet_X.xml2
  - [ ] 3. Open in Aptana Studio 3
  - [ ] 4. Select Source > Format from the menu bar
  - [ ] 5. Save as /instance/jmsml_d_Label.xml3
  - [ ] 6. Close Aptana Studio 3
- [ ] 1. Unformatted:
  - [ ] 1. In Windows Explorer, navigate to source/JointMilitarySymbologyLibraryCS/jmsml/bin/Debug/
  - [ ] 2. Move SymbolSet_X.xml4 to /instance and Rename as jmsml_d_Label.xml5
- [ ] 2. Select Solution Items > Add > Existing Item in the Visual Studio Solution Explorer
- [ ] 3. Add any new jmsml_d_Label.xml

Populate the XML with 2525 data

Each XML has seven separate portions: XML data, SymbolSetCode, LegacyCodingSchemeCode, Entities, SectorOneModifiers, SectorTwoModifiers, LegacySymbols

- [ ] 1. XML Data
  - [ ] 1. xmlns="http://disa.mil/JointMilSyML.xsd"
  - [ ] 2. ID="SS_SYMBOL_SET"
  - [ ] 3. Label="Symbol Set"
  - [ ] 4. Dimension="DIMENSION"
- [ ] 2. SymbolSetCode: This comes from MIL-STD-2525D, Table A-III (Page 46).
- [ ] 3. LegacyCodingSchemeCode: This comes from the previous version of MIL-STD-2525 that you are trying to implement legacy support for. In MIL-STD-2525C, it can be found in Table A-I, (Page 51), character position 1. If a Symbol Set is composed of two or more code schemes (i.e. Activities), you may list all of them.
  - [ ] 1. Name="2525X"
- [ ] 4. Entities: Entities are broken down into three components; Entity, EntityType, and EntitySubType. Each one has a corresponding two digit code which are the two identifying numbers from the SIDC, the first two (XX####) for Entity, the middle two for EntityType (##XX##), and the last two for EntitySubType (####XX). For any entry, there should be:
  - [ ] 1. ID: UNIQUE_ID
  - [ ] 2. Label: Description (from the standard). These are in title case in Appendix A, but listed in all-caps in the appendices themselves.
  - [ ] 3. Remarks: Remarks (from the standard). These should be in sentence case.
  - [ ] 4. Graphic: XXXXXXXX.svg (from the DISA Symbols)
  - [ ] 5. Icon: ICON_TYPE (from the standard, defaults to MAIN)
    - [ ] 1. If the entity is a place holder/part of the hierarchy for 2525, the standard will indicate this in the appendices and there will not be an svg file available for it. The Graphic attribute is left off and the Icon attribute is set to "NA" in these cases.
    - [ ] 2. If FULL_FRAME, Graphic must be replaced by four frame-specific graphics6:
      - [ ] 1. CloverGraphic: XXXXXXXX_0.svg
      - [ ] 2. RectangleGraphic: XXXXXXXX_1.svg
      - [ ] 3. SquareGraphic: XXXXXXXX_2.svg
      - [ ] 4. DiamondGraphic: XXXXXXXX_3.svg
  - [ ] 6. GeometryType: POINT, LINE, or AREA (from the standard, defaults to POINT)
  - [ ] 7. Standard: STANDARD_TYPE (MILSTD_2525, NATO_APP6, ALL (default))
- [ ] 5. Sector Modifiers: Broken down into two categories, Sectors One and Two (Top and Bottom, or Left and Right). Each one has a corresponding two digit code which are the two identifying numbers from the SIDC (##XX#). The first two numbers (XX###) signifies the Symbol Set, and the last number (####X) signifies the sector (1 or 2). For any entry, there should be:
  - [ ] 1. ID: UNIQUE_ID
  - [ ] 2. Label: Description (from the standard). Title case.
  - [ ] 3. Remarks: Remarks (from the standard). Sentence case.
  - [ ] 4. Category: Category (from the standard – found in the entrees in the various appendices – not in the tables in Appendix A.
  - [ ] 5. Graphic: XXXXX.svg (from the DISA Symbols)
  - [ ] 6. LimitUseTo: UNIQUE_ID (for some modifiers, use is limited to a specific set of entities. This comes from the standard). These are a string containing a space delimited list of unique IDs, referencing entities, entity types, and entity subtypes elsewhere in the same XML file.
  - [ ] 7. Standard: STANDARD_TYPE (MILSTD_2525, NATO_APP6, ALL (default))
- [ ] 6. LegacySymbols:    This uses the Unique IDs from Entities and Modifiers above to build the proper symbol using old 2525 rules.
  - [ ] 1. ID: UNIQUE_ID
  - [ ] 2. EntityID: UNIQUE_ID (If necessary, omit otherwise)
  - [ ] 3. EntityTypeID: UNIQUE_ID (If necessary, omit otherwise)
  - [ ] 4. EntitySubTypeID: UNIQUE_ID (If necessary, omit otherwise)
  - [ ] 5. ModifierOneID: UNIQUE_ID (If necessary, omit otherwise)
  - [ ] 6. ModifierTwoID: UNIQUE_ID (If necessary, omit otherwise)
  - [ ] 7. Remarks: Remarks. Sentence case.
  - [ ] 8. Standard: STANDARD_TYPE (MILSTD_2525, NATO_APP6, ALL (default))
- [ ] 7. LegacyFunctionCode: This uses the six digit Function ID from the version of MIL-STD-2525 you are using.
  - [ ] 1. Name: 2525X (Depending on the standard you're using)

Exporting to CSV

- [ ] 1. Run ExportAll.bat in /samples/symbolcode_firstten_domains_values

1# Where X = Symbol Set Code, and Y = Legacy Scheme Code
2# Ibid
3# Where Label = The Symbol Set Name
4# Where X = Symbol Set Code, and Y = Legacy Scheme Code
5# Where Label = The Symbol Set Name
6# Other options are CircleGraphic and CurveGraphic, used only if a future icon should be introduced that touches the edges of these unique frame shapes.