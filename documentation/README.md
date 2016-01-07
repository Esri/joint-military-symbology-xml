# JMSML Documentation #

The following links will take you to the various documents written to support the design, development, and use of this repo.

**Note: The hyperlinks embedded inside the HTML based documentation below will not display pretty web pages of information.  The HTML based documentation is best viewed on your machine, after you have cloned or otherwise downloaded this repo.** 

## Design Documentation ##
Visit [here](DESIGN_DOC.md) to learn more about the design of JMSML.

## Base Schema Reference ##
The following link will show you the auto-generated HTML documentation for the Library global element, defined [here](https://github.com/Esri/joint-military-symbology-xml/blob/master/schema/base.xsd).

http://htmlpreview.github.io/?https://github.com/Esri/joint-military-symbology-xml/blob/master/documentation/base.html

## Core Schema Reference ##
The following link will show you the auto-generated HTML documentation for the collection of type and attribute definitions, defined [here](https://github.com/Esri/joint-military-symbology-xml/blob/master/schema/core.xsd), commonly used throughout JMSML.

http://htmlpreview.github.io/?https://github.com/Esri/joint-military-symbology-xml/blob/master/documentation/core.html

## SymbolSet Schema Reference ##
The following link will show you the auto-generated HTML documentation for the SymbolSet global element, defined [here](https://github.com/Esri/joint-military-symbology-xml/blob/master/schema/symbolSet.xsd).

http://htmlpreview.github.io/?https://github.com/Esri/joint-military-symbology-xml/blob/master/documentation/symbolSet.html

## Instructions for Importing Data ##
Visit [here](DATA_IMPORT.md) to read about the steps needed to import raw data into JMSML.

## Schema Usage Examples ##
The following examples are included here to give you a head start in using some of the features of JMSML.

**Tags**
Tags can be used by an implementer/developer in a number of ways.  The most common use for Tags is as a list of keywords that can be searched for when an end user is trying to find specific symbols in a symbol library.

Custom Tags can be added to Entities, EntityTypes, or EntitySubTypes in a SymbolSet by forking and modifying the JMSML SymbolSet XML files like so:

	<EntityType ID="BROADCAST_TRANSMITTER_ANTENNAE" Label="Broadcast Transmitter Antennae" Graphic="10110100.svg" Icon="FULL_OCTAGON">
        <EntityTypeCode>
            <DigitOne>0</DigitOne>
            <DigitTwo>1</DigitTwo>
        </EntityTypeCode>
		<Tags>
            <Tag>First tag</Tag>
            <Tag>Second tag</Tag>
        </Tags>
    </EntityType>