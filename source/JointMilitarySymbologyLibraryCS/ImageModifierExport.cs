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
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class ImageModifierExport : ModifierExport, IModifierExport
    {
        // Class designed to implement the export of a SymbolSet and one modifier from that SymbolSet
        // by outputting image file path information, its name and the category of icon it falls within,
        // and the tags associated with that SymbolSet and modifier.

        private bool _omitSource = false;

        public ImageModifierExport(ConfigHelper configHelper, bool omitSource)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
        }

        string IModifierExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,notes"; }
        }

        string IModifierExport.Line(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            _notes = "";

            string result = "";
            string graphicPath = "";

            switch (modNumber)
            {
                case "1":
                    graphicPath = _configHelper.GetPath(ss.ID, FindEnum.FindModifierOnes);
                    break;

                case "2":
                    graphicPath = _configHelper.GetPath(ss.ID, FindEnum.FindModifierTwos);
                    break;
            }

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, m.Graphic);
            string itemActualPath = _configHelper.BuildActualPath(graphicPath, m.Graphic);

            if (!File.Exists(itemActualPath))
                _notes = _notes + "image file does not exist;";

            string itemName = BuildModifierItemName(ss, modNumber, m);
            string itemCategory = BuildModifierItemCategory(ss, modNumber);
            string itemTags = BuildModifierItemTags(ss, modNumber, m, _omitSource);

            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," +
                     _notes;

            return result;
        }
    }
}
