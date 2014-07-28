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
    public class DomainFrameExport : FrameExport, IFrameExport
    {
        // Class designed to export Frame elements as name and value information

        public DomainFrameExport(ConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        string IFrameExport.Headers
        {
            get { return "Name,Value"; }
        }

        string IFrameExport.Line(Librarian librarian, LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension, LibraryStatus status)
        {
            string result = "";

            LibraryAffiliation affiliation = librarian.Affiliation(context.ID, dimension.ID, identity.ID);

            if (affiliation != null)
            {
                if(affiliation.Shape != ShapeType.NA && (status.StatusCode == 0 || affiliation.PlannedGraphic != ""))
                    result = BuildFrameItemName(context, dimension, identity, status) + "," + BuildQuotedFrameCode(context, identity, dimension, status);
            }

            return result;
        }
    }
}
