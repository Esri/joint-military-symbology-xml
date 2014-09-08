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
using System.Threading.Tasks;

namespace JointMilitarySymbologyLibrary
{
    public class DomainOCAExport : OCAExport, IOCAExport
    {
        // Class designed to export OCA elements as name and value information

        public DomainOCAExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IOCAExport.Headers
        {
            get { return "Name,Value"; }
        }

        string IOCAExport.Line(LibraryStatus status, LibraryStatusGraphic statusGraphic)
        {
            //LibraryDimension dimension = _configHelper.Librarian.Dimension(statusGraphic.Dimension);
            //LibraryStandardIdentity identity = _configHelper.Librarian.StandardIdentity(statusGraphic.StandardIdentity);
            
            string result = BuildOCAItemName(null, null, status) + "," + BuildQuotedOCACode(null, null, status);

            return result;
        }

        string IOCAExport.Line(LibraryStatus status)
        {
            string result = BuildOCAItemName(null, null, status) + "," + BuildQuotedOCACode(null, null, status);

            return result;
        }
    }
}
