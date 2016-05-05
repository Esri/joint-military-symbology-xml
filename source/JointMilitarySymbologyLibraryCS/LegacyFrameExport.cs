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
                if (lsc.Name.Contains(standard))
                {
                    result = lsc.Value;
                    break;
                }
            }

            return result;
        }

        string IFrameExport.Headers
        {
            get { return "Name,LegacyKey,MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Standard,Status,Notes"; }
        }

        string IFrameExport.Line(Librarian librarian, LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension, LibraryStatus status, bool asCivilian, bool asPlannedCivilian)
        {
            string result = "";

            if (_legacyFrame != null)
            {
                result = BuildFrameItemName(context, dimension, identity, status, false);

                result = result + "," + BuildSIDCKey(_legacyStatusCode(_standard, status), _legacyFrame);

                if(_legacyFrame.LimitUseTo == "2525C" || _legacyFrame.LimitUseTo == "")
                    // For 2525C frames or 2525Bc2 frames that are the same we 2525C we use the 2525D icons
                    // (2525C and some 2525Bc2 frames are identical to 2525D)
                    result = result + "," + BuildFrameCode(context, identity, dimension, status, false);
                else
                    // For 2525Bc2 unique frames we use the unique icons that are keyed accordingly.
                    result = result + "," + BuildFrameCode(_legacyStatusCode(_standard, status), _legacyFrame);

                result = result + ","; // + "Modifier1";
                result = result + ","; // + "Modifier2";
                result = result + ","; // + "ExtraIcon";
                result = result + ","; // + "FullFrame";
                result = result + "," + "Point"; // + "GeometryType";

                switch (_legacyFrame.LimitUseTo)
                {
                    case "2525C":
                        result = result + ",C";
                        break;

                    case "2525Bc2":
                        result = result + ",B2";
                        break;

                    default:
                        result = result + ",";
                        break;
                }

                //result = result + "," + _legacyFrame.LimitUseTo; // + "Standard";
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
