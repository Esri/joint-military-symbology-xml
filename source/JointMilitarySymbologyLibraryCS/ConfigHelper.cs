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
using NLog;

namespace JointMilitarySymbologyLibrary
{
    public enum FindEnum
    {
        FindEntities,
        FindModifierOnes,
        FindModifierTwos,
        FindFrames,
        FindEchelons,
        FindSpecials,
        FindMobilities,
        FindAuxiliaryEquipment,
        FindHQTFFD,
        FindOCA,
        Find2525C,
        Find2525BC2
    }

    public class ConfigHelper
    {
        // This class provides helper functions to navigate the
        // information in the JMSML config file.

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private Librarian _librarian;
        private JMSMLConfig _configData;
        private JMSMLConfigETLConfig _etlConfig;

        // Dictionaries to hold the path strings for different groups of graphic files

        private Dictionary<string, string> _framePaths = new Dictionary<string, string>();
        private Dictionary<string, string> _entityPaths = new Dictionary<string, string>();
        private Dictionary<string, string> _modifierOnePaths = new Dictionary<string, string>();
        private Dictionary<string, string> _modifierTwoPaths = new Dictionary<string, string>();
        private Dictionary<string, string> _basePaths = new Dictionary<string, string>();

        public ConfigHelper(Librarian librarian)
        {
            _librarian = librarian;
            _configData = librarian.ConfigData;
            _etlConfig = _configData.ETLConfig;

            _findPaths();
        }

        public Librarian Librarian { get { return _librarian; } }
        public GraphicFolderType[] GraphicFolders { get { return _etlConfig.GraphicFolder; } }
        public string GraphicHome { get { return _etlConfig.GraphicHome; } }
        public string GraphicRoot { get { return _etlConfig.GraphicRoot; } }
        public string SVGHome { get { return _etlConfig.SVGHome; } }
        public string GraphicExtension { get { return _etlConfig.GraphicExtension; } }
        public string DomainSeparator { get { return _etlConfig.DomainSeparator; } }
        public string SIDCIsNA { get { return "SIDC_IS_NA"; } }
        public string SIDCIsNew { get { return "NEW_AT_2525D"; } }
        public int PointSize { get { return _etlConfig.PointSize; } set { _etlConfig.PointSize = value; } }

        private bool _splitAndSearch(string listToSplit, string lookingFor)
        {
            // Splits a string into an array of tokens, based on the space as a delimiter.
            // Then finds the string we're looking for in that collection.

            string[] s = listToSplit.Split(' ');
            return s.Contains(lookingFor);
        }

        private bool _findIt(FindEnum findThis, string forThis, GraphicFolderType inThis, string whereWeAre, ref string whereItIs)
        {
            // Recursive function that looks for a particular type of attribute (findThis), belonging to 
            // a particular JMSML entity (forThis) in the specified graphic folder object (inThis).
            // If it's found we return the path we found it at (whereItIs) all the way up the recursion chain,
            // based on a path string we've been building as we recurse (whereWeAre).
            // If we don't find it, we continue with a depth first search until we do find it or run
            // out of graphic folders to search.

            bool foundIt = false;

            if (inThis != null)
            {
                // First look for what we are searching for in this folder
                // If it is here then append this folder's name to our path
                // and return immediately.

                whereWeAre = whereWeAre + "\\" + inThis.Name;

                // Based on what we are looking for, here is where we check
                // the current folder for the attribute value we want.

                switch (findThis)
                {
                    case FindEnum.FindAuxiliaryEquipment:
                        foundIt = inThis.AuxiliaryEquipment;
                        break;

                    case FindEnum.FindEchelons:
                        foundIt = inThis.Echelons;
                        break;

                    case FindEnum.FindEntities:
                        if (inThis.Entities != null)
                        {
                            foundIt = _splitAndSearch(inThis.Entities, forThis);
                        }
                        break;

                    case FindEnum.FindFrames:
                        if (inThis.Frames != null)
                        {
                            foundIt = _splitAndSearch(inThis.Frames, forThis);
                        }
                        break;

                    case FindEnum.FindHQTFFD:
                        foundIt = inThis.HQTFFD;
                        break;

                    case FindEnum.FindMobilities:
                        foundIt = inThis.Mobilities;
                        break;

                    case FindEnum.FindModifierOnes:
                        if (inThis.ModifierOnes != null)
                        {
                            foundIt = _splitAndSearch(inThis.ModifierOnes, forThis);
                        }
                        break;

                    case FindEnum.FindModifierTwos:
                        if (inThis.ModifierTwos != null)
                        {
                            foundIt = _splitAndSearch(inThis.ModifierTwos, forThis);
                        }
                        break;

                    case FindEnum.FindSpecials:
                        foundIt = inThis.Specials;
                        break;

                    case FindEnum.FindOCA:
                        foundIt = inThis.OCA;
                        break;

                    case FindEnum.Find2525C:
                        foundIt = inThis.LegacyStandard == "2525C";
                        break;

                    case FindEnum.Find2525BC2:
                        foundIt = inThis.LegacyStandard == "2525BC2";
                        break;
                }

                if (foundIt)
                {
                    // We found it here so lets determine where we are so
                    // we can return that information up the recursion chain.
                    
                    whereItIs = whereWeAre;
                }
                else
                {
                    // No luck finding it here, so let's look deeper

                    if (inThis.GraphicFolder != null)
                    {
                        foreach (GraphicFolderType childFolder in inThis.GraphicFolder)
                        {
                            foundIt = _findIt(findThis, forThis, childFolder, whereWeAre, ref whereItIs);

                            if (foundIt)
                                break;
                        }
                    }
                }
            }

            return foundIt;
        }

        private void _findPath(FindEnum findThis, string forThis, Dictionary<string, string> recordItHere)
        {
            // Look for a given path, in our configuration file, and if
            // found, set it in a path dictionary for later retrieval.

            string whereItIs = "";
            //bool foundIt = false;

            foreach (GraphicFolderType folder in this.GraphicFolders)
            {
                if (_findIt(findThis, forThis, folder, "", ref whereItIs))
                {
                    recordItHere.Add(forThis, whereItIs);
                    //foundIt = true;
                    break;
                }
            }

            //if(!foundIt)
            //    logger.Warn("Can't find the " + findThis + " config data for " + forThis + " graphics.");
        }

        private void _findPaths()
        {
            // For each context let's look for that context's frame graphics

            foreach (LibraryContext lc in _librarian.Library.Contexts)
            {
                _findPath(FindEnum.FindFrames, lc.ID, _framePaths);
            }

            // For every symbol set we know about we're going to look for the
            // location of its entity and modifier image/graphic files, in our config data.

            foreach (SymbolSet ss in _librarian.SymbolSets)
            {
                _findPath(FindEnum.FindEntities, ss.ID, _entityPaths);
                _findPath(FindEnum.FindModifierOnes, ss.ID, _modifierOnePaths);
                _findPath(FindEnum.FindModifierTwos, ss.ID, _modifierTwoPaths);
            }

            // Find the rest of the paths

            _findPath(FindEnum.FindSpecials, "JMSML_SPECIALS", _basePaths);
            _findPath(FindEnum.FindHQTFFD, "JMSML_HQTFDUMMIES", _basePaths);
            _findPath(FindEnum.FindEchelons, "JMSML_ECHELONS", _basePaths);
            _findPath(FindEnum.FindMobilities, "JMSML_MOBILITIES", _basePaths);
            _findPath(FindEnum.FindAuxiliaryEquipment, "JMSML_AUXILIARY", _basePaths);
            _findPath(FindEnum.FindOCA, "JMSML_OCA", _basePaths);
            _findPath(FindEnum.Find2525C, "JMSML_2525C", _basePaths);
            _findPath(FindEnum.Find2525BC2, "JMSML_2525BC2", _basePaths);
        }

        private string _getPath(string getWhat, Dictionary<string, string> inThis)
        {
            // Retrieve an entry, by ID, from one of the file path
            // dictionaries.

            string result = "";

            try
            {
                result = inThis[getWhat];
            }
            catch (KeyNotFoundException e)
            {
                logger.Error(e.Message + "No graphic path could be found for " + getWhat + ".");
            }

            return result;
        }

        public string GetPath(string getWhat, FindEnum ofType, bool sansLeadingSlash = false)
        {
            // The public entry point for retrieving and returning a file path
            // string from the various Dictionarys that hold them.

            string result = "";

            switch (ofType)
            {
                case FindEnum.FindAuxiliaryEquipment:
                    result = _getPath("JMSML_AUXILIARY", _basePaths);
                    break;

                case FindEnum.FindEchelons:
                    result = _getPath("JMSML_ECHELONS", _basePaths);
                    break;

                case FindEnum.FindEntities:
                    result = _getPath(getWhat, _entityPaths);
                    break;

                case FindEnum.FindFrames:
                    result = _getPath(getWhat, _framePaths);
                    break;

                case FindEnum.FindHQTFFD:
                    result = _getPath("JMSML_HQTFDUMMIES", _basePaths);
                    break;

                case FindEnum.FindMobilities:
                    result = _getPath("JMSML_MOBILITIES", _basePaths);
                    break;

                case FindEnum.FindModifierOnes:
                    result = _getPath(getWhat, _modifierOnePaths);
                    break;

                case FindEnum.FindModifierTwos:
                    result = _getPath(getWhat, _modifierTwoPaths);
                    break;

                case FindEnum.FindSpecials:
                    result = _getPath("JMSML_SPECIALS", _basePaths);
                    break;

                case FindEnum.FindOCA:
                    result = _getPath("JMSML_OCA", _basePaths);
                    break;

                case FindEnum.Find2525C:
                    result = _getPath("JMSML_2525C", _basePaths);
                    break;

                case FindEnum.Find2525BC2:
                    result = _getPath("JMSML_2525BC2", _basePaths);
                    break;
            }

            if (result.Length > 1 && sansLeadingSlash)
                result = result.Substring(1);

            return result;
        }

        public string BuildRootedPath(string path, string graphic)
        {
            // A helper function for combining the config information for
            // the graphic root, the path found, and an element's graphic attribute
            // into a string. Ex: "{Graphic Root}\Appendices\...\1001100000.svg"

            string result = this.GraphicRoot + path + "\\" + graphic;

            result = result.Replace(".svg", "." + this.GraphicExtension);

            return result;
        }

        public string BuildActualPath(string path, string graphic)
        {
            // A helper function for combining the config information for
            // an actual base path, the path found, and an element's graphic attribute
            // into a string. Ex: "C:\Symbols\Appendices\...\1001100000.svg"

            string result = this.GraphicHome + path + "\\" + graphic;

            result = result.Replace(".svg", "." + this.GraphicExtension);

            return result;
        }

        public string BuildOriginalPath(string path, string graphic)
        {
            // A helper function for combining the config information for
            // an actual base path, the path found, and an element's graphic attribute
            // into a string. Ex: "C:\Symbols\Appendices\...\1001100000.svg"

            string result = this.GraphicHome + path + "\\" + graphic;

            return result;
        }

        public string CustomExportTags(string id)
        {
            string result = "";

            foreach(JMSMLConfigETLConfigExportTag tag in _etlConfig.ExportTags)
            {
                if (id == tag.Name)
                    result = result + ";" + tag.Value;
            }

            return result;
        }

        public string AddCustomTags(string tagString, string id, string[] tags)
        {
            string result = tagString;

            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    result = result + ";" + tag;
                }
            }

            string exportTags = CustomExportTags(id);

            if (exportTags != "")
                result = result + exportTags;

            return result;
        }

        public string AmplifierLabelValue(LibraryAmplifierValuesValue value)
        {
            // Creates and returns a line of text to be exported for a given amplifier value.

            // Uses overrides in the jmsml configuration file to deliver custom output.

            string result;
            
            if(value.Label.Contains(','))
                result = "\"" + value.Label + "\"" + "," + value.LabelAlias;
            else
                result = value.Label + "," + value.LabelAlias;

            foreach (JMSMLConfigETLConfigAmplifierValue v in _etlConfig.AmplifierValues)
            {
                if (value.Name == v.ID)
                {
                    result = v.Label + "," + v.Value;
                    break;
                }
            }

            return result;
        }

        public SchemaETL MakeSchemaETL()
        {
            // Create an instance of a light weight SchemaETL object and return it, reusing the one ETLConfig object in memory.

            return new SchemaETL(_etlConfig);
        }

        public LegacyETL MakeLegacyETL()
        {
            // Create an instance of a light weight LegacyETL object and return it, reusing the one ETLConfig object in memory.

            return new LegacyETL(this, _etlConfig);
        }

        public LegacyLetterCodeType LegacyLetter(LegacyLetterCodeType[] letterArray, string standard)
        {
            // Retrieves and returns the first letter code in the given array matching the given standard

            LegacyLetterCodeType letterCode = null;

            foreach(LegacyLetterCodeType letterCodeInArray in letterArray)
            {
                if (letterCodeInArray.Name == standard)
                {
                    letterCode = letterCodeInArray;
                    break;
                }
            }

            return letterCode;
        }

        public LegacyLetterCodeType[] LegacyLetters(LegacyLetterCodeType[] letterArray, string standard)
        {
            // Retrieves and returns an array of all letter codes in the given array matching the given standard

            List<LegacyLetterCodeType> letterCodes = new List<LegacyLetterCodeType>();

            foreach (LegacyLetterCodeType letterCodeInArray in letterArray)
            {
                if (letterCodeInArray.Name == standard)
                {
                    letterCodes.Add(letterCodeInArray);
                }
            }

            return letterCodes.ToArray<LegacyLetterCodeType>();
        }

        public LegacyFunctionCodeType LegacyFunction(LegacyFunctionCodeType[] functionArray, string standard)
        {
            // Retrieves and returns the first function code in the given array matching the given standard

            LegacyFunctionCodeType functionCode = null;

            foreach(LegacyFunctionCodeType functionCodeInArray in functionArray)
            {
                if (functionCodeInArray.Name == standard)
                {
                    functionCode = functionCodeInArray;
                    break;
                }
            }

            return functionCode;
        }

        public LegacyFunctionCodeType[] LegacyFunctions(LegacyFunctionCodeType[] functionArray, string standard)
        {
            // Retrieves and returns an array of all function codes in the given array matching the given standard

            List<LegacyFunctionCodeType> functionCodes = new List<LegacyFunctionCodeType>();

            foreach (LegacyFunctionCodeType functionCodeInArray in functionArray)
            {
                if (functionCodeInArray.Name == standard)
                {
                    functionCodes.Add(functionCodeInArray);
                }
            }

            return functionCodes.ToArray<LegacyFunctionCodeType>();
        }

        public bool IsSpecialEntitySubtype(SymbolSet ss, EntitySubTypeType entitySubType)
        {
            // Determines if the supplied EntitySubType is in fact one of the special entity subtypes in the specified symbol set

            bool isSpecial = false;

            if (ss != null && entitySubType != null)
            {
                if (ss.SpecialEntitySubTypes != null)
                {
                    foreach (EntitySubTypeType subType in ss.SpecialEntitySubTypes)
                    {
                        if (subType.ID == entitySubType.ID)
                        {
                            isSpecial = true;
                            break;
                        }
                    }
                }
            }

            return isSpecial;
        }

        public string MapKey(string standard, string key)
        {
            string result = key;
            //if (key == "G-G-GLC---")
            //    logger.Info("Here");

            if (_etlConfig.KeyMaps != null)
            {
                foreach (JMSMLConfigETLConfigKeyMap keyMap in _etlConfig.KeyMaps)
                {
                    if (standard == keyMap.Standard && key == keyMap.Key)
                    {
                        result = keyMap.Value;
                        break;
                    }
                }
            }

            return result;
        }
    }
}
