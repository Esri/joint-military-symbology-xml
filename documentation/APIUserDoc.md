# joint-military-symbology-xml #

## API User Documentation ##

### Sections

* [Design FAQ](#design-faq)
* [Schema Reference](#schem-reference)
* [API Reference](#api-reference)
* [Diagnostics](#diagnostics)

### Design FAQ
1. What problem are you solving?
	- This is my reasons
	- Test
1. What is your high-level plan for solving the problem?
1. Why are you doing things this this way? 
1. Is what you are creating adding value? 
1. Will this change behavior?
1. Is there any alternative or easier way? 
1. Is this actually useful?
1. Is what you are doing really worth it?
1. Are there any other dependencies or risks to consider?
1. Is what are doing usable in different environments?

### Schema Reference

### API Reference

### Diagnostics

Before you use a Librarian object to manufacture a Symbol, you can enable logging using the Librarian.IsLogging property.  When you use Librarian.MakeSymbol, detailed diagnostics will be written to a log file found under My Documents/jmsml/logs.  The negative status code illustrated in parentheses in these log files indicates any issues encountered in creating a symbol.  It is a binary coded value, with each bit set if the following is determined for the desired SIDC (and used to summarize this information in the log):

* Bit 1	= "Version Not Found"
* Bit 2	= "Context Not Found"
* Bit 3	= "Dimension Not Found"
* Bit 4	= "Standard Identity Not Found"
* Bit 5	= "Symbol Set Not Found"
* Bit 6	= "Status Not Found"
* Bit 7	= "HQ/TF/Dummy Not Found"
* Bit 8	= "Amplifier Group Not Found"
* Bit 9	= "Amplifier Not Found"
* Bit 10 = "Affiliation Not Found"
* Bit 11 = "Context Amplifier Not Found"
* Bit 12 = "Entity Not Found"
* Bit 13 = "Entity Type Not Found"
* Bit 14 = "Entity SubType Not Found"
* Bit 15 = "Modifier One Not Found"
* Bit 16 = "Modifier Two Not Found"
* Bit 17 = "Legacy Symbol Not Found"

Bits 1 to 12 directly indicate whether the desired SIDC is valid or not, and can be examined programmatically, using Librarian.StatusCode.  If bits 1 to 12 are all clear (zero) then bit 17 indicates the status of older 2525C symbols (whether they are retired of not).  The value of bits 13 to 16 do not necessarily indicate an issue with a given SIDC, but may be used to troubleshoot if a situation occurs where a given SIDC is not producing the results expected.

The status of a given symbol can be summarized by examining its SymbolStatus property.  Values for this property are:

* statusEnumNew = The SIDC used to create this symbol indicates a new symbol in the version of 2525/APP-6 represented by the Librarian object.
* statusEnumOld = The SIDC used to create this symbol indicates a symbol that was in the previous version of 2525/APP-6 and is also in the version of 2525/APP-6 represented by the Librarian object.
* statusEnumRetired = The SIDC used to create this symbol indicates a symbol that was in the previous version of 2525/APP-6 BUT IS NO LONGER in the version of 2525/APP-6 represented by the Librarian object.  It has been "retired".
* statusEnumInvalid = The SIDC used to create this symbol could not be interpreted, resulting in an invalid symbol  This may also occur if the XML instance data is incomplete.  For example, if you try to create a Land Installation symbol before the Land Installation symbol set has been provided in XML form.