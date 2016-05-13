﻿/* Copyright 2014 - 2015 Esri
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
using System.IO;
using System.Threading.Tasks;

namespace JointMilitarySymbologyLibrary
{
    public class LegacySymbolExport
    {
        private string _standard;
        private ConfigHelper _configHelper;
        private ImageEntityExport _entityExport;
        private ImageModifierExport _modifierExport;

        private SymbolSetEntity _entity = null;
        private SymbolSetEntityEntityType _entityType = null;
        private EntitySubTypeType _entitySubType = null;
        private ModifiersTypeModifier _modifier1 = null;
        private ModifiersTypeModifier _modifier2 = null;

        public LegacySymbolExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;

            _entityExport = new ImageEntityExport(_configHelper, true, true);
            _modifierExport = new ImageModifierExport(_configHelper, true, true);
        }

        private string _buildName(SymbolSet ss, SymbolSetLegacySymbol legacySymbol)
        {
            // Builds a seperated string of components names for all the 2525D elements
            // used to make this legacy symbol

            string result = "";

            Librarian lib = _configHelper.Librarian;

            if (legacySymbol.EntityID != "")
                _entity = lib.Entity(ss, legacySymbol.EntityID);
            else
                _entity = null;

            if (_entity != null && legacySymbol.EntityTypeID != "")
                _entityType = lib.EntityType(_entity, legacySymbol.EntityTypeID);
            else
                _entityType = null;

            if (_entityType != null && legacySymbol.EntitySubTypeID != "")
                _entitySubType = lib.EntitySubType(ss, _entityType, legacySymbol.EntitySubTypeID);
            else
                _entitySubType = null;

            if (legacySymbol.ModifierOneID != "")
                _modifier1 = lib.ModifierOne(ss, legacySymbol.ModifierOneID);
            else
                _modifier1 = null;

            if (legacySymbol.ModifierTwoID != "")
                _modifier2 = lib.ModifierTwo(ss, legacySymbol.ModifierTwoID);
            else
                _modifier2 = null;

            result = _entityExport.NameIt(null, ss, _entity, _entityType, _entitySubType);

            if (_modifier1 != null && _modifier1.Label != "Unspecified" && _modifier1.Label != "Not Applicable")
                result = result + _configHelper.DomainSeparator + _modifierExport.NameIt(ss, "1", _modifier1);

            if (_modifier2 != null && _modifier2.Label != "Unspecified" && _modifier2.Label != "Not Applicable")
                result = result + _configHelper.DomainSeparator + _modifierExport.NameIt(ss, "2", _modifier2);

            return result;
        }

        private string _buildSIDCKey(SymbolSet ss, SymbolSetLegacySymbol legacySymbol, LegacyFunctionCodeType functionCode)
        {
            // Builds a unique key from a select number of characters from an SIDC.
            // The pattern for this is:
            //
            // S-D-FFFFFF
            //
            // Where:
            //
            // S = Schema code
            // D = Dimension code
            // FFFFFF = Function code

            string result = "";

            if (functionCode != null)
            {
                if (functionCode.SchemaOverride == "")
                {
                    LegacyLetterCodeType letterCode = _configHelper.LegacyLetter(ss.LegacyCodingSchemeCode, _standard);
                    if(letterCode != null)
                        result = letterCode.Value;
                }
                else
                    result = functionCode.SchemaOverride;

                result = result + "-";

                Librarian lib = _configHelper.Librarian;
                LibraryDimension dimension = lib.Dimension(ss.DimensionID);

                if (functionCode.DimensionOverride == "")
                {
                    LegacyLetterCodeType letterCode = _configHelper.LegacyLetter(dimension.LegacyDimensionCode, _standard);
                    if (letterCode != null)
                        result = result + letterCode.Value;
                }
                else
                    result = result + functionCode.DimensionOverride;

                result = result + "-";
                result = result + functionCode.Value;
            }

            return result;
        }

        public string Headers
        {
            get { return "Name,LegacyKey,MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Standard,Status,Notes"; }
        }

        public string Line(SymbolSet ss, SymbolSetLegacySymbol legacySymbol, LegacyFunctionCodeType functionCode)
        {
            string result = "";

            _buildName(ss, legacySymbol);

            string name = _entityExport.NameIt(null, ss, legacySymbol, null, functionCode);
            string entityCode = "";
            string extraIcon = "";

            // Special care is needed with special entity subtypes

            bool isEntitySubTypeSpecial = false;

            if (_entitySubType != null)
            {
                if (_configHelper.IsSpecialEntitySubtype(ss, _entitySubType))
                {
                    entityCode = _entityExport.CodeIt(null, ss, _entity, _entityType, null);
                    extraIcon = _entityExport.CodeIt(null, ss, null, null, _entitySubType);
                    isEntitySubTypeSpecial = true;
                }
                else
                    entityCode = _entityExport.CodeIt(null, ss, _entity, _entityType, _entitySubType);
            }
            else
                entityCode = _entityExport.CodeIt(null, ss, _entity, _entityType, _entitySubType);

            string mod1Code = "";
            string mod2Code = "";

            if (_modifier1 != null && _modifier1.Label != "Unspecified" && _modifier1.Label != "Not Applicable")
                mod1Code = _modifierExport.CodeIt(ss, "1", _modifier1);

            if (_modifier2 != null && _modifier2.Label != "Unspecified" && _modifier2.Label != "Not Applicable")
                mod2Code = _modifierExport.CodeIt(ss, "2", _modifier2);

            bool fullFrame = false;
            string geometry = _entityExport.GeometryIt(_entity, _entityType, _entitySubType);

            if (_entitySubType != null && !isEntitySubTypeSpecial)
            {
                fullFrame = _entitySubType.Icon == IconType.FULL_FRAME;
            }
            else if (_entityType != null)
            {
                fullFrame = _entityType.Icon == IconType.FULL_FRAME;
            }
            else if (_entity != null)
            {
                fullFrame = _entity.Icon == IconType.FULL_FRAME;
            }

            string fullFrameOutput = fullFrame ? "TRUE" : "";

            result = name;
            result = result + "," + _buildSIDCKey(ss, legacySymbol, functionCode); // + "LegacyKey";
            result = result + "," + entityCode; // + "MainIcon";
            result = result + "," + mod1Code;  // + "Modifier1";
            result = result + "," + mod2Code; // + "Modifier2";
            result = result + "," + extraIcon; // + "ExtraIcon";
            result = result + "," + fullFrameOutput; // + "FullFrame";
            result = result + "," + geometry; // + "GeometryType";
            result = result + ","; // + "Standard";
            result = result + ","; // + "Status";
            result = result + ","; // + "Notes";

            return result;
        }

        public string Line(SymbolSet ss, SymbolSetLegacySymbol legacySymbol, LegacyEntityType legacyEntity, LegacyFunctionCodeType functionCode)
        {
            string result = "";

            string sidcKey = _buildSIDCKey(ss, legacySymbol, functionCode);

            bool fullFrame = (legacyEntity.Icon == IconType.FULL_FRAME);
            string fullFrameOutput = fullFrame ? "TRUE" : "";

            result = _entityExport.NameIt(null, ss, legacySymbol, legacyEntity, functionCode); //legacyEntity.Label;
            result = result + "," + sidcKey;
            result = result + "," + sidcKey;
            result = result + ",";
            result = result + ",";
            result = result + ",";
            result = result + "," + fullFrameOutput;
            result = result + "," + "Point"; // TODO : Handle this through a modernized form of GeometryIt
            result = result + ",";
            result = result + ",";
            result = result + ",";

            return result;
        }
    }
}
