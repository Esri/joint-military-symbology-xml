# joint-military-symbology-xml

Joint Military Symbology XML (JointMilSyML or JMSML) is an XML schema, and associated instance data, designed to definitively document the contents of MIL-STD 2525D and NATO STANAG APP-6(C).  

The purpose of defining this schema and populating its associated instance data is four-fold:
  * To aid in the configuration management (CM) of these standards.
  * To provide implementors with a machine-readable form of the contents of these standards.
  * To help support legacy military symbology systems by providing bi-directopnal lookup of SIDCs.
  * To aid in migrating old militay symbology information to the latest version of these standards.

In providing this schema, files of instance data, and sample C# code to navigate same, it is hoped that future defense and intelligence systems will be engineered to take advantage of this technology and, in so doing, accelerate the delivery of new military symbology, reflected in updates to these standards, to warfighters.

![Image of Joint Military Symbology XML]( ScreenShot.JPG "joint-military-symbology-xml" )

## Features

* Schema (xsd) files for:
  * Commonly used types.
  * The base portion (part one) of a military symbol ID code (SIDC).
  * The symbol set portion (part two) of a military symbol ID code (SIDC).

* Instance (xml) files for:
  * The base portion of the two standards.
  * Each symbol set, including all legacy symbols from 2525C and APP-6(B).  This includes:
    * Air symbols.
    * Air Missile symbols.

* A .Net (C#) solution that contains projects with all the code to:
  * Define all the classes needed to import the XML instance data.
  * Navigate the aforementioned classes to perform the following functions:
    * Convert from old style 15-character SIDC to new style 20-digit SIDC.
    * Convert from new style 20-digit SIDC to new style 15-character SIDC.
  * Test the above functions.

## Sections

* [Requirements](#requirements)
* [Instructions](#instructions)
* [NLog](#nlog)
* [Resources](#resources)
* [Issues](#issues)
* [Contributing](#contributing)
* [Licensing](#licensing)


## Requirements

* A text editor of your choice for viewing and/or editing the XML files found under the instance or schema folders.
* To build the .NET Solution source in source\JointMilitarySymbologyLibraryCS you will also need:
    * Visual Studio 2010 or later.
    * NLog (see below)
    * If you do not require the C# library, you may skip this requirement.

## Instructions

### General Help
[New to Github? Get started here.](http://htmlpreview.github.com/?https://github.com/Esri/esri.github.com/blob/master/help/esri-getting-to-know-github.html)

### Getting Started with the solution
* Open and build the Visual Studio Solution at joint-military-symbology-xml\source\JointMilitarySymbologyLibraryCS
    * To use MSBuild to build the solution
        * Open a Visual Studio Command Prompt: Start Menu | Microsoft Visual Studio 2010/2012 | Visual Studio Tools | Developer Command Prompt for VS 2010/2012
        * `cd joint-military-symbology-xml\source\JointMilitarySymbologyLibraryCS
        * `msbuild JointMilitarySymbologyLibrary.sln /property:Configuration=Release`
            * NOTE: if you recieve an error message: `'msbuild' is not recognized` 
            * You may need to add the path the .NET Framework SDK (if multiple SDKs are installed)
            * E.g. `set path=%path%;C:\Windows\Microsoft.NET\Framework\v4.0.30319`
* Use the UnitTests project and the Visual Studio Test tools to run the unit tests provided.  These will test various aspects of the source used to navigate the XML data and convert symbol ID codes (SIDC) from one military symbology standard to another.
* Run the Test project to see a form appear, which you can use to manually test the SIDC conversion.
	* Edit a 15-character 2525C SIDC on the left and click on the right arrow to convert that SIDC to the equivalent 20 digits for 2525D.
	* Edit the 20 digits on the right and click the left arrow to convert that SIDC to the equivalent 15-character code for 2525C.
	* Note that not all symbols are currently supported and not all possible 2525D codes will convert to 2525C codes, or vice versa.  
		* Because of the component construction nature of 2525D, there are many more symbol ID combinations possible in 2525D, combinations that don't convert to 2525C.
	* For some symbols you may see an image appear in the picture box control.  This image is a compound image created by the included JMSML C# project, from data stored in the XML instance files, using the graphic files supplied by DISA for 2525D.
		* Not all of the necessary images are currently included, and/or referenced, at this time.  Work is ongoing between Esri and the DISA SSMC to make sure that developers will be provided with all the image files they will need to create symbols that comply with the military symbology standards.

## NLog

NLog is a free logging platform for .NET, Silverlight, and Windows Phone with rich log routing and management capabilities. It makes it easy to produce and manage high-quality logs for your application regardless of its size or complexity. 

Support for NLog is available via the NLog-Users group. 

NLog is an open source project hosted on GitHub and its source code is available under the terms of a BSD license. 

Binary packages are available via NuGet. The binary for NLog 2.1.0 is provided in the source/Dependencies folder.

If you're interested in joining NLog development efforts, go to the NLog-Development group.

Visit http://nlog-project.org/ for more information.

## Resources

* Learn more about Esri's [ArcGIS for Defense maps and apps](http://resources.arcgis.com/en/communities/defense-and-intelligence/).

## Issues

* Not all of the symbols in MIL-STD 2525D or NATO APP-6(C) are included in this initial release, as not all of the appendices/chapters have been instantiated as XML data.
* The image resources (svg/png) provided by DISA, used to display the individual elements of a complete symbol, are not all present and many are not currently "wired" in to the XML instance data, at this time.

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Esri welcomes contributions from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

## Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

A copy of the license is available in the repository's
[license.txt](license.txt) file.

[](Esri Tags: ArcGIS Defense and Intelligence Joint Military Symbology XML)
[](Esri Language: XML)
