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
    public class ImageFrameExport : FrameExport, IFrameExport
    {
        // Class designed to export Frame elements as image path, name, category, and tag information

        private bool _omitSource = false;

        public ImageFrameExport(ConfigHelper configHelper, bool omitSource)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
        }

        string IFrameExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,notes"; }
        }

        string IFrameExport.Line(Librarian librarian, LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension)
        {
            _notes = "";

            string result = "";
            string graphic = "";

            string graphicPath = _configHelper.GetPath(context.ID, FindEnum.FindFrames);

            LibraryAffiliation affiliation = librarian.Affiliation(context.ID, dimension.ID, identity.ID);

            if (affiliation != null)
            {
                if (affiliation.Shape != ShapeType.NA)
                {
                    graphic = affiliation.Graphic;

                    if (graphic == null)
                        _notes = _notes + "graphic is missing - frame is NA - frame is never to be drawn;";

                    string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic);
                    string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic);

                    if (!File.Exists(itemOriginalPath))
                        _notes = _notes + "image file does not exist;";

                    string itemName = BuildFrameItemName(context, dimension, identity);
                    string itemCategory = "Frame";
                    string itemTags = BuildFrameItemTags(context, identity, dimension, graphicPath + "\\" + graphic, _omitSource);

                    result = itemRootedPath + "," +
                             Convert.ToString(_configHelper.PointSize) + "," +
                             itemName + "," +
                             itemCategory + "," +
                             itemTags + "," +
                             _notes;
                }
            }

            return result;
        }
    }
}
