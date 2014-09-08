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

namespace JointMilitarySymbologyLibrary
{
    public class DomainAmplifierExport : AmplifierExport, IAmplifierExport
    {
        // Class designed to export Amplifier elements as name and value information

        public DomainAmplifierExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IAmplifierExport.Headers
        {
            get { return "Name,Value"; }
        }

        string IAmplifierExport.Line(LibraryAmplifierGroup amplifierGroup, LibraryAmplifierGroupAmplifier amplifier, LibraryAmplifierGroupAmplifierGraphic graphic)
        {
            //LibraryStandardIdentityGroup identityGroup = _configHelper.Librarian.StandardIdentityGroup(graphic.StandardIdentityGroup);

            string result = BuildAmplifierItemName(amplifierGroup, amplifier, null) + "," + BuildQuotedAmplifierCode(amplifierGroup, amplifier, null);

            return result;
        }
    }
}
