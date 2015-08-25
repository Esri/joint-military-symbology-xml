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
using System.Threading.Tasks;
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class ImageOCAExport : OCAExport, IOCAExport
    {
        private bool _omitSource = false;
        private bool _omitLegacy = false;

        public ImageOCAExport(ConfigHelper configHelper, bool omitSource, bool omitLegacy)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
            _omitLegacy = omitLegacy;
        }

        string IOCAExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,styleItemUniqueId,styleItemGeometryType,notes"; }
        }

        string IOCAExport.Line(LibraryStatus status, LibraryStatusGraphic statusGraphic)
        {
            _notes = "";

            string result = "";

            string graphicPath = _configHelper.GetPath("", FindEnum.FindOCA);

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, statusGraphic.Graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, statusGraphic.Graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            LibraryDimension dimension = _configHelper.Librarian.Dimension(statusGraphic.Dimension);
            LibraryStandardIdentityGroup identity = _configHelper.Librarian.StandardIdentityGroup(statusGraphic.StandardIdentityGroup);

            string itemName = BuildOCAItemName(identity, dimension, status);
            string itemCategory = "Amplifier : Operational Condition";
            string itemTags = BuildOCAItemTags(identity, dimension, status, graphicPath + "\\" + statusGraphic.Graphic, _omitSource, _omitLegacy);
            string itemID = BuildOCACode(identity, dimension, status);

            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," +
                     itemID + "," +
                     "Point" + "," +
                     _notes;

            return result;
        }

        string IOCAExport.Line(LibraryStatus status)
        {
            _notes = "";

            string result = "";

            string graphicPath = _configHelper.GetPath("", FindEnum.FindOCA);

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, status.Graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, status.Graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            string itemName = BuildOCAItemName(null, null, status);
            string itemCategory = "Amplifier : Operational Condition";
            string itemTags = BuildOCAItemTags(null, null, status, graphicPath + "\\" + status.Graphic, _omitSource, _omitLegacy);
            string itemID = BuildOCACode(null, null, status);

            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," +
                     itemID + "," +
                     "Point" + "," +
                     _notes;

            return result;
        }
    }
}
