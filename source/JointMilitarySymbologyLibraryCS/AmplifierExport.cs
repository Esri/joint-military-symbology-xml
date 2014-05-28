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

            string code = Convert.ToString(identityGroup.StandardIdentityGroupCode) +
                          Convert.ToString(amplifierGroup.AmplifierGroupCode) +
                          Convert.ToString(amplifier.AmplifierCode);

            return code;
        }

        protected string BuildAmplifierItemName(LibraryAmplifierGroup amplifierGroup,
                                                LibraryAmplifierGroupAmplifier amplifier,
                                                LibraryStandardIdentityGroup identityGroup)
        {
            // Constructs a string containing the name of an amplifier, where each label value
            // is seperated by a DomainSeparator (usually a colon).

            string category = "";
            string result = "Amplifier" + _configHelper.DomainSeparator;

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

            result = result + category + _configHelper.DomainSeparator;
            result = result + identityGroup.Label.Replace(',', '-') + _configHelper.DomainSeparator;
            result = result + amplifier.Label.Replace(',', '-');

            return result;
        }

        protected string BuildAmplifierItemTags(LibraryAmplifierGroup amplifierGroup,
                                                LibraryAmplifierGroupAmplifier amplifier,
                                                LibraryStandardIdentityGroup identityGroup,
                                                string graphicPath,
                                                bool omitSource)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given amplifier.

            // The information concatenated together for this comes from a given AmplifierGroup, Amplifier, and StandardIdentityGroup.
            // Information includes the Label attributes, location of the original graphic file, the code, etc.

            string category = "";
            string result = "Amplifier;";

            switch (amplifierGroup.AmplifierGroupCode)
            {
                case 1:
                case 2:
                    category = "Echelon;";
                    break;

                case 3:
                case 4:
                case 5:
                    category = "Mobility;";
                    break;

                case 6:
                    category = "Auxiliary Equipment;";
                    break;
            }

            result = result + category;
            result = result + identityGroup.Label.Replace(',', '-') + ";";
            result = result + amplifier.Label.Replace(',', '-') + ";";

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

            if(!omitSource)
                result = result + graphicPath.Substring(1) + ";";

            result = result + "Point" + ";";
            result = result + BuildAmplifierItemName(amplifierGroup, amplifier, identityGroup) + ";";
            result = result + BuildAmplifierCode(amplifierGroup, amplifier, identityGroup);

            return result;
        }
    }
}

