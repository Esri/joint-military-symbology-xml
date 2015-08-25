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
    public class OCAExport
    {
        // The super class for operational condition amplifier (OCA) export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        protected string BuildOCACode(LibraryStandardIdentityGroup identity, LibraryDimension dimension, LibraryStatus status)
        {
            // Creates the unique idntifier code for a given OCA.
            
            string code = "";

            if (identity != null && dimension != null)
            {
                code = "0" + Convert.ToString(identity.StandardIdentityGroupCode) +
                             Convert.ToString(dimension.DimensionCode.DigitOne) + Convert.ToString(dimension.DimensionCode.DigitTwo) +
                             Convert.ToString(status.StatusCode) + "2";
            }
            else
                code = Convert.ToString(status.StatusCode);

            return code;
        }

        protected string BuildQuotedOCACode(LibraryStandardIdentityGroup identity, LibraryDimension dimension, LibraryStatus status)
        {
            // Creates the quoted unique idntifier code for a given OCA.

            string code = '"' + this.BuildOCACode(identity, dimension, status) + '"';

            return code;
        }

        protected string BuildOCAItemName(LibraryStandardIdentityGroup identity, LibraryDimension dimension, LibraryStatus status)
        {
            // Constructs a string containing the name of an OCA, where each label value
            // is seperated by a DomainSeparator (usually a colon).

            string result = "";

            if (identity != null && dimension != null)
            {
                result = result + ((status.LabelAlias != "") ? status.LabelAlias.Replace(',', '-') : status.Label.Replace(',', '-')) + _configHelper.DomainSeparator;
                result = result + dimension.Label.Replace(',', '-') + _configHelper.DomainSeparator;
                result = result + identity.Label.Replace(',', '-');
            }
            else
                result = (status.LabelAlias != "") ? status.LabelAlias.Replace(',', '-') : status.Label.Replace(',', '-');

            return result;
        }

        protected string BuildOCAItemTags(LibraryStandardIdentityGroup identity, 
                                          LibraryDimension dimension,
                                          LibraryStatus status,
                                          string graphicPath,
                                          bool omitSource,
                                          bool omitLegacy)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given OCA.

            // The information concatenated together for this comes from a given StandardIdentity, Dimension, and Status.
            // Information includes the Label attributes, location of the original graphic file, the code, etc.

            string result = "Operational Condition;";

            if (identity != null && dimension != null)
            {
                result = result + ((status.LabelAlias != "") ? status.LabelAlias.Replace(',', '-') : status.Label.Replace(',', '-')) + ";";
                result = result + dimension.Label.Replace(',', '-') + ";";
                result = result + identity.Label.Replace(',', '-') + ";";
            }
            else
                result = result + ((status.LabelAlias != "") ? status.LabelAlias.Replace(',', '-') : status.Label.Replace(',', '-')) + ";";

            result = result + "OCA;";

            if(!omitLegacy)
                result = result + _configHelper.SIDCIsNA + ";";

            if (!omitSource)
                result = result + graphicPath.Substring(1) + ";";

            result = result + "Point" + ";";
            result = result + BuildOCAItemName(identity, dimension, status) + ";";
            result = result + BuildOCACode(identity, dimension, status);

            return result;
        }

        public string NameIt(LibraryStandardIdentityGroup identity, LibraryDimension dimension, LibraryStatus status)
        {
            string name = "";

            if(status != null)
                name = BuildOCAItemName(identity, dimension, status);

            return name;
        }
    }
}
