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
    public class SimpleModifierExport : ModifierExport, IModifierExport
    {
        // This class implements IModifierExport to export a SymbolSet and a given
        // modifier within that SymbolSet.  The export format is very simple,
        // including the code for the SymbolSet and Modifier and the Label and Category
        // attributes of that Modifier.

        public SimpleModifierExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IModifierExport.Headers
        {
            get { return "SymbolSet,ModifierNumber,Category,Name,Code,UniqueName"; }
        }

        string IModifierExport.Line(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string result = Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);

            result = result + "," + modNumber + ",";

            if (m.Category != null)
                result = result + m.Category.Replace(',', '-') + ",";
            else
                result = result + ",";

            result = result + m.Label.Replace(',', '-') + ",";

            result = result + Convert.ToString(m.ModifierCode.DigitOne) + Convert.ToString(m.ModifierCode.DigitTwo);

            result = result + "," + BuildModifierItemName(ss, modNumber, m);

            return result;
        }
    }
}
