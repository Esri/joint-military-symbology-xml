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

namespace JointMilitarySymbologyLibrary
{
    public class LegacyOCAExport : OCAExport, IOCAExport
    {
        // Class designed to export Amplifiers as legacy to 2525D lookups

        private string _standard = "2525C";

        public LegacyOCAExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;
        }

        string IOCAExport.Headers
        {
            get { return "Name,LegacyKey,MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Standard,Status,Notes"; }
        }

        string IOCAExport.Line(LibraryStatus status, LibraryStatusGraphic statusGraphic)
        {
            string result = "";

            LibraryStandardIdentityGroup siGroup = _configHelper.Librarian.StandardIdentityGroup(statusGraphic.StandardIdentityGroup);
            LibraryDimension dimension = _configHelper.Librarian.Dimension(statusGraphic.DimensionID);

            result = BuildOCAItemName(siGroup, dimension, status);
            result = result + "," + BuildSIDCKey(siGroup, dimension, status);
            result = result + "," + BuildOCACode(siGroup, dimension, status);
            result = result + ","; // + "Modifier1";
            result = result + ","; // + "Modifier2";
            result = result + ","; // + "ExtraIcon";
            result = result + ","; // + "FullFrame";
            result = result + "," + "Point"; // + "GeometryType";
            result = result + ","; // + "Standard";
            result = result + ","; // + "Status";
            result = result + ","; // + "Notes";

            return result;
        }

        string IOCAExport.Line(LibraryStatus status)
        {
            string result = "";

            result = BuildOCAItemName(null, null, status);
            result = result + "," + BuildSIDCKey(null, null, status);
            result = result + "," + BuildOCACode(null, null, status);
            result = result + ","; // + "Modifier1";
            result = result + ","; // + "Modifier2";
            result = result + ","; // + "ExtraIcon";
            result = result + ","; // + "FullFrame";
            result = result + "," + "Point"; // + "GeometryType";
            result = result + ","; // + "Standard";
            result = result + ","; // + "Status";
            result = result + ","; // + "Notes";

            return result;
        }
    }
}
