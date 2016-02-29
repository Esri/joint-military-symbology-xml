# Key for MIL-STD-X to MIL-STD-2525D ID Mapping Files #
The following columns are included:


The JMSML legacy lookup table, mapping 2525X symbol elements to their nearest equivalent 2525D symbol elements, is created and maintained in this folder.

[The 2525C to 2525D file.](https://github.com/Esri/joint-military-symbology-xml/blob/master/samples/legacy_support/All_ID_Mapping_C_to_D.csv)

Its format of these files consists of the following

- **id** - A sequentially assigned numeric identifier, used to make it easier to refer to a specific line for review, discussion, and/or revision.

- **Name** - The symbol's name as it appears in 2525D.  This represents the latest military doctrine-driven terms used for the object the symbol represents.  When a given MIL-STD-2525X symbol doesn't exist in 2525D, we use the dot separated hierarchy string assigned to that symbol, as documented in the appropriate legacy standard.
- **Key2525C** (or **Key2525BC2**, depending on the contents of this file) - This column contains keys for the various parts of the 2525X symbol we are trying to construct.  The user of this file would parse out and assemble, from a full 15-character SIDC, a number of category-specific patterns, each representing the minimum number of specific characters necessary to find the needed elements of the complete symbol.
- **MainIcon** - The unique ID for a specific main icon symbol element in our 2525D stylx file.  These are the values our 2525X dictionary renderer plug-ins should be returning to the dictionary renderer.
- **Modifier1** - The unique ID for a specific sector one modifier symbol element in our 2525D stylx file.  Sometimes the legacy symbol needs more than just a main icon to be accurately depicted.
- **Modifier2** - The unique ID for a specific sector two modifier symbol element in our 2525D stylx file.  Sometimes the legacy symbol needs more than just a main icon to be accurately depicted.
- **ExtraIcon** - The unique ID for a special or extra symbol element in our 2525D stylx file.  Special entity sub types are the most common example of these.
- **FullFrame** - Indicates whether the main icon touches the edges of the frame.  TRUE if it does, left blank or null if it does not.  If this is TRUE the user of this file should know that there are actually four versions of the specified main icon in our 2525D stylx file.
- **Geometry** - Indicates the type of geometry being symbolized by this object.  Point, Line, or Area.
- **Status** - An optional string which notes any special handling, condition, or (current) known limitation of the specified symbol.  Examples include:
	- **No Icon** - 2525X uses an empty frame to depict this root level symbol so there is no 2525D element for this.
	- **Different Icon** - 2525X and 2525D don't use the same icon.  After all these years, while the symbol may still exist, it doesn't look the same as it did before.  We'll need to do something about this, in time, if we want to legitimately claim to implement 2525X.  For now, the 2525D icon will have to suffice and we'll have to document this as a known limitation.
	- **Moved Modifier** - There is at least one 2525D sector modifier needed for the 2525X symbol to be accurately depicted, but in 2525D that modifier has moved from from one sector to the other sector.
	- **Retired** - All or part of the 2525X symbol no longer exists in 2525D.  We'll need to do something about this, in time, if we want to legitimately claim to implement 2525X.  For now, we'll have to decide on an alternative 2525D symbol, with guidance from SMEs and our own best judgment, and we'll have to document this as a known limitation.
- **Notes** - An optional string that provides additional explanation or information about the entry.  Examples include:
	- **SIGINT** - This entry is a Signals Intelligence symbol.
	- **SOF** - This entry is a Special Operations Forces symbol.
	- **EM** - This entry is an Emergency Management symbol.
	- **SO** - This entry is a Stability Operations symbol.



Key2525C Keys have the following format:

* Frame Icons 

    Frame Icons have the following pattern:

    CADSF 

    C = Coding Scheme Code
    A = Affiliation
    D = Dimension Code (Battle Dimension)
    S = Status
    F = Dash literal("-") "E" "I" / FunctionCode 1 (only exists for S-G- frames)
    
    Ex: SFGP-

* Main Icons

    Main Icons have the following pattern:

    C-D-F23456
	
	C = Coding Scheme Code
    Dash literal("-")
    D = Dimension Code (Battle Dimension)
    Dash literal("-")
    F23456 - Function Code
   
    Ex: S-G-UCI---

* Echelon/Mobility icons

    Echelon/Mobility icons have the following pattern:

    CS-E2

    C = Coding Scheme Code
    S = Standard Identity Code (Affiliation)
    Dash literal("-")
    E - Echelon Char 1 = Character 11: {empty/not there} M N
    2 - Echelon Char 2 = Character 12: Echelon/Mobility
   
    Ex: SH-MO

* HQ/TF/FD Icons

    HQ TF FD Indicators have the following pattern:

    CSD-FH

    C = Coding Scheme Code
    S = Standard Identity Code (Affiliation)
    D = Dimension Code (Battle Dimension)
    Dash literal("-")
    F = Dash literal("-") "E" "I" / FunctionCode 1 (only exists for S-G- frames)
    H = HQ / TF / FD Code
   
    Ex: SHU--D

* Operational Condition icons

    Operational Condition Amplifiers(OCA) and they have the following Key pattern :
    ADS- -or- ADSF
   
    A = Affiliation / Standard Identity Code 
    D = Dimension Code (Battle Dimension)
    S = Status Code
    F = Dash literal("-") "E" "I" / FunctionCode 1 (only exists for S-G- frames)
   
    Ex: SGF-
 