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
    public class FrameExport
    {
        // The super class for frame export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        protected string BuildFrameCode(LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension)
        {
            string code = Convert.ToString(context.ContextCode) +
                          Convert.ToString(identity.StandardIdentityCode) +
                          Convert.ToString(dimension.DimensionCode.DigitOne) + Convert.ToString(dimension.DimensionCode.DigitTwo);

            return code;
        }

        protected string BuildFrameItemName(LibraryContext context, LibraryDimension dimension, LibraryStandardIdentity identity)
        {
            // Constructs a string containing the name of a frame, where each label value
            // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
            // of related contexts, standard identities, and symbolsets.

            string result = "Frame" + _configHelper.DomainSeparator;

            result = result + context.Label + _configHelper.DomainSeparator;
            result = result + identity.Label + _configHelper.DomainSeparator;
            result = result + dimension.Label + _configHelper.DomainSeparator;
            result = result + BuildFrameCode(context, identity, dimension);

            return result;
        }

        protected string BuildFrameItemTags(LibraryContext context, LibraryStandardIdentity identity, LibraryDimension dimension, string graphicPath, bool omitSource)
        {
            // Constructs a string of semicolon delimited tags that users can utilize to search
            // for or find a given symbol.

            // The information concatenated together for this comes from a given SymbolSet, Context, and Standard Identity.
            // Information includes the Label attributes, location of the original graphic file, the code, etc.

            string result = "Frame;";
            result = result + context.Label + ";";
            result = result + identity.Label + ";";
            result = result + dimension.Label + ";";

            // Loop through each symbol set in the dimension and add any labels from those

            if (dimension.SymbolSets != null)
            {
                foreach (LibraryDimensionSymbolSetRef ssRef in dimension.SymbolSets)
                {
                    if (ssRef.Label != dimension.Label)
                        result = result + ssRef.Label + ";";
                }
            }

            if(!omitSource)
                result = result + graphicPath.Substring(1) + ";";

            result = result + "Point" + ";";
            result = result + BuildFrameItemName(context, dimension, identity) + ";";
            result = result + BuildFrameCode(context, identity, dimension);

            return result;
        }
    }
}
