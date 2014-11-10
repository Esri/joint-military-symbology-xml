/* Copyright 2014 Esri
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace JointMilitarySymbologyLibrary
{
    public class EntityExport
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        // The super class for entity export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        //protected string[] _iconTypes = {"No Icon",
        //                                 "Center Icon",
        //                                 "Center + 1 Icon",
        //                                 "Center + 2 Icon",
        //                                 "Full Octagon Icon",
        //                                 "Full Frame Icon",
        //                                 "Special Icon"};

        protected string[] _iconTypes = {"No Icon",
                                         "Main Icon",
                                         "Main Icon",
                                         "Main Icon",
                                         "Main Icon",
                                         "Main Icon",
                                         "Main Icon"};

        protected string[] _geometryList = { "NotValid", "Point", "Line", "Area" };

        protected string GeometryIs(SymbolSetEntity e,
                                   SymbolSetEntityEntityType eType,
                                   EntitySubTypeType eSubType)
        {
            string geometry = _geometryList[0];

            if(eSubType != null)
                geometry = _geometryList[(int)eSubType.GeometryType];
            else if(eType != null)
                geometry = _geometryList[(int)eType.GeometryType];
            else if(e != null)
                geometry = _geometryList[(int)e.GeometryType];

            return geometry;
        }

        protected string BuildEntityCode(LibraryStandardIdentityGroup sig,
                                         SymbolSet ss,
                                         SymbolSetEntity e,
                                         SymbolSetEntityEntityType eType,
                                         EntitySubTypeType eSubType)
        {
            // Constructs a string containing the symbol set and entity codes for a given
            // set of those objects.

            string code = "";

            if (ss != null)
            {
                code = code + Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);
            }

            if (e == null && eType == null && eSubType != null)
            {
                // Deal with the special entity sub types as a special case

                code = code + eSubType.EntityCode + eSubType.EntityTypeCode + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            }
            else
            {
                // Almost everything is dealt with below

                code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

                if (eType != null)
                    code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
                else
                    code = code + "00";

                if (eSubType != null)
                    code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
                else
                    code = code + "00";
            }
            
            if (sig != null)
            {
                code = code + sig.GraphicSuffix;
            }

            return code;
        }

        protected string BuildQuotedEntityCode(LibraryStandardIdentityGroup sig,
                                               SymbolSet ss,
                                               SymbolSetEntity e,
                                               SymbolSetEntityEntityType eType,
                                               EntitySubTypeType eSubType)
        {
            // Constructs a quoted string containing the symbol set and entity codes for a given
            // set of those objects.

            string code = '"' + this.BuildEntityCode(sig, ss, e, eType, eSubType) + '"';

            return code;
        }

        protected string BuildEntityItemName(LibraryStandardIdentityGroup sig,
                                             SymbolSet ss,
                                             SymbolSetEntity e,
                                             SymbolSetEntityEntityType eType,
                                             EntitySubTypeType eSubType)
        {
            // Constructs a string containing the name of an entity, where each label value
            // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
            // of related SymbolSet and entity.

            string result = "";

            if (e == null && eType == null && eSubType != null)
            {
                result = "Special Entity Subtypes" + _configHelper.DomainSeparator + eSubType.Label.Replace(',', '-');
            }
            else
            {
                result = (e.LabelAlias == "") ? e.Label : e.LabelAlias;

                if (eType != null)
                {
                    string eTypeLabel = (eType.LabelAlias == "") ? eType.Label : eType.LabelAlias;
                    result = result + _configHelper.DomainSeparator + eTypeLabel.Replace(',', '-');
                }

                if (eSubType != null)
                {
                    string eSubTypeLabel = (eSubType.LabelAlias == "") ? eSubType.Label : eSubType.LabelAlias;
                    result = result + _configHelper.DomainSeparator + eSubTypeLabel.Replace(',', '-');
                }
            }

            if (sig != null)
            {
                result = result + _configHelper.DomainSeparator + sig.Label;
            }
            
            return result;
        }

        protected string BuildEntityItemCategory(SymbolSet ss, IconType iconType, string geometry)
        {
            // Contructs the category information for a given SymbolSet and entity, including the Label 
            // attribute of the SymbolSet and the type of icon being categorized, deperated by the
            // domain separator (usually a colon).
            string result = "";

            if(ss.Geometry == GeometryType.MIXED)
                result =  ss.Label + _configHelper.DomainSeparator + geometry;
            else
                result = ss.Label + _configHelper.DomainSeparator + _iconTypes[(int)iconType];

            return result;
        }

        protected string GrabGraphic(string uGraphic, string fGraphic, string nGraphic, string hGraphic, string gSuffix)
        {
            string graphic = "";

            switch (gSuffix)
            {
                case "_0":
                    graphic = uGraphic;
                    break;

                case "_1":
                    graphic = fGraphic;
                    break;

                case "_2":
                    graphic = nGraphic;
                    break;

                case "_3":
                    graphic = hGraphic;
                    break;
            }

            return graphic;
        }

        protected string BuildEntityItemTags(LibraryStandardIdentityGroup sig,
                                             SymbolSet ss,
                                             SymbolSetEntity e,
                                             SymbolSetEntityEntityType eType,
                                             EntitySubTypeType eSubType,
                                             bool omitSource,
                                             bool omitLegacy)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given symbol.

            // The information concatenated together for this comes from a given SymbolSet and
            // entity (type and sub type).  Information includes the Label attributes, geometry
            // type, location of the original graphic file, the code, etc.

            string result = ss.Label.Replace(',', '-');
            string graphic = "";
            string geometry = "";
            string iType = "NO_ICON";

            string[] xmlTags = null;

            if (e == null && eType == null && eSubType != null)
            {
                result = result + ";" + "Special Entity Subtypes";
            }

            if(e != null)
            {
                result = result + ";" + e.Label.Replace(',', '-');
                iType = Convert.ToString(e.Icon);
            }

            if (eType != null)
            {
                result = result + ";" + eType.Label.Replace(',', '-');
                iType = Convert.ToString(eType.Icon);
            }

            if (eSubType != null)
            {
                result = result + ";" + eSubType.Label.Replace(',', '-');
                iType = Convert.ToString(eSubType.Icon);

                // Add the type of geometry

                geometry = _geometryList[(int)eSubType.GeometryType];

                if (eSubType.Icon == IconType.FULL_FRAME)
                {
                    if (sig != null)
                    {
                        graphic = GrabGraphic(eSubType.CloverGraphic, eSubType.RectangleGraphic, eSubType.SquareGraphic, eSubType.DiamondGraphic, sig.GraphicSuffix);
                    }
                    
                    _notes = _notes + "icon touches frame;";
                }
                else if (eSubType.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = eSubType.Graphic;

                // Grab any custom XML tags that might exist

                xmlTags = eSubType.Tags;
            }
            else if(eType != null)
            {
                // Add the type of geometry

                geometry = _geometryList[(int)eType.GeometryType];

                if (eType.Icon == IconType.FULL_FRAME)
                {
                    if (sig != null)
                    {
                        graphic = GrabGraphic(eType.CloverGraphic, eType.RectangleGraphic, eType.SquareGraphic, eType.DiamondGraphic, sig.GraphicSuffix);
                    }
                    
                    _notes = _notes + "icon touches frame;";
                }
                else if (eType.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = eType.Graphic;

                // Grab any custom XML tags that might exist

                xmlTags = eType.Tags;
            }
            else if(e != null)
            {
                // Add the type of geometry

                geometry = _geometryList[(int)e.GeometryType];

                if (e.Icon == IconType.FULL_FRAME)
                {
                    if (sig != null)
                    {
                        graphic = GrabGraphic(e.CloverGraphic, e.RectangleGraphic, e.SquareGraphic, e.DiamondGraphic, sig.GraphicSuffix);
                    }
                    
                    _notes = _notes + "icon touches frame;";
                }
                else if (e.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = e.Graphic;

                // Grab any custom XML tags that might exist

                xmlTags = e.Tags;
            }

            // Create the unique ID/code for this object

            string code = BuildEntityCode(sig, ss, e, eType, eSubType);

            // If there is a standard identity group, add it

            if (sig != null)
            {
                result = result + ";" + sig.Label;
            }

            // Add any custom XML or export tags that might exist

            result = _configHelper.AddCustomTags(result, code, xmlTags);

            // Add an equivalent 2525C SIDC tag, if one exists

            if (!omitLegacy)
            {
                string sidcTag = BuildLegacySIDCTag(sig, ss, e, eType, eSubType);
                if (sidcTag != "")
                    result = result + ";" + sidcTag;
            }

            // Add the icon's type

            result = result + ";" + iType;

            // Add the svg source

            if (!omitSource)
                result = result + ";" + _configHelper.GetPath(ss.ID, FindEnum.FindEntities, true) + "\\" + graphic;
            
            // Add the three most important pieces of information

            result = result + ";" + geometry;
            result = result + ";" + BuildEntityItemName(sig, ss, e, eType, eSubType);
            result = result + ";" + code;

            if (result.Length > 255)
            {
                // Can't have a tag string greater than 255 in length.
                // Human interaction will be required to resolve these on a case by case basis.

                _notes = _notes + "styleItemTags > 255;";
            }

            return result;
        }

        protected string BuildLegacySIDCTag(LibraryStandardIdentityGroup sig,
                                            SymbolSet ss,
                                            SymbolSetEntity e,
                                            SymbolSetEntityEntityType eType,
                                            EntitySubTypeType eSubType)
        {
            string result = _configHelper.SIDCIsNew;

            if(ss != null && e != null)
            {
                uint partA = 1000000000 + (ss.SymbolSetCode.DigitOne * (uint)100000) + (ss.SymbolSetCode.DigitTwo * (uint)10000);

                if (sig != null)
                {
                    partA = partA + (sig.StandardIdentityGroupCode * (uint)1000000);
                }

                uint partB = (e.EntityCode.DigitOne * (uint)1000000000) + (e.EntityCode.DigitTwo * (uint)100000000);

                if (eType != null)
                {
                    partB = partB + (eType.EntityTypeCode.DigitOne * (uint)10000000) + (eType.EntityTypeCode.DigitTwo * (uint)1000000);
                }

                if (eSubType != null)
                {
                    partB = partB + (eSubType.EntitySubTypeCode.DigitOne * (uint)100000) + (eSubType.EntitySubTypeCode.DigitTwo * (uint)10000);
                }

                Symbol symbol = _configHelper.Librarian.MakeSymbol(new SIDC(partA, partB));

                //
                // If the symbol has a 2525C equivalent, return it
                //

                if (symbol != null)
                {
                    if (symbol.SymbolStatus == SymbolStatusEnum.statusEnumOld)
                    {
                        result = symbol.LegacySIDC;

                        if (sig == null && result.Substring(1, 1) == "P")
                        {
                            result = result.Substring(0, 1) + "*" + result.Substring(2);
                        }

                        if (result.Substring(14, 1) == "X")
                            result = result.Substring(0, 10) + "****X";

                        else if (result.Substring(0, 1) == "W")
                            result = result.Substring(0, 13) + "**";
                        
                        else if (result.Substring(10, 5) == "-----")
                            result = result.Substring(0, 10) + "*****";

                        else if (result.Substring(11, 4) == "----")
                            result = result.Substring(0, 11) + "****";   
                    }
                }
                else
                {
                    logger.Warn("Symbol could not be built from: " + partA + ":" + partB);
                }
            }

            return result;
        }

        public string NameIt(LibraryStandardIdentityGroup sig, SymbolSet ss, SymbolSetEntity e, SymbolSetEntityEntityType eType, EntitySubTypeType eSubType)
        {
            string name = "";

            if(ss != null && e != null)
                name = BuildEntityItemName(sig, ss, e, eType, eSubType);

            return name;
        }
    }
}
