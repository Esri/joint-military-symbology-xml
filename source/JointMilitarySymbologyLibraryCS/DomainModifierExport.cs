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
    public class DomainModifierExport : ModifierExport, IModifierExport
    {
        // The class that provides column headers and a constructed line or row of
        // comma separated text containing coded domain values for a given SymbolSet
        // and Modifier within that SymbolSet.

        public DomainModifierExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IModifierExport.Headers
        {
            get { return "Name,Value"; }
        }

        string IModifierExport.Line(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string result = BuildModifierItemName(null, modNumber, m) + ",";

            result = result + BuildModifierCode(null, modNumber, m);

            return result;
        }
    }
}
