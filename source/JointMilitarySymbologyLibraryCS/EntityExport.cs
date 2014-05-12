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

namespace JointMilitarySymbologyLibrary
{
    public class EntityExport
    {
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

        protected string[] _geometryList = { "Point", "Line", "Area" };

        protected string BuildEntityCode(SymbolSet ss,
                                         SymbolSetEntity e,
                                         SymbolSetEntityEntityType eType,
                                         SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            // Constructs a string containing the symbol set and entity codes for a given
            // set of those objects.

            string code = Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);
            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

            if (eType != null)
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
            else
                code = code + "00";

            if (eSubType != null)
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            else
                code = code + "00";

            return code;
        }

        protected string BuildEntityItemName(SymbolSet ss,
                                             SymbolSetEntity e,
                                             SymbolSetEntityEntityType eType,
                                             SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            // Constructs a string containing the name of an entity, where each label value
            // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
            // of related SymbolSet and entity.

            string result = ss.Label.Replace(',', '-') + _configHelper.DomainSeparator + e.Label.Replace(',', '-');
            
            if (eType != null)
            {
                result = result + _configHelper.DomainSeparator + eType.Label.Replace(',', '-');   
            }
            
            if (eSubType != null)
            {
                result = result + _configHelper.DomainSeparator + eSubType.Label.Replace(',', '-');
            }
            
            return result;
        }

        protected string BuildEntityItemCategory(SymbolSet ss, IconType iconType)
        {
            // Contructs the category information for a given SymbolSet and entity, including the Label 
            // attribute of the SymbolSet and the type of icon being categorized, deperated by the
            // domain separator (usually a colon).

            string result =  ss.Label + _configHelper.DomainSeparator + _iconTypes[(int)iconType];
            
            return result;
        }

        protected string BuildEntityItemTags(SymbolSet ss,
                                             SymbolSetEntity e,
                                             SymbolSetEntityEntityType eType,
                                             SymbolSetEntityEntityTypeEntitySubType eSubType,
                                             bool omitSource)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given symbol.

            // The information concatenated together for this comes from a given SymbolSet and
            // entity (type and sub type).  Information includes the Label attributes, geometry
            // type, location of the original graphic file, the code, etc.

            string result = ss.Label.Replace(',', '-') + ";" + e.Label.Replace(',', '-');
            string graphic = "";
            string geometry = "";

            if (eType != null)
                result = result + ";" + eType.Label.Replace(',', '-');

            if (eSubType != null)
            {
                result = result + ";" + eSubType.Label.Replace(',', '-');

                // Add the type of geometry

                geometry = _geometryList[(int)eSubType.GeometryType];

                // The following is a work around because our symbol system doesn't
                // need the export of multiple SVG files for the same symbol.
                // TODO: handle this differently, but for now, we export the square
                // graphic if there are in fact multiple files.

                if (eSubType.Icon == IconType.FULL_FRAME)
                {
                    graphic = eSubType.SquareGraphic;
                    _notes = _notes + "icon touches frame;";
                }
                else if (eSubType.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = eSubType.Graphic;
            }
            else if(eType != null)
            {
                // Add the type of geometry

                geometry = _geometryList[(int)eType.GeometryType];

                // TODO: handle this differently, but for now, we export the square
                // graphic if there are in fact multiple files.

                if (eType.Icon == IconType.FULL_FRAME)
                {
                    graphic = eType.SquareGraphic;
                    _notes = _notes + "icon touches frame;";
                }
                else if (eType.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = eType.Graphic;
            }
            else if(e != null)
            {
                // Add the type of geometry

                geometry = _geometryList[(int)e.GeometryType];

                // TODO: handle this differently, but for now, we export the square
                // graphic if there are in fact multiple files.

                if (e.Icon == IconType.FULL_FRAME)
                {
                    graphic = e.SquareGraphic;
                    _notes = _notes + "icon touches frame;";
                }
                else if (e.Icon == IconType.NA)
                    graphic = "";
                else
                    graphic = e.Graphic;
            }

            result = result + ";" + geometry;

            if(!omitSource)
                result = result + ";" + _configHelper.GetPath(ss.ID, FindEnum.FindEntities) + "\\" + graphic;

            result = result + ";" + BuildEntityItemName(ss, e, eType, eSubType);
            result = result + ";" + BuildEntityCode(ss, e, eType, eSubType);

            if (result.Length > 255)
            {
                // Can't have a tag string greater than 255 in length.
                // Human interaction will be required to resolve these on a case by case basis.

                _notes = _notes + "styleItemTags > 255;";
            }

            return result;
        }
    }
}
