# joint-military-symbology-xml #

## SVG Files

This folder contains a zip file with all of the SVG files supplied by DISA, for use in implementing MIL-STD-2525.

When unzipping these files for use with the included image conversion utility, please refer to its instructions [here](../source/utilities/image-conversion-utilities/README.md).

## Sections

* [Naming Conventions](#naming)
* [File Details](#details)
* [Licensing](#licensing)

## Naming

All files are named according to select portions of the 20-character Symbol Identification Code (SIDC) in order to give them proper uniqueness and consistency.

For symbol assembly purposes, the following SIDC positions are used to determine what icons are to be used to create the proper symbol layers. Note: all of these icon layers do not apply to all Symbol Sets.

![Image of SIDC](sidc.png)
 
* Frame (Folder: `Frames` - Digits: 4): Uses SIDC positions 3-6.

* Main Icon (Folder: `Appendices\XXX` - Digits: 8): Uses SIDC positions 5-6 and 11-16.
    * Note: For full-frame main icons (main icons that touch the frame), there is an additional suffix depending on the frame that the icon must touch:
    * _0 = Unknown
    * _1 = Friend
    * _2 = Neutral
    * _3 = Hostile

* Modifier 1 (Folder: `Appendices\XXX\mod1` - Digits: 5): Uses SIDC positions 5-6 and 17-18 along with the number 1 at the end.

* Modifier 2 (Folder: `Appendices\XXX\mod2` - Digits: 5): Uses SIDC positions 5-6 and 19-20 along with the number 2 at the end.

* Amplifier (Echelon/Mobility/Towed Array) (Folder: `Amplifier` - Digits: 3): Uses SIDC positions 4 and 9-10.

* Headquarters(HQ)/Task Force(TF)/Feint/Dummy(FD) (Folder: `HQTFFD` - Digits: 4): Uses SIDC positions 4-6 and position 8.

* Status - (TBD)

## Details

This section describes the details of the internal composition of the SVG files, for those developers who wish to make their own changes (TBD).

## Licensing

Copyright 2014 DISA (SSMC Technical Working Group)

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
