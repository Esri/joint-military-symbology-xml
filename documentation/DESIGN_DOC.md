# joint-military-symbology-xml #

## API User Documentation ##

### Sections

* [Design FAQ](#design-faq)
* [Schema Reference](#schema-reference)
* [API Reference](#api-reference)
* [Adding a Symbol Set](#adding-a-symbol-set)
* [Adding a Symbol](#adding-a-symbol)
* [Diagnostics](#diagnostics)

### Design FAQ
With the increased use of agile software development techniques and processes, like Sprint/Scrum, we are seeing less emphasis being placed on writing big design documents for projects, in the classic sense.  Instead, we are beginning to ask a series of simpler questions about what, how, and why we are building something.  

We give the design some initial thought and have a big picture in mind, and then we start building stuff.  In each sprint we refine and expand what we've done before and build a little more, in manageable bite sized pieces.

For this project, in the following ten frequently asked design questions, we consider and answer the what, how, and why of JMSML.

#### What problem are we solving?
DISA's Symbology Standards Management Committee (SSMC) and NATO's Joint Symbology Panel (JSP), have both specified a need for an XML representation of the contents of MIL-STD 2525, for the express purposes of:
	
- Aiding in the configuration management (CM) of these standards, by automating/simplifying the production of the documentation for these standards and provide a means to track and version control the evolution of these standards.
- Providing implementors with a machine-readable form of the contents of these standards, so they can extract the information they need to implement military symbology in their own systems.
- Helping support legacy military symbology systems by providing bi-directional lookup of SIDCs.
- Aiding in migrating old military symbology information to the latest version of these standards.
		
The fundamental problem that we are solving with this project is that the SSMC and JSP both lack the resources and knowledge to develop said XML representation, populate the initial instance data for the latest version of their respective military symbology standards, and develop a sample library of source code that can be used to retrieve useful information from that data.
	
Therefore, this project is intended to be a collaborative community effort to provide the SSMC and JSP with the aforementioned schema, instance data, and tools to assist with the use of that instance data, so that the four bulleted goals above can be achieved.

#### What is our high-level plan for solving the problem?
Our high level plan for solving this problem can be broken down into four key steps:
	
- Develop an XML schema (XSD) for the fundamental contents of MIL-STD 2525D.
- Populate, using said schema,  XML instance data to include all the valid components of a military symbol, that is compatible with the tenets set forth in MIL-STD 2525D, including all of the symbol sets set forth in those standards.
- Develop a C# .Net API that demonstrates one means of extracting information from that XML instance data, including unit tests to validate proper operation of that information extraction, and tools and sample applications that facilitate an implementor extracting what he/she needs from the XML data.
- Develop, in time, a set of stylesheets (XSLT) files that will support the formatting of the contents of the XML data into human readable pages.  XSLT (Extensible Stylesheet Language Transformations) is a language for transforming XML documents into other XML documents, or other objects such as HTML for web pages, plain text or into XSL Formatting Objects which can then be converted to PDF, PostScript, and PNG.

#### Why are we doing things this way?
Some organizations have, at various times, developed Excel files, CSV (comma separated values) files, or even complete databases containing parts of these military symbology standards.  While the SSMC and JSP themselves have from time to time used means like these to store military symbology information, they do not currently have a formal, managed, repository in place that meets all of the aforementioned goals.  The use of XML to deliver a repository that does meet these goals has a number of benefits over these "flatter" separated or delimited approaches.

A discussion of the strengths of XML over CSV can be found [here](http://www.devx.com/tips/Tip/25109):

#### Is what we are creating adding value?
There is indeed value in what we are building here.  Our solution will address and fulfill each of our stated goals.  Goals which are not currently met by any single existing system.

Had this project not been initiated, it is highly unlikely that the SSMC or JSP, as government-run committees with no real intrinsic software development budgets, would ever have been in a position to initiate the project themselves.

One of the other major objectives here is to encourage the SSMC and JSP, and their governing government agencies, to take over ownership of this project, or at least become the major contributors to it.  Maintaining the master XML instance data files herein, and cloning and editing them as needed, to ensure all consumers of this GitHub repository have access to the latest definitive military symbology information.  That these XML files become yet another "product" generated by the SSMC and JSP, to accompany and complement the PDF version of the standards and any SVG files released to support the standards.
    
#### Will this change behavior?
We hope it will change lots of behavior.

We hope that the SSMC and JSP will start using this common schema for their data so that the two standards are more and more alike and differences between them are easier to cope with.

We hope that the SSMC and JSP will adopt this repository and the work it contains and become a major contributor, or use a forked version of it as their official XML based specification for the contents of the two standards.

We hope both organizations are inspired by what we do here, and that this drives them to move their standards documentation and its associated SVG files to GitHub as well, so the commercial military symbology implementation community has a single non-government place to go to find definitive information and can help contribute to the standards and make them better.

We hope these self-same commercial military symbology implementation users will modify their existing and future software engineering workflows to injest the XML data in this repository and use it to create whatever they need in their own software architectures, to ensure their end-user software solutions are properly implementing these military symbology standards.

We hope the aforementioned changes in behavior occur so that the warfighter, using these end-user software solutions, are able to use the latest standardized military symbologies, and that their coalition partners and peers in the field with them, soldiers they must collaborate with, are also using the same standardized military symbologies. 

#### Is there any alternative or easier way?
There are always alternative ways.  There are as many alternative ways as there are software developers with the time, money, and wherewithal to take action and help build a solution.

The easiest way would be to abandon this effort completely and let the SSMC and JSP continue on as they have been doing.  Letting the world's military symbology implementors spend lots of money and time implementing military symbology from what they read in a PDF document,  potentially doing so in wildly different and possibly contradicting ways, and delaying the delivery of new symbology to the warfighter who needs it and often proposed it in the first place.

What we propose here is the best way to deliver a solution that:
- Allows for the structured reprsentation of the inherent hiearchies within the symblology standards...
- In a way that permits the SSMC and JSP to perform configuration management on a single offical repository of military symbology information...
- While permitting them to easily and flexibly "stylize" different types of output, including HTML and PDF, document versions of what is in the standards...
- From a single, definitive, and easily maintained library...
- That implementors can use to examine, query, extract, and/or convert for their own purposes...
- In a consistent fashion (using APIs that the military symbology community can collaboratively extend and maintain over time)...
- To deliver new military symbology to the warfighter in less time and for less money.

#### How useful is this solution? 
Any assessment of usefulness is extremely subjective.  There are necessarily two considerations that need to be made when asking this question.  Useful to who and useful in what time frame?

Clearly, usefulness will increase as the above goals are fully achieved.  The goals stated earlier are goals because no current system in the hands of the SSMC or JSP can currently meet them.  There are thousands of symbol components defined in 2525.  Instantiating that data in XML will take some time.  This library of military symbology information will therefore become more useful as that data is added to the system, and the project nears its intended completion.

Once in place the SSMC and JSP will share a common library of symbology information, that in turn can be used to generate large portions of a printed document.  The manual creation/editing of that document today costs the US taxpayers a lot of money as it is very time consuming.  While military symbol implementors wait for each iteration of these standards to be completed, warfighters in need of the symbols in the newest versions of these standards must wait for those implementors to finish their jobs.  Any process that speeds all or part of that military symbology development and delivery cycle up, and reduces the chance that any two systems will implement the same standards differently, is useful and valuable.

This project can deliver that usesfullness and value, if the SSMC and JSP are willing to use and promote it, and implementors are willing to consume it.

As an example of this, our JavaScript based military feature overlay editor, that needs to be designed and developed to modify the symbology content of online military feature services, could use the JMSML data to populate its user interface widgets with the military symbol information needed by an end user. 
 
#### Is what we are doing really worth it?
As stated in the previous paragraphs, developing this project fills several needs that the SSMC and JSP have already agreed they have and that they agree are very important objectives.  

Making the standard and data products the SSMC and JSP produce more relevant, more usable, more consistent, and more timely not only helps implementors and warfighters, as illustrated above, it also helps improve the reputation of the SSMC and JSP. 

This helps justify and validate the existence of these two committees, in the face of serious opposition from some government quarters.

Which in turn insures the continued viability and longevity of MIL-STD 2525.  Without which there would be no agreed joint military symbology standards, which would in turn lead to issues with C2 system interoperability and the accurate and consistent communication and depiction of tomorrow's tactical pictures.
 
#### Are there any other dependencies or risks to consider?
There are no technical dependencies or risks involved in completing this project, as envisioned.  XML is a stable and dependable data format and the tools to parse it are legion.  

What's more, MIL-STD 2525 is nearing the end of a major release (2525D) will soon evolve into APP-6(D), as it absorbs all of the symbology encoding that has laboriously gone in to 2525D.  Therefore, a subsequent release of these standards, that might reflect significant change, is far off.  It is hoped that the SSMC and JSP can be convinced to adopt use and maintenance of this information as they begin development of the next (Change 1) versions of these two standards.

The only real risk in completing the goals of this project are more organizational/political.  The efficacy of this effort may languish over time if the SSMC and JSP do not adopt ownership of it, or contribute in maintaining it at the very least.  Leaving the community to maintain it from version to version of these standards would then become dependent on implementors or other government or commercial consumers of it, those who would find it useful enough to keep up for their own purposes.

The other potential risk is that the sponsoring organization and current owner of this project (Esri) should decide to pull the plug on completing its development and the population of all its XML instance data, before the project's goals have all been met.  This then would mean the effort of completing it would fall on the SSMC, JSP, or other interested and invested members of the military symbology community. 

#### Is what we are doing usable in different environments?
As stated above, this project contains a number of pieces (schema, instance files, C# parser and samples, eventually some sample style sheets).  The core part of this project, however, is really the XML schema (XSD files) and the associated XML instance files which contain the real military symbology information.  The accompanying C# library of code has been developed to help quality control the contents of the XML data, and to make it possible for .Net developers to readily extract common information from that data, for their own needs.

The choice of XML as the means to define and deliver this machine readable military symbology content leads that content to be readily available to many non-C# users.  As we've illustrated, XML is widely accepted as a common flexible data storage format, and a number of parsing technologies in a number of different programming environments make accessing the JMSML data relatively easy.

A discussion of the categorization of various XML processing options can be found [here](http://en.wikipedia.org/wiki/XML#Programming_interfaces).

### Schema Reference

### API Reference

### Adding a Symbol Set

### Adding a Symbol

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
