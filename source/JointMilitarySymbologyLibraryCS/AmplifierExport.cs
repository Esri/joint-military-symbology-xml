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
    public class AmplifierExport
    {
        // The super class for amplifier export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        protected string BuildAmplifierCode(LibraryAmplifierGroup amplifierGroup, 
                                            LibraryAmplifierGroupAmplifier amplifier,
                                            LibraryStandardIdentityGroup identityGroup)
        {
            // Creates the unique idntifier code for a given amplifier.

            string code = "";

            if (identityGroup != null)
            {
                code = Convert.ToString(identityGroup.StandardIdentityGroupCode);
            }

            code = code + Convert.ToString(amplifierGroup.AmplifierGroupCode) + Convert.ToString(amplifier.AmplifierCode);

            return code;
        }

        protected string BuildQuotedAmplifierCode(LibraryAmplifierGroup amplifierGroup,
                                                  LibraryAmplifierGroupAmplifier amplifier,
                                                  LibraryStandardIdentityGroup identityGroup)
        {
            // Creates the quoted unique idntifier code for a given amplifier.

            string code = '"' + this.BuildAmplifierCode(amplifierGroup, amplifier, identityGroup) + '"';

            return code;
        }

        protected string BuildAmplifierItemName(LibraryAmplifierGroup amplifierGroup,
                                                LibraryAmplifierGroupAmplifier amplifier,
                                                LibraryStandardIdentityGroup identityGroup)
        {
            // Constructs a string containing the name of an amplifier, where each label value
            // is seperated by a DomainSeparator (usually a colon).

            string category = "";
            string result = ""; //"Amplifier" + _configHelper.DomainSeparator;  //Removed because thought to be redundant

            switch (amplifierGroup.AmplifierGroupCode)
            {
                case 1:
                case 2:
                    category = "Echelon";
                    break;

                case 3:
                case 4:
                case 5:
                    category = "Mobility";
                    break;

                case 6:
                    category = "Auxiliary Equipment";
                    break;
            }

            //result = result + category + _configHelper.DomainSeparator; //Removed because thought to be redundant
            result = result + amplifier.Label.Replace(',', '-');

            if (identityGroup != null)
            {
                result = result + _configHelper.DomainSeparator;
                result = result + identityGroup.Label.Replace(',', '-');
            }
            
            return result;
        }

        protected string BuildAmplifierItemTags(LibraryAmplifierGroup amplifierGroup,
                                                LibraryAmplifierGroupAmplifier amplifier,
                                                LibraryStandardIdentityGroup identityGroup,
                                                string graphicPath,
                                                bool omitSource,
                                                bool omitLegacy)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given amplifier.

            // The information concatenated together for this comes from a given AmplifierGroup, Amplifier, and StandardIdentityGroup.
            // Information includes the Label attributes, location of the original graphic file, the code, etc.

            string category = "";
            string iType = "";
            string result = "Amplifier;";

            switch (amplifierGroup.AmplifierGroupCode)
            {
                case 1:
                case 2:
                    category = "Echelon;";
                    iType = "ECHELON";
                    break;

                case 3:
                case 4:
                case 5:
                    category = "Mobility;";
                    iType = "MOBILITY";
                    break;

                case 6:
                    category = "Auxiliary Equipment;";
                    iType = "AUXILIARY";
                    break;
            }

            result = result + category;
            result = result + amplifier.Label.Replace(',', '-') + ";";
            result = result + identityGroup.Label.Replace(',', '-') + ";";

            // Loop through standard identities in the group and add them

            foreach(string sIID in identityGroup.StandardIdentityIDs.Split(' '))
            {
                LibraryStandardIdentity si = _configHelper.Librarian.StandardIdentity(sIID);
                if(si != null)
                {
                    if (si.Label != identityGroup.Label)
                        result = result + si.Label.Replace(',', '-') + ";";
                }
            }

            result = result + iType + ";";

            if(!omitLegacy)
                result = result + _configHelper.SIDCIsNA + ";";

            if(!omitSource)
                result = result + graphicPath.Substring(1) + ";";

            result = result + "Point" + ";";
            result = result + BuildAmplifierItemName(amplifierGroup, amplifier, identityGroup) + ";";
            result = result + BuildAmplifierCode(amplifierGroup, amplifier, identityGroup);

            return result;
        }

        public string NameIt(LibraryAmplifierGroup amplifierGroup,
                             LibraryAmplifierGroupAmplifier amplifier,
                             LibraryStandardIdentityGroup identityGroup)
        {
            string name = "";

            if (amplifierGroup != null && amplifier != null)
                name = BuildAmplifierItemName(amplifierGroup, amplifier, identityGroup);

            return name;
        }
    }
}

