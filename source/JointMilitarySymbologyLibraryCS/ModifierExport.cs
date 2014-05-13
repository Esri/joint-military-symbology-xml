using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JointMilitarySymbologyLibrary
{
    public class ModifierExport
    {
        // The super class for modifier export objects.  This class holds
        // properties and methods that are used by child classes.

        protected ConfigHelper _configHelper;
        protected string _notes = "";

        // Constructs a string containing the symbol set and modifier codes for a given
        // set of those objects.

        protected string BuildModifierCode(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string code = Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);
            code = code + Convert.ToString(m.ModifierCode.DigitOne) + Convert.ToString(m.ModifierCode.DigitTwo);
            code = code + modNumber;

            return code;
        }

        // Constructs a string containing the name of a modifier, where each Label value
        // is seperated by a DomainSeparator (usually a colon).  Builds this for each group
        // of related SymbolSet and modifier.

        protected string BuildModifierItemName(SymbolSet ss, string modNumber, ModifiersTypeModifier m)
        {
            string result = this.BuildModifierItemCategory(ss, modNumber);
                
            result = result + _configHelper.DomainSeparator + m.Label;

            return result;
        }

        // Contructs the category information for a given SymbolSet and modifier, including the Label 
        // attribute of the SymbolSet and the type of icon being categorized, deperated by the
        // domain separator (usually a colon).

        protected string BuildModifierItemCategory(SymbolSet ss, string modNumber)
        {
            string result = ss.Label.Replace(',', '-') + _configHelper.DomainSeparator;

            result = result + "Modifier " + modNumber;

            return result;
        }

        // Constructs a string of semicolon delimited tags that users can utilize to search
        // for or find a given symbol.

        // The information concatenated together for this comes from a given SymbolSet and
        // modifier.  Information includes the Label attributes, geometry
        // type, location of the original graphic file, the code, etc.

        protected string BuildModifierItemTags(SymbolSet ss, string modNumber, ModifiersTypeModifier m, bool omitSource)
        {
            string path = "";
            string result = ss.Label.Replace(',', '-');

            result = result + ";" + "Modifier " + modNumber;
            result = result + ";" + m.Label;

            switch (modNumber)
            {
                case "1":
                    path = _configHelper.GetPath(ss.ID, FindEnum.FindModifierOnes);
                    break;
                case "2":
                    path = _configHelper.GetPath(ss.ID, FindEnum.FindModifierTwos);
                    break;
            }

            if(!omitSource)
                result = result + ";" + path + "\\" + m.Graphic;

            result = result + ";" + BuildModifierItemName(ss, modNumber, m);
            result = result + ";" + m.Category;
            result = result + ";" + BuildModifierCode(ss, modNumber, m);

            return result;
        }
    }
}
