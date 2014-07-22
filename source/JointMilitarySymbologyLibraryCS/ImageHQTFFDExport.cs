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
    public class ImageHQTFFDExport : HQTFFDExport, IHQTFFDExport
    {
        // Class designed to implement the export of a HQTFFD
        // by outputting image file path information, its name and the category of icon it falls within,
        // and the tags associated with that HQTFFD.

        private bool _omitSource = false;

        public ImageHQTFFDExport(ConfigHelper configHelper, bool omitSource)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
        }

        string IHQTFFDExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,notes"; }
        }

        string IHQTFFDExport.Line(LibraryHQTFDummy hqTFFD, LibraryHQTFDummyGraphic graphic)
        {
            _notes = "";

            string result = "";

            string graphicPath = _configHelper.GetPath("", FindEnum.FindHQTFFD);

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic.Graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic.Graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            LibraryStandardIdentityGroup identityGroup = _configHelper.Librarian.StandardIdentityGroup(graphic.StandardIdentityGroup);
            LibraryDimension dimension = _configHelper.Librarian.Dimension(graphic.Dimension);

            string itemName = BuildHQTFFDItemName(identityGroup, dimension, hqTFFD);
            string itemCategory = "Amplifier" + _configHelper.DomainSeparator + "HQTFFD";
            string itemTags = BuildHQTFFDItemTags(identityGroup, dimension, hqTFFD, graphicPath + "\\" + graphic.Graphic, _omitSource);

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
