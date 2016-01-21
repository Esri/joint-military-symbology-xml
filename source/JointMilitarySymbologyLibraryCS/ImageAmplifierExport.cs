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
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class ImageAmplifierExport : AmplifierExport, IAmplifierExport
    {
        private bool _omitSource = false;
        private bool _omitLegacy = false;

        public ImageAmplifierExport(ConfigHelper configHelper, bool omitSource, bool omitLegacy)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
            _omitLegacy = omitLegacy;
        }

        string IAmplifierExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,styleItemUniqueId,styleItemGeometryType,notes"; }
        }

        string IAmplifierExport.Line(LibraryAmplifierGroup amplifierGroup, LibraryAmplifierGroupAmplifier amplifier, LibraryAmplifierGroupAmplifierGraphic graphic)
        {
            _notes = "";

            string result = "";
            string category = "";

            FindEnum find = FindEnum.FindEntities;

            switch(amplifierGroup.AmplifierGroupCode)
            {
                case 1:
                case 2:
                    find = FindEnum.FindEchelons;
                    category = "Echelon";
                    break;

                case 3:
                case 4:
                case 5:
                    find = FindEnum.FindMobilities;
                    category = "Mobility";
                    break;

                case 6:
                    find = FindEnum.FindAuxiliaryEquipment;
                    category = "Auxiliary Equipment";
                    break;
            }

            string graphicPath = _configHelper.GetPath("", find);

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic.Graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic.Graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            LibraryStandardIdentityGroup identityGroup = _configHelper.Librarian.StandardIdentityGroup(graphic.StandardIdentityGroup);

            string itemName = BuildAmplifierItemName(amplifierGroup, amplifier, identityGroup);
            string itemCategory = "Amplifier" + _configHelper.DomainSeparator + category;
            string itemTags = BuildAmplifierItemTags(amplifierGroup, amplifier, identityGroup, graphicPath + "\\" + graphic.Graphic, _omitSource, _omitLegacy);
            string itemID = BuildAmplifierCode(amplifierGroup, amplifier, identityGroup);

            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," + 
                     itemID + "," +
                     "Point"+ "," +
                     _notes;

            return result;
        }
    }
}
