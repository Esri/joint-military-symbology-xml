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
using System.IO;
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

        private void _exportEntities(IEntityExport exporter, string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true)
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

                foreach (SymbolSet s in _symbolSets)
                {
                    if (symbolSetExpression != "" && !System.Text.RegularExpressions.Regex.IsMatch(s.Label, symbolSetExpression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        continue;

                    foreach (SymbolSetEntity e in s.Entities)
                    {
                        if (exportPoints && e.GeometryType == GeometryType.POINT ||
                            exportLines && e.GeometryType == GeometryType.LINE ||
                            exportAreas && e.GeometryType == GeometryType.AREA ||
                            e.GeometryType == GeometryType.NA)
                        {

                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(e.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                line = string.Format("{0}", exporter.Line(s, e, null, null));

                                w.WriteLine(line);
                                w.Flush();
                            }

                            if (e.EntityTypes != null)
                            {
                                foreach (SymbolSetEntityEntityType eType in e.EntityTypes)
                                {
                                    if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                    {
                                        line = string.Format("{0}", exporter.Line(s, e, eType, null));

                                        w.WriteLine(line);
                                        w.Flush();
                                    }

                                    if (eType.EntitySubTypes != null)
                                    {
                                        foreach (SymbolSetEntityEntityTypeEntitySubType eSubType in eType.EntitySubTypes)
                                        {
                                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eSubType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                            {
                                                line = string.Format("{0}", exporter.Line(s, e, eType, eSubType));

                                                w.WriteLine(line);
                                                w.Flush();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                w.Close();
            }
        }

        private void _exportModifiers(IModifierExport exporter, string path, string symbolSetExpression = "", string expression = "", bool append = false)
        {
            // Exports sector one and sector two modifiers to CSV, by optionally testing a 
            // regular expression against the Label attributes of the containing symbol sets
            // and of the modifiers in those symbol sets.  It also allows for appendig the
            // resulting output to an existing file.

            // This method accepts an exporter, a light weight object that knows what column
            // headings to return and how to compose a CSV line of output from the data its
            // provided.

            using (var w = new StreamWriter(path, append))
            {
                string line;

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
                        foreach (ModifiersTypeModifier mod in s.SectorOneModifiers)
                        {
                            if (expression == "" ||
                                System.Text.RegularExpressions.Regex.IsMatch(mod.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                                System.Text.RegularExpressions.Regex.IsMatch(mod.Category, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                line = string.Format("{0}", exporter.Line(s, "1", mod));

                                w.WriteLine(line);
                                w.Flush();
                            }
                        }
                    }

                    if (s.SectorTwoModifiers != null)
                    {
                        foreach (ModifiersTypeModifier mod in s.SectorTwoModifiers)
                        {
                            if (expression == "" ||
                                System.Text.RegularExpressions.Regex.IsMatch(mod.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                                System.Text.RegularExpressions.Regex.IsMatch(mod.Category, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                line = string.Format("{0}", exporter.Line(s, "2", mod));

                                w.WriteLine(line);
                                w.Flush();
                            }
                        }
                    }
                }

                w.Close();
            }
        }

        private void _exportContext(string path, bool dataValidation, bool append = false, bool isFirst = false)
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
                filePath = path + "\\jmsml_Context.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryContext obj in _library.Contexts)
                {
                    if(append)
                        w.WriteLine("Context," + Convert.ToString(obj.ContextCode) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.ContextCode) + ',' + obj.Label.Replace(',', '-'));

                    w.Flush();
                }

                if (dataValidation)
                {
                    if(append)
                        w.WriteLine("Context,-1,NotSet");
                    else
                        w.WriteLine("-1,NotSet");

                    w.Flush();
                }

                w.Close();
            }
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
                filePath = path + "\\jmsml_StandardIdentity.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryStandardIdentity obj in _library.StandardIdentities)
                {
                    if(append)
                        w.WriteLine("Identity," + Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label.Replace(',', '-'));

                    w.Flush();
                }

                if (dataValidation)
                {
                    if(append)
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
                filePath = path + "\\jmsml_SymbolSet.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach(SymbolSet obj in _symbolSets)
                {
                    if(append)
                        w.WriteLine("SymbolSet," + Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label.Replace(',', '-'));
                    
                    w.Flush();
                }

                if (dataValidation)
                {
                    if(append)
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
                filePath = path + "\\jmsml_Status.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryStatus obj in _library.Statuses)
                {
                    if(append)
                        w.WriteLine("Status," + Convert.ToString(obj.StatusCode) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.StatusCode) + ',' + obj.Label.Replace(',', '-'));

                    w.Flush();
                }

                if (dataValidation)
                {
                    if(append)
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
                filePath = path + "\\jmsml_HQTFDummy.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryHQTFDummy obj in _library.HQTFDummies)
                {
                    if(append)
                        w.WriteLine("HQ_TF_FD," + Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label.Replace(',', '-'));
                    else
                        w.WriteLine(Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label.Replace(',', '-'));

                    w.Flush();
                }

                if (dataValidation)
                {
                    if(append)
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
                filePath = path + "\\jmsml_Amplifier.csv";
                headers = "Code,Value";
            }

            using (var w = new StreamWriter(filePath, (append && !isFirst)))
            {
                if (isFirst || !append)
                    w.WriteLine(headers);

                foreach (LibraryAmplifierGroup descript in _library.AmplifierGroups)
                {
                    if (descript.Amplifiers != null)
                    {
                        foreach (LibraryAmplifierGroupAmplifier obj in descript.Amplifiers)
                        {
                            if(append)
                                w.WriteLine("Amplifier," + Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label.Replace(',', '-'));
                            else
                                w.WriteLine(Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label.Replace(',', '-'));

                            w.Flush();
                        }
                    }
                }

                if (dataValidation)
                {
                    if(append)
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

        public void Export(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false)
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
            string modifierPath = path;

            switch (exportType)
            {
                // Based on the type of export, create instances of the
                // appropriate helper class(es).

                case ETLExportEnum.ETLExportSimple:
                    entityExporter = new SimpleEntityExport();
                    modifierExporter = new SimpleModifierExport();
                    break;

                case ETLExportEnum.ETLExportDomain:
                    entityExporter = new DomainEntityExport(_configHelper);
                    modifierExporter = new DomainModifierExport(_configHelper);
                    break;

                case ETLExportEnum.ETLExportImage:
                    entityExporter = new ImageEntityExport(_configHelper, omitSource);
                    modifierExporter = new ImageModifierExport(_configHelper, omitSource);
                    break;
            }

            if (entityExporter != null && modifierExporter != null)
            {
                if (!append)
                {
                    // If we're not appending the modifiers to the entities
                    // then add a string to the file name to make them unique.

                    entityPath = entityPath + "_Entities";
                    modifierPath = modifierPath + "_Modifiers";
                }

                entityPath = entityPath + ".csv";
                modifierPath = modifierPath + ".csv";

                _exportEntities(entityExporter, entityPath, symbolSetExpression, expression, exportPoints, exportLines, exportAreas);
                _exportModifiers(modifierExporter, modifierPath, symbolSetExpression, expression, append);
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

        public void ExportAmplifiers(string path, ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool append = false, bool omitSource = false)
        {
            // The public entry point for exporting amplifiers from the JMSML library
            // into CSV format.  These include echelons, mobilities, and auxiliary equipment

            // Accepts a path for the output (sans file name extension).  The output may also
            // be appended to an existing file.

            IAmplifierExport amplifierExporter = null;
            string line = "";

            switch (exportType)
            {
                case ETLExportEnum.ETLExportImage:
                    amplifierExporter = new ImageAmplifierExport(_configHelper, omitSource);
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
                        if (lag.Amplifiers != null)
                        {
                            foreach (LibraryAmplifierGroupAmplifier amp in lag.Amplifiers)
                            {
                                if (amp.Graphics != null)
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
                            }
                        }
                    }

                    w.Close();
                }
            }
        }

        public void ExportFrames(string path, string contextExpression = "", string standardIdentityExpression = "", string dimensionExpression = "", ETLExportEnum exportType = ETLExportEnum.ETLExportSimple, bool omitSource = false)
        {
            // The public entry point for exporting frames from the JMSML library
            // into CSV format.

            // Accepts a path for the output (sans file name extension).  The caller 
            // may also provide optional regular expressions to filter on the Label
            // attributes of the objects being exported.

            IFrameExport frameExporter = null;

            switch (exportType)
            {
                case ETLExportEnum.ETLExportImage:
                    frameExporter = new ImageFrameExport(_configHelper, omitSource);
                    break;
            }

            if (frameExporter != null)
            {
                using (var w = new StreamWriter(path + ".csv"))
                {
                    var line = string.Format("{0}", frameExporter.Headers);

                    w.WriteLine(line);
                    w.Flush();

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

                                line = frameExporter.Line(_librarian, context, identity, dimension);

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
    }
}
