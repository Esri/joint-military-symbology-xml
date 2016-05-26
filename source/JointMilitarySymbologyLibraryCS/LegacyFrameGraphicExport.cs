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
using System.Threading.Tasks;

namespace JointMilitarySymbologyLibrary
{
    public class LegacyFrameGraphicExport : FrameExport, IFrameExport
    {
        private LibraryAffiliation _affiliation;
        private LegacyLetterCodeType _legacyFrame;

        public LegacyFrameGraphicExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;
        }

        string IFrameExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,styleItemUniqueId,styleItemGeometryType,notes"; }
        }

        private string _legacyStatusCode(string standard, LibraryStatus status)
        {
            string result = "";

            foreach (LegacyLetterCodeType lsc in status.LegacyStatusCode)
            {
                if (lsc.Name.Contains(standard))
                {
                    result = lsc.Value;
                    break;
                }
            }

            return result;
        }

        string IFrameExport.Line(Librarian librarian, LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension, LibraryStatus status, bool asCivilian, bool asPlannedCivilian)
        {
            string result = "";
            string graphic = "";

            _notes = "";

            string graphicPath = _configHelper.GetPath("JMSML_2525BC2", FindEnum.Find2525BC2);

            graphic = _legacyFrame.Graphic;
            if (status.LabelAlias == "Planned")
                graphic = graphic.Substring(0, 3) + "A" + graphic.Substring(4);

            string id = BuildFrameCode(_legacyStatusCode(_standard, status), _legacyFrame);

            string geometryType = "Point";

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic);
            string tags = BuildFrameItemTags(context, identity, dimension, status, graphicPath, true, true, false);

            // Replace the 2525D ID with the 2525B Change 2 ID
            string dCode = BuildFrameCode(context, identity, dimension, status, false);
            tags = tags.Replace(dCode, id); 

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            result = result + itemRootedPath;
            result = result + "," + Convert.ToString(_configHelper.PointSize);
            result = result + "," + id;
            result = result + "," + "Frame";
            result = result + "," + tags;
            result = result + "," + id;
            result = result + "," + geometryType;
            result = result + "," + _notes;

            return result;
        }

        public LibraryAffiliation Affiliation
        {
            set { _affiliation = value; }
        }

        public LegacyLetterCodeType LegacyFrame
        {
            set { _legacyFrame = value; }
        }

        public string IDIt(LibraryStatus status)
        {
            return BuildFrameCode(_legacyStatusCode(_standard, status), _legacyFrame);
        }
    }
}
