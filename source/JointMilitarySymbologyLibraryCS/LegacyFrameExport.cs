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
    public class LegacyFrameExport : FrameExport, IFrameExport
    {
        // Class designed to export Frame elements as legacy to 2525D lookups

        private string _standard = "2525C";
        private LibraryAffiliation _affiliation;
        private LegacyLetterCodeType _legacyFrame;

        public LegacyFrameExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;
        }

        private string _legacyStatusCode(string standard, LibraryStatus status)
        {
            string result = "";

            foreach (LegacyLetterCodeType lsc in status.LegacyStatusCode)
            {
                if (lsc.Name == standard)
                {
                    result = lsc.Value;
                    break;
                }
            }

            return result;
        }

        string IFrameExport.Headers
        {
            get { return "Name,Key" + _standard + ",MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Status,Notes"; }
        }

        string IFrameExport.Line(Librarian librarian, LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension, LibraryStatus status, bool asCivilian, bool asPlannedCivilian)
        {
            string result = "";

            if (_legacyFrame != null)
            {
                result = BuildFrameItemName(context, dimension, identity, status, false);

                result = result + "," + BuildSIDCKey(_legacyStatusCode(_standard, status), _legacyFrame);

                result = result + "," + BuildFrameCode(context, identity, dimension, status, false);

                result = result + ","; // + "Modifier1";
                result = result + ","; // + "Modifier2";
                result = result + ","; // + "ExtraIcon";
                result = result + ","; // + "FullFrame";
                result = result + "," + "Point"; // + "GeometryType";
                result = result + ","; // + "Status";
                result = result + "," + _legacyFrame.Description; // + "Notes";
            }

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
    }
}
