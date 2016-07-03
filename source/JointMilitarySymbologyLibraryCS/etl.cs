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
using System.IO;
using System.Xml.Serialization;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace JointMilitarySymbologyLibrary
{
    public enum ETLExportEnum
    {
        ETLExportSimple,
        ETLExportDomain,
        ETLExportImage
    }

    public class ETL
    {
        // ETL serves as the primary Extract, Transform, and Load class for
        // importing raw simple CSV into raw JMSML Symbol Set XML, and more
        // importantly, exporting all or some of the contents of the JMSML
        // XML to CSV files.

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private Librarian _librarian;
        private Library _library;
        private List<SymbolSet> _symbolSets;
        private ConfigHelper _configHelper;

        public ETL(Librarian librarian)
        {
            _librarian = librarian;
            _library = _librarian.Library;
            _symbolSets = _librarian.SymbolSets;

            _configHelper = new ConfigHelper(_librarian);
        }

        private void _exportAmplifierValues(string path, LibraryAmplifierValues values, bool append, bool isFirst)
        {
            // Exports a single list of amplifier values into a single CSV.  Appends those to one CSV
            // if that option is selected.

            string line;

            if (append)
            {
                path = path + ".csv";
                line = "Type,Name,Value";
            }
            else
            {
                path = path + "_" + values.LabelAlias + ".csv";
                line = "Name,Value";
            }

            using (var w = new StreamWriter(path, append))
            {
                if (!append || isFirst)
                {
                    w.WriteLine(line);
                    w.Flush();
                }

                foreach (LibraryAmplifierValuesValue value in values.Value)
                {
                    line = "";

                    if (append)
                        line = values.LabelAlias + ",";

                    line = _configHelper.AmplifierLabelValue(value);

                    w.WriteLine(line);
                    w.Flush();
                }
            }

        }

        private void _exportSymbolSetCodes(string path)
        {
            // Exports a single list of symbol set code values used in a given schema, into a single domain range CSV file.

            string line;
            string outPath;

            JMSMLConfigETLConfigSchemaContainer container = _configHelper.MakeSchemaETL().ETLConfig.SchemaContainer;
            JMSMLConfigETLConfigSchemaContainerSchemas[] schemas = container.Schemas;

            foreach (JMSMLConfigETLConfigSchemaContainerSchemas schemasInstance in schemas)
            {
                foreach (JMSMLConfigETLConfigSchemaContainerSchemasSchema schema in schemasInstance.Schema)
                {
                    outPath = path + "_" + schema.DomainName + ".csv";
                    line = "Name,Value";

                    bool deleteIt = false;

                    using (var w = new StreamWriter(outPath))
                    {
                        w.WriteLine(line);
                        w.Flush();

                        string[] ss = schema.SymbolSetIDs.Split(' ');

                        // NOTE: We decided to keep the domains with only one value.  Should that decision ever change then the
                        // next line needs to be edited so that the 0 becomes a 1.

                        if (ss.Length > 0)
                        {
                            foreach (string symSetID in ss)
                            {
                                line = "";

                                SymbolSet symbolSet = _librarian.SymbolSet(symSetID);

                                if (symbolSet != null)
                                {
                                    if(symbolSet.SymbolSetCode.DigitOne == 0)
                                        line = symbolSet.Label + "," + Convert.ToString(symbolSet.SymbolSetCode.DigitTwo);
                                    else
                                        line = symbolSet.Label + "," + Convert.ToString(symbolSet.SymbolSetCode.DigitOne) + Convert.ToString(symbolSet.SymbolSetCode.DigitTwo);

                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                        else
                            deleteIt = true;
                    }

                    if (deleteIt)
                        File.Delete(outPath);
                }
            }
        }

        private void _exportAmplifierValueDomains(string path, bool append = false)
        {
            // Exports each of the lists of values associated with amplifiers in a library
            // into its own CSV file, for use in establishing domain ranges in a system.

            bool isFirst = true;

            foreach (LibraryAmplifier amplifier in _library.Amplifiers)
            {
                if (amplifier.Values != null)
                {
                    foreach (LibraryAmplifierValues values in amplifier.Values)
                    {
                        _exportAmplifierValues(path, values, append, isFirst);

                        isFirst = false;
                    }
                }
            }

        }

        private void _exportSpecialEntities(ETLExportEnum exportType, IEntityExport exporter, SymbolSet s, StreamWriter w, EntitySubTypeType[] array)
        {
            // Exports special entities to a CSV file

            string line = "";

            if (array != null)
            {
                foreach (EntitySubTypeType eSubType in array)
                {
                    if (eSubType.Icon == IconType.SPECIAL && exportType == ETLExportEnum.ETLExportImage)
                    {
                        foreach (LibraryStandardIdentityGroup sig in _library.StandardIdentityGroups)
                        {
                            line = string.Format("{0}", exporter.Line(sig, s, eSubType));

                            w.WriteLine(line);
                            w.Flush();
                        }
                    }
                    else
                    {
                        if (exportType == ETLExportEnum.ETLExportDomain)
                            line = string.Format("{0}", exporter.Line(eSubType));
                        else
                            line = string.Format("{0}", exporter.Line(null, s, eSubType));

                        w.WriteLine(line);
                        w.Flush();
                    }
                }
            }
        }

        private void _exportEntities(ETLExportEnum exportType, IEntityExport exporter, string path, string specialPath, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, bool append = false)
        {
            // Exports entity, entity types, and entity sub types to CSV, by optionally testing a 
            // regular expression against the Label attributes of the containing symbol sets
            // and of the entitites in those symbol sets.  It also allows filtering on geometry,
            // so only point, line, or area symbols can be exported.

            // This method accepts an exporter, a light weight object that knows what column
            // headings to return and how to compose a CSV line of output from the data its
            // provided.

            using (var w = new StreamWriter(path))
            {
                var line = string.Format("{0}", exporter.Headers);

                w.WriteLine(line);
                w.Flush();

                // Initialize a counter to track lines written out
                int rowCount = 0;

                foreach (SymbolSet s in _symbolSets)
                {
                    if (symbolSetExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(s.Label, symbolSetExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        continue;

                    foreach (SymbolSetEntity e in s.Entities)
                    {
                        if (!(exporter is ImageEntityExport) || e.EntityCode.DigitOne != 0 || e.EntityCode.DigitTwo != 0)
                        {
                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(e.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                if (exportPoints && e.GeometryType == GeometryType.POINT ||
                                    exportLines && e.GeometryType == GeometryType.LINE ||
                                    exportAreas && e.GeometryType == GeometryType.AREA ||
                                    (e.EntityCode.DigitOne == 0 && e.EntityCode.DigitTwo == 0))
                                {
                                    // If the icon is Full Frame then four lines need to be exported, to reflect the four icon shapes.
                                    // Else just write out one line for non-Full-Frame.

                                    if (e.Icon == IconType.FULL_FRAME && exportType == ETLExportEnum.ETLExportImage)
                                    {
                                        foreach (LibraryStandardIdentityGroup sig in _library.StandardIdentityGroups)
                                        {
                                            line = string.Format("{0}", exporter.Line(sig, s, e, null, null));

                                            w.WriteLine(line);
                                            w.Flush();

                                            rowCount++;
                                        }
                                    }
                                    else
                                    {
                                        line = string.Format("{0}", exporter.Line(null, s, e, null, null));

                                        w.WriteLine(line);
                                        w.Flush();

                                        rowCount++;
                                    }
                                }
                            }

                            if (e.EntityTypes != null)
                            {
                                foreach (SymbolSetEntityEntityType eType in e.EntityTypes)
                                {
                                    if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                    {
                                        if (exportPoints && eType.GeometryType == GeometryType.POINT ||
                                            exportLines && eType.GeometryType == GeometryType.LINE ||
                                            exportAreas && eType.GeometryType == GeometryType.AREA)
                                        {
                                            // If the icon is Full Frame then four lines need to be exported, to reflect the four icon shapes.
                                            // Else just write out one line for non-Full-Frame.

                                            if (eType.Icon == IconType.FULL_FRAME && exportType == ETLExportEnum.ETLExportImage)
                                            {
                                                foreach (LibraryStandardIdentityGroup sig in _library.StandardIdentityGroups)
                                                {
                                                    line = string.Format("{0}", exporter.Line(sig, s, e, eType, null));

                                                    w.WriteLine(line);
                                                    w.Flush();

                                                    rowCount++;
                                                }
                                            }
                                            else
                                            {
                                                line = string.Format("{0}", exporter.Line(null, s, e, eType, null));

                                                w.WriteLine(line);
                                                w.Flush();

                                                rowCount++;
                                            }
                                        }
                                    }

                                    if (eType.EntitySubTypes != null)
                                    {
                                        foreach (EntitySubTypeType eSubType in eType.EntitySubTypes)
                                        {
                                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eSubType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                            {
                                                if (exportPoints && eSubType.GeometryType == GeometryType.POINT ||
                                                    exportLines && eSubType.GeometryType == GeometryType.LINE ||
                                                    exportAreas && eSubType.GeometryType == GeometryType.AREA)
                                                {
                                                    // If the icon is Full Frame then four lines need to be exported, to reflect the four icon shapes.
                                                    // Else just write out one line for non-Full-Frame.

                                                    if (eSubType.Icon == IconType.FULL_FRAME && exportType == ETLExportEnum.ETLExportImage)
                                                    {
                                                        foreach (LibraryStandardIdentityGroup sig in _library.StandardIdentityGroups)
                                                        {
                                                            line = string.Format("{0}", exporter.Line(sig, s, e, eType, eSubType));

                                                            w.WriteLine(line);
                                                            w.Flush();

                                                            rowCount++;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        line = string.Format("{0}", exporter.Line(null, s, e, eType, eSubType));

                                                        w.WriteLine(line);
                                                        w.Flush();

                                                        rowCount++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Now process through any special entity sub types that might exist in a symbol set

                    if (s.SpecialEntitySubTypes != null && !(exporter is DomainEntityExport))
                    {
                        _exportSpecialEntities(exportType, exporter, s, w, s.SpecialEntitySubTypes);
                    }
                    else if (s.SpecialEntitySubTypes != null)
                    {
                        if (!append)
                        {
                            StreamWriter ws = new StreamWriter(specialPath);

                            line = string.Format("{0}", exporter.Headers);

                            ws.WriteLine(line);
                            ws.Flush();

                            _exportSpecialEntities(exportType, exporter, s, ws, s.SpecialEntitySubTypes);

                            ws.Close();
                        }
                        else
                            _exportSpecialEntities(exportType, exporter, s, w, s.SpecialEntitySubTypes);
                    }
                }

                w.Close();

                if (rowCount == 0)
                {
                    // Empty file so delete it
                    logger.Warn("Empty " + path + ". Deleting file...");
                    File.Delete(path);
                }
            }
        }

        private void _exportMod(StreamWriter w, SymbolSet s, ModifiersTypeModifier mod, string modNumber, IModifierExport exporter, string expression)
        {
            string line;

            if (!(exporter is ImageModifierExport) || mod.ModifierCode.DigitOne != 0 || mod.ModifierCode.DigitTwo != 0)
            {
                if (expression == "" ||
                    System.Text.RegularExpressions.Regex.IsMatch(mod.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    line = string.Format("{0}", exporter.Line(s, modNumber, mod));

                    w.WriteLine(line);
                    w.Flush();
                }
                else if (mod.Category != null)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(mod.Category, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        line = string.Format("{0}", exporter.Line(s, modNumber, mod));

                        w.WriteLine(line);
                        w.Flush();
                    }
                }
            }
        }

        private void _exportModifier2(IModifierExport exporter, string path, string symbolSetExpression = "", string expression = "", bool append = false)
        {
            using (var w = new StreamWriter(path, append))
            {
                string line;
                bool deleteIt = true;

                // If we're appending this to another file we don't need the
                // header line added again to that file.

                if (!append)
                {
                    line = string.Format("{0}", exporter.Headers);

                    w.WriteLine(line);
                    w.Flush();
                }

                foreach (SymbolSet s in _symbolSets)
                {
                    if (symbolSetExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(s.Label, symbolSetExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        continue;

                    if (s.SectorTwoModifiers != null)
                    {
                        deleteIt = false;

                        foreach (ModifiersTypeModifier mod in s.SectorTwoModifiers)
                        {
                            _exportMod(w, s, mod, "2", exporter, expression);
                        }
                    }
                }

                w.Close();

                if (deleteIt && !append)
                    File.Delete(path);
            }
        }

        private void _exportModifier1(IModifierExport exporter, string path, string symbolSetExpression = "", string expression = "", bool append = false)
        {
            using (var w = new StreamWriter(path, append))
            {
                string line;
                bool deleteIt = true;

                // If we're appending this to another file we don't need the
                // header line added again to that file.

                if (!append)
                {
                    line = string.Format("{0}", exporter.Headers);

                    w.WriteLine(line);
                    w.Flush();
                }

                foreach (SymbolSet s in _symbolSets)
                {
                    if (symbolSetExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(s.Label, symbolSetExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        continue;

                    if (s.SectorOneModifiers != null)
                    {
                        deleteIt = false;

                        foreach (ModifiersTypeModifier mod in s.SectorOneModifiers)
                        {
                            _exportMod(w, s, mod, "1", exporter, expression);
                        }
                    }
                }

                w.Close();

                if (deleteIt && !append)
                    File.Delete(path);
            }
        }

        private void _exportModifiers(IModifierExport exporter, string path1, string path2, string symbolSetExpression = "", string expression = "", bool append = false)
        {
            // Exports sector one and sector two modifiers to CSV, by optionally testing a 
            // regular expression against the Label attributes of the containing symbol sets
            // and of the modifiers in those symbol sets.  It also allows for appendig the
            // resulting output to an existing file.

            // This method accepts an exporter, a light weight object that knows what column
            // headings to return and how to compose a CSV line of output from the data its
            // provided.

            _exportModifier1(exporter, path1, symbolSetExpression, expression, append);
            _exportModifier2(exporter, path2, symbolSetExpression, expression, append);
        }

        private void _exportContextDetails(string headers, string path, bool dataValidation = false, bool append = false, bool isFirst = false)
        {
            using (var w = new StreamWriter(path + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryContext obj in _library.Contexts)
                {
                    if (!obj.IsExtension)
                    {
                        if (append)
                            w.WriteLine("Context," + Convert.ToString(obj.ContextCode) + ',' + obj.Label.Replace(',', '-'));
                        else
                            w.WriteLine(Convert.ToString(obj.ContextCode) + ',' + obj.Label.Replace(',', '-'));

                        w.Flush();
                    }
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("Context,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportContext(string path, bool dataValidation = false, bool append = false, bool isFirst = false, bool appendFileName = true)
        {
            // Export the Context elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                if (appendFileName)
                    filePath = path + "\\jmsml_Context";
                else
                    filePath = path;

                headers = "Code,Value";
            }

            _exportContextDetails(headers, filePath, dataValidation, append, isFirst);
        }

        private void _exportStandardIdentity(string path, bool dataValidation, bool append = false, bool isFirst = false)
        {
            // Export the Standard Identity elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                filePath = path + "\\jmsml_StandardIdentity";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryStandardIdentity obj in _library.StandardIdentities)
                {
                    if (!obj.IsExtension)
                    {
                        if (append)
                            w.WriteLine("Identity," + Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label.Replace(',', '-'));
                        else
                            w.WriteLine(Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label.Replace(',', '-'));

                        w.Flush();
                    }
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("Identity,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportSymbolSet(string path, bool dataValidation, bool append = false, bool isFirst = false)
        {
            // Export the Symbol Set elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                filePath = path + "\\jmsml_SymbolSet";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (SymbolSet obj in _symbolSets)
                {
                    if (append)
                        w.WriteLine("SymbolSet," + Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label.Replace(',', '-'));

                    w.Flush();
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("SymbolSet,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportStatus(string path, bool dataValidation, bool append = false, bool isFirst = false)
        {
            // Export the Status elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                filePath = path + "\\jmsml_Status";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryStatus obj in _library.Statuses)
                {
                    if (!obj.IsExtension)
                    {
                        if (append)
                            w.WriteLine("Status," + Convert.ToString(obj.StatusCode) + ',' + obj.Label.Replace(',', '-'));
                        else
                            w.WriteLine(Convert.ToString(obj.StatusCode) + ',' + obj.Label.Replace(',', '-'));

                        w.Flush();
                    }
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("Status,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportHQTFDummy(string path, bool dataValidation, bool append = false, bool isFirst = false)
        {
            // Export the HQTFDummy elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                filePath = path + "\\jmsml_HQTFDummy";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryHQTFDummy obj in _library.HQTFDummies)
                {
                    if (!obj.IsExtension)
                    {
                        if (append)
                            w.WriteLine("HQ_TF_FD," + Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label.Replace(',', '-'));
                        else
                            w.WriteLine(Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label.Replace(',', '-'));

                        w.Flush();
                    }
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("HQ_TF_FD,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportAmplifier(string path, bool dataValidation, bool append = false, bool isFirst = false)
        {
            // Export the Amplifier elements in the library as a coded domain CSV.

            string headers;
            string filePath;

            if (append)
            {
                filePath = path;
                headers = "Type,Code,Value";
            }
            else
            {
                filePath = path + "\\jmsml_Amplifier";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath + ".csv", (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryAmplifierGroup descript in _library.AmplifierGroups)
                {
                    if (descript.Amplifiers != null)
                    {
                        foreach (LibraryAmplifierGroupAmplifier obj in descript.Amplifiers)
                        {
                            if (!obj.IsExtension)
                            {
                                if (append)
                                    w.WriteLine("Amplifier," + Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label.Replace(',', '-'));
                                else
                                    w.WriteLine(Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label.Replace(',', '-'));

                                w.Flush();
                            }
                        }
                    }
                }

                if (dataValidation)
                {
                    if (append)
                        w.WriteLine("Amplifier,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
        }

        private string _cleanString(string s)
        {
            // Replace characters we don't like with the underscore

            string o = s.ToUpper();

            o = o.Replace('/', '_');
            o = o.Replace(' ', '_');
            o = o.Replace('-', '_');
            o = o.Replace('(', '_');
            o = o.Replace(')', '_');
            o = o.Trim();

            return o;
        }

        private void _writeCode(StreamWriter w, string name, string digitOne, string digitTwo)
        {
            // Write a basic two digt code to XML
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            w.WriteLine("<" + name + ">");
            w.WriteLine("<DigitOne>" + digitOne + "</DigitOne>");
            w.WriteLine("<DigitTwo>" + digitTwo + "</DigitTwo>");
            w.WriteLine("</" + name + ">");
            w.Flush();
        }

        private void _writeEntity(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            // Write an Entity element to XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            switch (mode)
            {
                case 0:
                    w.WriteLine("</Entity>");
                    break;

                case 1:
                    w.WriteLine("</EntityType>");
                    w.WriteLine("</EntityTypes>");
                    w.WriteLine("</Entity>");
                    break;

                case 2:
                    w.WriteLine("</EntitySubTypes>");
                    w.WriteLine("</EntityType>");
                    w.WriteLine("</EntityTypes>");
                    w.WriteLine("</Entity>");
                    break;
            }

            mode = 0;

            w.WriteLine("<Entity ID=\"" + id + "\" Label=\"" + label + "\" Graphic=\"" + graphic + "\">");
            _writeCode(w, "EntityCode", one, two);
            w.Flush();
        }

        private void _writeEntityType(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            // Write an EntityType element to XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            switch (mode)
            {
                case 0:
                    w.WriteLine("<EntityTypes>");
                    break;

                case 1:
                    w.WriteLine("</EntityType>");
                    break;

                case 2:
                    w.WriteLine("</EntitySubTypes>");
                    w.WriteLine("</EntityType>");
                    break;
            }

            mode = 1;

            w.WriteLine("<EntityType ID=\"" + id + "\" Label=\"" + label + "\" Graphic=\"" + graphic + "\">");
            _writeCode(w, "EntityTypeCode", one, two);
            w.Flush();
        }

        private void _writeEntitySubType(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            // Write an EntitySubType element to XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            switch (mode)
            {
                case 1:
                    w.WriteLine("<EntitySubTypes>");
                    break;
            }

            mode = 2;

            w.WriteLine("<EntitySubType ID=\"" + id + "\" Label=\"" + label + "\" Graphic=\"" + graphic + "\">");
            _writeCode(w, "EntitySubTypeCode", one, two);
            w.WriteLine("</EntitySubType>");
            w.Flush();
        }

        private void _writeModifier(StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            // Write a Modifier element to XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            w.WriteLine("<Modifier ID=\"" + id + "\" Label=\"" + label + "\" Category=\"TODO\" Graphic=\"" + graphic + "\">");
            _writeCode(w, "ModifierCode", one, two);
            w.WriteLine("</Modifier>");
            w.Flush();
        }

        private void _writeModifiers(string modPath, StreamWriter w, string setToDo, string modToDo)
        {
            // Write Modifier elements to XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            StreamReader rm1 = new StreamReader(modPath);

            while (!rm1.EndOfStream)
            {
                string line = rm1.ReadLine();
                string[] tokens = line.Split(',');

                string modSet = tokens[1].PadLeft(2, '0');
                string modNo = tokens[2];

                if (setToDo == modSet && modToDo == modNo)
                {
                    string mod = tokens[0];
                    string id = _cleanString(mod);
                    id = id + "_MOD";

                    string modCode = tokens[3].PadLeft(2, '0');

                    _writeModifier(w, id, mod, modCode.Substring(0, 1), modCode.Substring(1, 1), setToDo + modCode + modToDo + ".svg");
                }
            }

            rm1.Close();
        }

        private void _importCSV(string path, string modPath, string ssCode, string legacyCode)
        {
            // Read in a CSV file containing raw symbol data and use the above
            // methods to write out what is read as rough XML.
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            string line, ss, id, entity, entityType, entitySubType, codeE, codeET, codeEST, graphic;

            StreamReader r = new StreamReader(path);
            StreamWriter w = new StreamWriter("SymbolSet_" + ssCode + ".xml");

            w.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            w.WriteLine("<SymbolSet xmlns=\"http://disa.mil/JointMilSyML.xsd\" ID=\"AN_ID\" Label=\"A Label\" DimensionID=\"A_DIMENSION\">");

            _writeCode(w, "SymbolSetCode", ssCode.Substring(0, 1), ssCode.Substring(1, 1));

            if (legacyCode != "")
            {
                w.WriteLine("<LegacyCodingSchemeCode Name=\"2525C\">" + legacyCode + "</LegacyCodingSchemeCode>");
            }

            w.WriteLine("<Entities>");
            w.Flush();

            int mode = -1;

            while (!r.EndOfStream)
            {
                line = r.ReadLine();

                string[] tokens = line.Split(',');

                ss = tokens[0];
                entity = tokens[1].Trim();
                entityType = tokens[2].Trim();
                entitySubType = tokens[3].Trim();

                if (ss == ssCode)
                {
                    codeE = tokens[4].Substring(0, 2);
                    codeET = tokens[4].Substring(2, 2);
                    codeEST = tokens[4].Substring(4, 2);

                    graphic = ss + tokens[4] + ".svg";

                    if (entityType == "")
                    {
                        id = _cleanString(entity);

                        _writeEntity(ref mode, w, id, entity, codeE.Substring(0, 1), codeE.Substring(1, 1), graphic);
                        w.WriteLine("");
                    }
                    else
                    {
                        if (entitySubType == "")
                        {
                            id = _cleanString(entityType);

                            _writeEntityType(ref mode, w, id, entityType, codeET.Substring(0, 1), codeET.Substring(1, 1), graphic);
                            w.WriteLine("");
                        }
                        else
                        {
                            id = _cleanString(entitySubType);

                            _writeEntitySubType(ref mode, w, id, entitySubType, codeEST.Substring(0, 1), codeEST.Substring(1, 1), graphic);
                            w.WriteLine("");
                        }
                    }
                }
            }

            r.Close();

            switch (mode)
            {
                case 1:
                    w.WriteLine("</EntityType>");
                    w.WriteLine("</EntityTypes>");
                    break;
                case 2:
                    w.WriteLine("</EntitySubTypes>");
                    w.WriteLine("</EntityType>");
                    w.WriteLine("</EntityTypes>");
                    break;
            }

            w.WriteLine("</Entity>");
            w.WriteLine("</Entities>");

            w.WriteLine("<SectorOneModifiers>");
            _writeModifiers(modPath, w, ssCode, "1");
            w.WriteLine("</SectorOneModifiers>");

            w.WriteLine("<SectorTwoModifiers>");
            _writeModifiers(modPath, w, ssCode, "2");
            w.WriteLine("</SectorTwoModifiers>");

            w.WriteLine("</SymbolSet>");
            w.Flush();
        }

        private SymbolSet _deserializeSymbolSet(string path)
        {
            SymbolSet ss = null;

            var serializer = new XmlSerializer(typeof(SymbolSet));
            using (FileStream reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    ss = (SymbolSet)serializer.Deserialize(reader);
                }
                catch (IOException ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return ss;
        }

        private void _serializeSymbolSet(SymbolSet ss, string path)
        {
            if (ss != null)
            {
                var serializer = new XmlSerializer(typeof(SymbolSet));
                using (FileStream writer = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    try
                    {
                        serializer.Serialize(writer, ss);
                    }
                    catch (IOException ex)
                    {
                        logger.Error(ex.Message);
                    }
                }
            }
        }

        private void _importLegacyData(string source, string destination)
        {
            if (File.Exists(destination))
            {
                SymbolSet ss = _deserializeSymbolSet(destination);
                LibraryDimension dimension = _librarian.Dimension(ss.DimensionID);

                if (ss != null)
                {
                    string code = Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);

                    if (code != "")
                    {
                        // Open the source file for read.

                        if (File.Exists(source))
                        {
                            // Lets build a list of legacy symbols to hold the results

                            List<SymbolSetLegacySymbol> legacySymbols = new List<SymbolSetLegacySymbol>();

                            // Now process the input

                            string line = "";

                            using (StreamReader r = new StreamReader(source))
                            {
                                while (!r.EndOfStream)
                                {
                                    line = r.ReadLine();

                                    string[] tokens = line.Split(',');
                                    string ssCode = tokens[2];
                                    string legacyCode = tokens[0];
                                    string fullLegacyCode = tokens[1];

                                    string mod1Code = tokens[4];
                                    string mod2Code = tokens[5];

                                    string status = tokens[6];
                                    string remarks = tokens[7];

                                    // Skip over all rows in the source file that don't match the symbol set code
                                    // or where the 2525C SIDC field is blank.

                                    if (ssCode == code && legacyCode != "")
                                    {
                                        // Extract the six character function code from the current 2525C SIDC.

                                        string functionCode = legacyCode.Substring(legacyCode.Length - 6);
                                        //string schemaCode = legacyCode.Substring(0, 1);
                                        //string dimensionCode = legacyCode.Substring(2, 1);

                                        string entityCode = tokens[3];

                                        if (entityCode.Length == 6)
                                        {
                                            string eCode = entityCode.Substring(0, 2);
                                            string etCode = entityCode.Substring(2, 2);
                                            string estCode = entityCode.Substring(4, 2);

                                            // Now that we have everything, let's find the pieces in the open symbol set

                                            SymbolSetLegacySymbol ls = new SymbolSetLegacySymbol();

                                            // Find the entity, entity type, and entity subtype for the specified SIDC.

                                            SymbolSetEntity entity = _librarian.Entity(ss, Convert.ToUInt16(eCode.Substring(0, 1)), Convert.ToUInt16(eCode.Substring(1, 1)));

                                            if (entity != null)
                                            {
                                                ls.EntityID = entity.ID;
                                                ls.ID = entity.ID;

                                                if (etCode != "00")
                                                {
                                                    SymbolSetEntityEntityType eType = _librarian.EntityType(entity, Convert.ToUInt16(etCode.Substring(0, 1)), Convert.ToUInt16(etCode.Substring(1, 1)));

                                                    if (eType != null)
                                                    {
                                                        ls.EntityTypeID = eType.ID;
                                                        ls.ID = ls.ID + "_" + eType.ID;

                                                        if (estCode != "00")
                                                        {
                                                            EntitySubTypeType eSubType = _librarian.EntitySubType(eType, Convert.ToUInt16(estCode.Substring(0, 1)), Convert.ToUInt16(estCode.Substring(1, 1)));

                                                            if (eSubType != null)
                                                            {
                                                                ls.EntitySubTypeID = eSubType.ID;
                                                                ls.ID = ls.ID + "_" + eSubType.ID;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            // No entities were found, so mark it as retired.

                                            if (ls.ID == null)
                                            {
                                                ls.ID = "RETIRED";
                                                ls.Remarks = "Retired";
                                            }

                                            // Find the modifier 1 and modifier 2 for the specified SIDC.

                                            if (mod1Code != "" && mod1Code != "00" && mod1Code != "0")
                                            {
                                                if (mod1Code.Length == 2)
                                                {
                                                    ModifiersTypeModifier mod = _librarian.ModifierOne(ss, Convert.ToUInt16(mod1Code.Substring(0, 1)), Convert.ToUInt16(mod1Code.Substring(1, 1)));

                                                    if (mod != null)
                                                    {
                                                        ls.ModifierOneID = mod.ID;
                                                        ls.ID = ls.ID + "_" + mod.ID;
                                                    }
                                                }
                                            }

                                            if (mod2Code != "" && mod2Code != "00" && mod2Code != "0")
                                            {
                                                if (mod2Code.Length == 2)
                                                {
                                                    ModifiersTypeModifier mod = _librarian.ModifierTwo(ss, Convert.ToUInt16(mod2Code.Substring(0, 1)), Convert.ToUInt16(mod2Code.Substring(1, 1)));

                                                    if (mod != null)
                                                    {
                                                        ls.ModifierTwoID = mod.ID;
                                                        ls.ID = ls.ID + "_" + mod.ID;
                                                    }
                                                }
                                            }

                                            ls.ID = ls.ID + "_SYM";
                                            ls.Label = fullLegacyCode;

                                            // Add the legacy function code

                                            LegacyFunctionCodeType lfCode = new LegacyFunctionCodeType();
                                            lfCode.Name = "2525C";

                                            if (fullLegacyCode.Substring(0, 1) != ss.LegacyCodingSchemeCode[0].Value)
                                                lfCode.SchemaOverride = fullLegacyCode.Substring(0, 1);

                                            if (fullLegacyCode.Substring(2, 1) != dimension.LegacyDimensionCode[0].Value)
                                                lfCode.DimensionOverride = fullLegacyCode.Substring(2, 1);

                                            lfCode.Value = functionCode;

                                            if (status != "")
                                                lfCode.Description = status;

                                            if (remarks != "")
                                                lfCode.Remarks = remarks;

                                            ls.LegacyFunctionCode = new LegacyFunctionCodeType[] { lfCode };

                                            // Add it to the current legacy symbol set

                                            legacySymbols.Add(ls);
                                        }
                                    }
                                }

                                r.Close();
                            }

                            // Done processing the input.  If there is anything in our list then add it to the
                            // symbol set and re-serialize it.  Remove the existing values if they exist.

                            if (legacySymbols.Count > 0)
                            {
                                ss.LegacySymbols = legacySymbols.ToArray();

                                // Serialize the symbol set

                                _serializeSymbolSet(ss, destination);
                            }
                        }
                    }
                }
            }
        }

        private void _exportLegacyData(string path)
        {
            using (StreamWriter stream = new StreamWriter(path, false))
            {
                long passCount = 0;
                long failCount = 0;
                long cerrorCount = 0;
                long derrorCount = 0;
                long retiredCount = 0;
                long passWithConditionCount = 0;
                long totalCount = 0;

                stream.WriteLine("2525Charlie1stTen,2525Charlie,2525DeltaSymbolSet,2525DeltaEntity,2525DeltaMod1,2525DeltaMod2,2525DeltaName,2525DeltaMod1Name,2525DeltaMod2Name,DeltaToCharlie,Remarks");
                stream.Flush();

                foreach (SymbolSet ss in _librarian.SymbolSets)
                {
                    if (ss.LegacySymbols != null)
                    {
                        foreach (SymbolSetLegacySymbol legacy in ss.LegacySymbols)
                        {
                            string cSIDC = legacy.Label;
                            string cSIDCIn = legacy.Label;

                            // Create a proper 2525C SIDC for testing

                            if (cSIDCIn == null)
                                break;

                            if (cSIDCIn.Length == 15)
                            {
                                if (cSIDCIn.Substring(1, 1) == "*")
                                    cSIDCIn = cSIDCIn.Substring(0, 1) + "P" + cSIDCIn.Substring(2);

                                cSIDCIn = cSIDCIn.Replace('*', '-');

                                foreach (LegacyFunctionCodeType fcode in legacy.LegacyFunctionCode)
                                {
                                    string dFirst10 = "";
                                    string dSecond10 = "";
                                    string cSIDCOut = "";
                                    string status = "";

                                    string symbolName = "";
                                    string modOneName = "";
                                    string modTwoName = "";

                                    totalCount++;

                                    cSIDC = cSIDC.Substring(0, 4) + fcode.Value + cSIDC.Substring(10);
                                    cSIDCIn = cSIDCIn.Substring(0, 4) + fcode.Value + cSIDCIn.Substring(10);

                                    if (fcode.SchemaOverride != "")
                                    {
                                        cSIDC = fcode.SchemaOverride + cSIDC.Substring(1);
                                        cSIDCIn = fcode.SchemaOverride + cSIDCIn.Substring(1);
                                    }

                                    if (fcode.DimensionOverride != "")
                                    {
                                        cSIDC = cSIDC.Substring(0, 2) + fcode.DimensionOverride + cSIDC.Substring(3);
                                        cSIDCIn = cSIDCIn.Substring(0, 2) + fcode.DimensionOverride + cSIDCIn.Substring(3);
                                    }

                                    // Build a symbol using a 2525C SIDC

                                    Symbol sym = _librarian.MakeSymbol("2525C", cSIDCIn);

                                    if (sym != null)
                                    {
                                        if (sym.SymbolStatus == SymbolStatusEnum.statusEnumOld)
                                        {
                                            dFirst10 = sym.SIDC.PartAString;
                                            dSecond10 = sym.SIDC.PartBString;

                                            // Build a symbol using the 2525D SIDC, to see if reverse
                                            // conversion works.

                                            Symbol sym2 = _librarian.MakeSymbol(sym.SIDC);

                                            if (sym2 != null)
                                            {
                                                cSIDCOut = sym2.LegacySIDC;

                                                symbolName = sym2.Names["Entity"];
                                                modOneName = sym2.Names["ModifierOne"];
                                                modTwoName = sym2.Names["ModifierTwo"];

                                                if (cSIDCIn == cSIDCOut)
                                                {
                                                    status = "pass";
                                                    passCount++;
                                                }
                                                else if (sym2.LegacySIDCs.Count > 1)
                                                {
                                                    bool match = false;

                                                    foreach (string sidc in sym2.LegacySIDCs)
                                                    {
                                                        if (cSIDCIn == sidc)
                                                        {
                                                            match = true;
                                                            break;
                                                        }
                                                    }

                                                    if (match)
                                                    {
                                                        status = "pass (multiple)";
                                                        passWithConditionCount++;
                                                    }
                                                    else
                                                    {
                                                        status = "FAIL";
                                                        failCount++;
                                                    }
                                                }
                                                else
                                                {
                                                    status = "FAIL";
                                                    failCount++;
                                                }
                                            }
                                            else
                                            {
                                                status = "Error making 2525D";
                                                derrorCount++;
                                            }
                                        }
                                        else
                                        {
                                            status = "Retired";
                                            dFirst10 = SIDC.INVALID.PartAString;
                                            dSecond10 = SIDC.INVALID.PartBString;
                                            retiredCount++;
                                        }
                                    }
                                    else
                                    {
                                        status = "Error making 2525C";
                                        cerrorCount++;
                                    }

                                    stream.WriteLine(cSIDC.Substring(0, 10) + "," +
                                                     cSIDCIn + "," +
                                                     dFirst10.Substring(4, 2) + "," +
                                                     dSecond10.Substring(0, 6) + "," +
                                                     dSecond10.Substring(6, 2) + "," +
                                                     dSecond10.Substring(8, 2) + "," +
                                                     symbolName + "," +
                                                     modOneName + "," +
                                                     modTwoName + "," +
                                                     cSIDCOut + "," +
                                                     status);
                                    stream.Flush();
                                }
                            }
                            else
                                logger.Error("Bad SIDC : " + cSIDCIn);
                        }
                    }
                }

                logger.Info("----- Legacy Tests -----");
                logger.Info("Total: " + totalCount);
                logger.Info("Pass: " + passCount);
                logger.Info("Pass (multiple): " + passWithConditionCount);
                logger.Info("Retired: " + retiredCount);
                logger.Info("Fail: " + failCount);
                logger.Info("2525C Errors: " + cerrorCount);
                logger.Info("2525D Errors: " + derrorCount);
            }
        }

        public void Import(string path, string modPath, string symbolsetCode, string legacyCode)
        {
            // The public entry point to import a CSV file containing entities, and another containing
            // modiffiers, and use the above methods to do the work of
            // building a raw JMSML XML Symbol Set file.
            //
            // The source data comes from a manual dump of the tables
            // from Appendix A of 2525, modified in Excel to provide
            // further information to start fully populating JMSML XML.

            _importCSV(path, modPath, symbolsetCode, legacyCode);
        }

        public void ImportLegacyData(string source, string destination)
        {
            // Import the raw legacy SIDC information from source, and append it to the end of the specified
            // destination file.  The destination file should be an existing JMSML symbol set XML file.  
            // The symbol set code in that file will be used to extract the relevant rows from the source CSV.

            _importLegacyData(source, destination);
        }

        public void ExportLegacy(string path)
        {
            // Export legacy information, taking the existing 2525C code information in each Symbol Set
            // and converting it to 2525D, then taking what should be the 2525D numeric SIDC for that and
            // converting it back to 2525C.  Then testing to see if the results are the same.

            _exportLegacyData(path);
        }

        public void Export(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false, bool omitLegacy = false, long size = 32)
        {
            // The public entry point for exporting selective contents of the JMSML library
            // into CSV format.

            // Accepts a path for the output (sans file name extension).  The caller 
            // may also provide optional regular expressions to filter on the Label
            // attributes of SymbolSets in the library and a second optional regular
            // expression for filtering on the Label attributes of other objects being
            // exported.

            IEntityExport entityExporter = null;
            IModifierExport modifierExporter = null;

            string entityPath = path;
            string modifier1Path = path;
            string modifier2Path = path;
            string specialPath = path;

            _configHelper.PointSize = (int)size;

            switch (exportType)
            {
                // Based on the type of export, create instances of the
                // appropriate helper class(es).

                case ETLExportEnum.ETLExportSimple:
                    entityExporter = new SimpleEntityExport(_configHelper);
                    modifierExporter = new SimpleModifierExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportDomain:
                    entityExporter = new DomainEntityExport(_configHelper);
                    modifierExporter = new DomainModifierExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    entityExporter = new ImageEntityExport(_configHelper, omitSource, omitLegacy);
                    modifierExporter = new ImageModifierExport(_configHelper, omitSource, true); // Suppress legacy tag
                    break;
            }

            if (entityExporter != null && modifierExporter != null)
            {
                if (!append)
                {
                    // If we're not appending the modifiers to the entities
                    // then add a string to the file name to make them unique.

                    specialPath = entityPath + "_Special_Entity_Subtype";
                    entityPath = entityPath + "_Entities";
                    modifier1Path = modifier1Path + "_Modifier_Ones";
                    modifier2Path = modifier2Path + "_Modifier_Twos";
                }

                entityPath = entityPath + ".csv";
                specialPath = specialPath + ".csv";
                modifier1Path = modifier1Path + ".csv";
                modifier2Path = modifier2Path + ".csv";

                _exportEntities(exportType, entityExporter, entityPath, specialPath, symbolSetExpression, expression, exportPoints, exportLines, exportAreas, append);
                _exportModifiers(modifierExporter, modifier1Path, modifier2Path, symbolSetExpression, expression, append);
            }
        }

        public void ExportDomains(string path, bool dataValidation, bool append = false)
        {
            // The public entry point for exporting parts of the
            // 2525 and APP-6 base document - the codes making
            // up an SIDC's first ten digits.

            _exportAmplifier(path, dataValidation, append, true);
            _exportContext(path, dataValidation, append, false);
            _exportHQTFDummy(path, dataValidation, append, false);
            _exportStandardIdentity(path, dataValidation, append, false);
            _exportStatus(path, dataValidation, append, false);
            _exportSymbolSet(path, dataValidation, append, false);
        }

        public void ExportAmplifiers(string path, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false, bool omitLegacy = false, long size = 32, string amplifierExpression = "")
        {
            // The public entry point for exporting amplifiers from the JMSML library
            // into CSV format.  These include echelons, mobilities, and auxiliary equipment

            // Accepts a path for the output (sans file name extension).  The output may also
            // be appended to an existing file.

            IAmplifierExport amplifierExporter = null;
            string line = "";

            _configHelper.PointSize = (int)size;

            switch (exportType)
            {
                case ETLExportEnum.ETLExportDomain:
                    amplifierExporter = new DomainAmplifierExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    amplifierExporter = new ImageAmplifierExport(_configHelper, omitSource, omitLegacy);
                    break;
            }

            if (amplifierExporter != null)
            {
                using (var w = new StreamWriter(path + ".csv", append))
                {
                    if (!append)
                    {
                        line = string.Format("{0}", amplifierExporter.Headers);

                        w.WriteLine(line);
                        w.Flush();
                    }

                    foreach (LibraryAmplifierGroup lag in _library.AmplifierGroups)
                    {
                        if (amplifierExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(lag.Label, amplifierExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            continue;

                        if (lag.Amplifiers != null)
                        {
                            foreach (LibraryAmplifierGroupAmplifier amp in lag.Amplifiers)
                            {
                                if (amp.Graphics != null && exportType == ETLExportEnum.ETLExportImage)
                                {
                                    foreach (LibraryAmplifierGroupAmplifierGraphic graphic in amp.Graphics)
                                    {
                                        line = amplifierExporter.Line(lag, amp, graphic);

                                        if (line != "")
                                        {
                                            w.WriteLine(line);
                                            w.Flush();
                                        }
                                    }
                                }
                                else if (exportType == ETLExportEnum.ETLExportDomain)
                                {
                                    if (!amp.IsExtension)
                                    {
                                        line = amplifierExporter.Line(lag, amp, null);

                                        if (line != "")
                                        {
                                            w.WriteLine(line);
                                            w.Flush();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    w.Close();
                }
            }
        }

        public void ExportFrames(string path, string contextExpression = "", string standardIdentityExpression = "", string dimensionExpression = "", ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false, bool omitLegacy = false, long size = 32)
        {
            // The public entry point for exporting frames from the JMSML library
            // into CSV format.

            // Accepts a path for the output (sans file name extension).  The caller 
            // may also provide optional regular expressions to filter on the Label
            // attributes of the objects being exported.

            IFrameExport frameExporter = null;
            string line = "";

            _configHelper.PointSize = (int)size;

            switch (exportType)
            {
                case ETLExportEnum.ETLExportDomain:
                    frameExporter = new DomainFrameExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    frameExporter = new ImageFrameExport(_configHelper, omitSource, omitLegacy);
                    break;
            }

            if (frameExporter != null)
            {
                using (var w = new StreamWriter(path + ".csv", append))
                {
                    if (!append)
                    {
                        line = string.Format("{0}", frameExporter.Headers);

                        w.WriteLine(line);
                        w.Flush();
                    }

                    if (exportType == ETLExportEnum.ETLExportImage)
                    {
                        foreach (LibraryContext context in _library.Contexts)
                        {
                            if (contextExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(context.Label, contextExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                continue;

                            foreach (LibraryStandardIdentity identity in _library.StandardIdentities)
                            {
                                if (standardIdentityExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(identity.Label, standardIdentityExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                    continue;

                                foreach (LibraryDimension dimension in _library.Dimensions)
                                {
                                    if (dimensionExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(dimension.Label, dimensionExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                        continue;

                                    // Export a frame for Status = Current

                                    line = frameExporter.Line(_librarian, context, identity, dimension, _library.Statuses[0], false, false);

                                    if (line != "")
                                    {
                                        w.WriteLine(line);
                                        w.Flush();
                                    }

                                    // Export the frame for Status = Planned (not every frame will have a Planned version)
                                    // Shortened the label for this export so the stylx file will use the shorter name.

                                    LibraryStatus status = _library.Statuses[1];
                                    status.LabelAlias = "Planned";
                                    line = frameExporter.Line(_librarian, context, identity, dimension, status, false, false);
                                    status.LabelAlias = "";

                                    if (line != "")
                                    {
                                        w.WriteLine(line);
                                        w.Flush();
                                    }

                                    // Export the frame for the Civilian option (not every frame has one)

                                    line = frameExporter.Line(_librarian, context, identity, dimension, _library.Statuses[0], true, false);

                                    if (line != "")
                                    {
                                        w.WriteLine(line);
                                        w.Flush();
                                    }

                                    // Export the frame for the Planned Civilian option (not every frame has one)

                                    line = frameExporter.Line(_librarian, context, identity, dimension, _library.Statuses[1], false, true);

                                    if (line != "")
                                    {
                                        w.WriteLine(line);
                                        w.Flush();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (LibraryStandardIdentity identity in _library.StandardIdentities)
                        {
                            if (standardIdentityExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(identity.Label, standardIdentityExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                continue;

                            line = frameExporter.Line(null, null, identity, null, null, false, false);

                            if (!identity.IsExtension)
                            {
                                if (line != "")
                                {
                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                    }

                    w.Close();
                }
            }
        }

        public void ExportHQTFFD(string path, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false, bool omitLegacy = false, long size = 32)
        {
            // The public entry point for exporting HQTFFD from the JMSML library
            // into CSV format.  These include combinations of headquarter, task force, and feint/dummy amplifiers

            // Accepts a path for the output (sans file name extension).  The output may also
            // be appended to an existing file.

            IHQTFFDExport hqTFFDExporter = null;
            string line = "";

            _configHelper.PointSize = (int)size;

            switch (exportType)
            {
                case ETLExportEnum.ETLExportDomain:
                    hqTFFDExporter = new DomainHQTFFDExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    hqTFFDExporter = new ImageHQTFFDExport(_configHelper, omitSource, omitLegacy);
                    break;
            }

            if (hqTFFDExporter != null)
            {
                using (var w = new StreamWriter(path + ".csv", append))
                {
                    if (!append)
                    {
                        line = string.Format("{0}", hqTFFDExporter.Headers);

                        w.WriteLine(line);
                        w.Flush();
                    }

                    foreach (LibraryHQTFDummy hqTFFD in _library.HQTFDummies)
                    {
                        if (hqTFFD.Graphics != null && exportType == ETLExportEnum.ETLExportImage)
                        {
                            foreach (LibraryHQTFDummyGraphic graphic in hqTFFD.Graphics)
                            {
                                line = hqTFFDExporter.Line(hqTFFD, graphic);

                                if (line != "")
                                {
                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                        else if (exportType == ETLExportEnum.ETLExportDomain)
                        {
                            if (!hqTFFD.IsExtension)
                            {
                                line = hqTFFDExporter.Line(hqTFFD, null);

                                if (line != "")
                                {
                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ExportOCA(string path, string statusPath, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false, bool omitLegacy = false, long size = 32)
        {
            // The public entry point for exporting operational condition amplifiers from the JMSML library
            // into CSV format.

            // Accepts a path for the output (sans file name extension).  The output may also
            // be appended to an existing file.  Splits out the Status codes alone to a second file.

            IOCAExport ocaExporter = null;
            string line = "";

            _configHelper.PointSize = (int)size;

            switch (exportType)
            {
                case ETLExportEnum.ETLExportDomain:
                    ocaExporter = new DomainOCAExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    ocaExporter = new ImageOCAExport(_configHelper, omitSource, omitLegacy);
                    break;
            }

            if (ocaExporter != null)
            {
                using (var w = new StreamWriter(path + ".csv", append))
                {
                    // Create a second stream writer for status alone

                    StreamWriter ws = new StreamWriter(statusPath + ".csv");

                    if (!append)
                    {
                        line = string.Format("{0}", ocaExporter.Headers);

                        w.WriteLine(line);
                        w.Flush();

                        if (exportType == ETLExportEnum.ETLExportDomain)
                        {
                            ws.WriteLine(line);
                            ws.Flush();
                        }
                    }

                    foreach (LibraryStatus status in _library.Statuses)
                    {
                        if (status.Graphic != null && exportType == ETLExportEnum.ETLExportImage)
                        {
                            // Alternative graphic drawn for a given status

                            line = ocaExporter.Line(status);

                            if (line != "")
                            {
                                w.WriteLine(line);
                                w.Flush();
                            }
                        }

                        if (status.Graphics != null && exportType == ETLExportEnum.ETLExportImage)
                        {
                            foreach (LibraryStatusGraphic graphic in status.Graphics)
                            {
                                line = ocaExporter.Line(status, graphic);

                                if (line != "")
                                {
                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                        else if (exportType == ETLExportEnum.ETLExportDomain)
                        {
                            if (!status.IsExtension)
                            {
                                line = ocaExporter.Line(status);

                                if (line != "")
                                {
                                    w.WriteLine(line);
                                    w.Flush();

                                    if (status.StatusCode <= 1)
                                    {
                                        ws.WriteLine(line);
                                        ws.Flush();
                                    }
                                }
                            }
                        }
                    }

                    ws.Close();
                }
            }
        }

        public void ExportContext(string path, bool dataValidation = false, bool append = false)
        {
            // The public entry for exporting context information as a domain range, in CSV format

            // Accepts a path for the output (sans file name extension).  The output may also
            // be appended to an existing file.

            string line;

            IContextExport iCE = new DomainContextExport();

            using (var w = new StreamWriter(path + ".csv", append))
            {
                if (!append)
                {
                    line = string.Format("{0}", iCE.Headers);

                    w.WriteLine(line);
                    w.Flush();
                }

                foreach (LibraryContext context in _library.Contexts)
                {
                    if (!context.IsExtension)
                    {
                        line = string.Format("{0}", iCE.Line(context));

                        w.WriteLine(line);
                        w.Flush();
                    }
                }
            }
        }

        public void ExportAmplifierValueDomains(string path, bool append = false)
        {
            // The public entry for exporting the allowable values that are associated with
            // some text amplifiers.  The export format is compatible with other domain export
            // files created by this application.

            // Accepts a path for the output folder or a file, appending to that file if required.

            _exportAmplifierValueDomains(path, append);
            _exportSymbolSetCodes(path);
        }

        public void ExportSchemas(string path)
        {
            // Public entry for exporting military feature class schemas from the contents
            // of the JMSML config file and the JMSML XML data.

            _configHelper.MakeSchemaETL().ExportSchemas(path);
        }

        public void ExportLegacyLookup(string path, string standard, bool asOriginal, bool includeAmplifiers, bool append)
        {
            // Export legacy lookup information, taking all of the existing 2525X code information parts of symbols
            // and converting them to their 2525D equivalent codes, then writing out the results to the specified path.

            // asOriginal = false if the legacy symbols should map to graphics that make them look as they did originally.
            // asOriginal = true if the legacy symbols should map to graphics that are the latest in 2525D.

            _configHelper.MakeLegacyETL().ExportLegacyLookup(path, standard, asOriginal, includeAmplifiers, append);
        }

        public void ExportLegacyEntities(string path, string standard, long size = 32)
        {
            // Export entities from legacy versions of the standard that are no longer implemented or
            // implemented differently then they used to be.

            _configHelper.MakeLegacyETL().ExportLegacyEntities(path, standard, size);
        }

        public void ExportLegacyFrames(string path, string standard, long size = 32, bool append = false)
        {
            // Export frames from legacy versions of the standard that are no longer implemented or
            // implemented differently then they used to be.

            _configHelper.MakeLegacyETL().ExportLegacyFrames(path, standard, size, append);
        }
    }
}
