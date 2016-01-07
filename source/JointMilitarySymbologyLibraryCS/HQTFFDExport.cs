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

namespace JointMilitarySymbologyLibrary
{
    public class HQTFFDExport
    {
        // The super class for HQTFFD export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        protected string BuildHQTFFDCode(LibraryStandardIdentityGroup identityGroup, LibraryDimension dimension, LibraryHQTFDummy hqTFFD)
        {
            // Creates the unique idntifier code for a given HQTFFD.

            string code = "";

            if (identityGroup != null)
            {
                code = Convert.ToString(identityGroup.StandardIdentityGroupCode);
            }
            
            if(dimension != null)
            {
                code = code + Convert.ToString(dimension.DimensionCode.DigitOne) + Convert.ToString(dimension.DimensionCode.DigitTwo);
            }      
                    
            code = code + Convert.ToString(hqTFFD.HQTFDummyCode);

            return code;
        }

        protected string BuildQuotedHQTFFDCode(LibraryStandardIdentityGroup identityGroup, LibraryDimension dimension, LibraryHQTFDummy hqTFFD)
        {
            // Creates the quoted unique idntifier code for a given HQTFFD.

            string code = '"' + this.BuildHQTFFDCode(identityGroup, dimension, hqTFFD) + '"';

            return code;
        }

        protected string BuildHQTFFDItemName(LibraryStandardIdentityGroup identityGroup, LibraryDimension dimension, LibraryHQTFDummy hqTFFD)
        {
            // Constructs a string containing the name of a HQTFFD, where each label value
            // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
            // of related identity, dimension, and HQTFFD.

            string result = "";  //"HQTFFD" + _configHelper.DomainSeparator;  // Removed because thought to be redundant

            string hqTFFDLabel = (hqTFFD.LabelAlias == "") ? hqTFFD.Label : hqTFFD.LabelAlias;
            result = result + hqTFFDLabel.Replace(',', '-');

            if (identityGroup != null && dimension != null)
            {
                result = result + _configHelper.DomainSeparator;
                result = result + dimension.Label.Replace(',', '-') + _configHelper.DomainSeparator;
                result = result + identityGroup.Label.Replace(',', '-');
            }
            
            return result;
        }

        protected string BuildHQTFFDItemTags(LibraryStandardIdentityGroup identityGroup,
                                             LibraryDimension dimension, 
                                             LibraryHQTFDummy hqTFFD, 
                                             string graphicPath, 
                                             bool omitSource,
                                             bool omitLegacy)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given symbol.

            // The information concatenated together for this comes from a given HQTFFD, Dimension, and Standard Identity.
            // Information includes the Label attributes, location of the original graphic file, the code, etc.

            string result = "HQTFFD;";
            result = result + hqTFFD.Label.Replace(',', '-') + ";";
            result = result + dimension.Label.Replace(',', '-') + ";";
            result = result + identityGroup.Label.Replace(',', '-') + ";";
            
            // Loop through standard identities in the group and add them

            foreach (string sIID in identityGroup.StandardIdentityIDs.Split(' '))
            {
                LibraryStandardIdentity si = _configHelper.Librarian.StandardIdentity(sIID);
                if (si != null)
                {
                    if (si.Label != identityGroup.Label)
                        result = result + si.Label.Replace(',', '-') + ";";
                }
            }

            // Loop through each symbol set in the dimension and add any labels from those

            if (dimension.SymbolSets != null)
            {
                foreach (LibraryDimensionSymbolSetRef ssRef in dimension.SymbolSets)
                {
                    if (ssRef.Label != dimension.Label)
                        result = result + ssRef.Label.Replace(',', '-') + ";";
                }
            }

            result = result + "HQTFFD;";

            if(!omitLegacy)
                result = result + _configHelper.SIDCIsNA + ";";

            if (!omitSource)
                result = result + graphicPath.Substring(1) + ";";

            result = result + "Point;";
            result = result + BuildHQTFFDItemName(identityGroup, dimension, hqTFFD) + ";";
            result = result + BuildHQTFFDCode(identityGroup, dimension, hqTFFD);

            return result;
        }

        public string NameIt(LibraryStandardIdentityGroup identityGroup, LibraryDimension dimension, LibraryHQTFDummy hqTFFD)
        {
            string name = "";

            if(dimension != null && hqTFFD != null)
                name = BuildHQTFFDItemName(identityGroup, dimension, hqTFFD);

            return name;
        }
    }
}
