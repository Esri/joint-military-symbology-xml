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
    public class LegacyAmplifierExport : AmplifierExport, IAmplifierExport
    {
        // Class designed to export Amplifiers as legacy to 2525D lookups

        private string _standard = "2525C";

        public LegacyAmplifierExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;
        }

        string IAmplifierExport.Headers
        {
            get { return "Name,LegacyKey,MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Standard,Status,Notes"; }
        }

        string IAmplifierExport.Line(LibraryAmplifierGroup amplifierGroup, LibraryAmplifierGroupAmplifier amplifier, LibraryAmplifierGroupAmplifierGraphic graphic)
        {
            string result = "";

            LibraryStandardIdentityGroup siGroup = _configHelper.Librarian.StandardIdentityGroup(graphic.StandardIdentityGroup);

            result = BuildAmplifierItemName(amplifierGroup, amplifier, siGroup);
            result = result + "," + BuildSIDCKey(amplifierGroup, amplifier, siGroup);
            result = result + "," + BuildAmplifierCode(amplifierGroup, amplifier, siGroup);
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
