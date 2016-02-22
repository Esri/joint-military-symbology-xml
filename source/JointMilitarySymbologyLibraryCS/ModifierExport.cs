/* Copyright 2014 - 2015 Esri
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
    public class ModifierExport
    {
        // The super class for modifier export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        protected string BuildModifierCode(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            // Constructs a string containing the symbol set and modifier codes for a given
            // set of those objects.

            string code = "";

            if(ss != null)
            {
                code = code + Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);
            }

            code = code + Convert.ToString(m.ModifierCode.DigitOne) + Convert.ToString(m.ModifierCode.DigitTwo);

            if (ss != null)
            {
                code = code + modNumber;
            }

            return code;
        }

        protected string BuildQuotedModifierCode(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            // Constructs a quoted string containing the symbol set and modifier codes for a given
            // set of those objects.

            string code = '"' + this.BuildModifierCode(ss, modNumber, m) + '"';

            return code;
        }

        protected string BuildModifierItemName(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            // Constructs a string containing the name of a modifier, where each Label value
            // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
            // of related SymbolSet and modifier.

            //string result = this.BuildModifierItemCategory(ss, modNumber);

            //result = result + _configHelper.DomainSeparator + m.Category.Replace(',', '-') + _configHelper.DomainSeparator + m.Label.Replace(',', '-');

            string result;

            if (m.Category != null && m.Category != "")
            {
                result = ((m.CategoryAlias == "") ? m.Category : m.CategoryAlias) + _configHelper.DomainSeparator + ((m.LabelAlias == "") ? m.Label : m.LabelAlias);
            }
            else
            {
                result = m.LabelAlias == "" ? m.Label : m.LabelAlias;
            }

            result = result.Replace(',', '-');

            return result;
        }

        protected string BuildModifierItemCategory(SymbolSet ss, string modNumber)
        {
            // Contructs the category information for a given SymbolSet and modifier, including the Label 
            // attribute of the SymbolSet and the type of icon being categorized, deperated by the
            // domain separator (usually a colon).

            string result = ss.Label.Replace(',', '-') + _configHelper.DomainSeparator;

            result = result + "Modifier " + modNumber;

            return result;
        }

        protected string BuildModifierItemTags(SymbolSet ss, 
                                               string modNumber, 
                                               ModifiersTypeModifier m, 
                                               bool omitSource,
                                               bool omitLegacy)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given symbol.

            // The information concatenated together for this comes from a given SymbolSet and
            // modifier.  Information includes the Label attributes, geometry
            // type, location of the original graphic file, the code, etc.

            string path = "";
            string typ = "";
            string result = ss.Label.Replace(',', '-');

            result = result + ";" + "Modifier " + modNumber;
            result = result + ";" + m.Label.Replace(',', '-');

            switch (modNumber)
            {
                case "1":
                    path = _configHelper.GetPath(ss.ID, FindEnum.FindModifierOnes, true);
                    typ = "MOD1";
                    break;
                case "2":
                    path = _configHelper.GetPath(ss.ID, FindEnum.FindModifierTwos, true);
                    typ = "MOD2";
                    break;
            }

            result = result + ";" + typ;

            if(!omitLegacy)
                result = result + ";" + _configHelper.SIDCIsNA ;

            if(!omitSource)
                result = result + ";" + path + "\\" + m.Graphic;

            result = result + ";Point";
            result = result + ";" + BuildModifierItemName(ss, modNumber, m);
            result = result + ";" + BuildModifierCode(ss, modNumber, m);

            return result;
        }

        public string NameIt(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string name = "";

            if(ss != null && m != null)
                name = BuildModifierItemName(ss, modNumber, m);

            return name;
        }

        public string CodeIt(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string code = "";

            if (ss != null && m != null)
                code = BuildModifierCode(ss, modNumber, m);

            return code;
        }
    }
}
