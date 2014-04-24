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
    public class ETL
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private string _domainSeperator = " : ";
        private int _pointSize;

        private Librarian _librarian;
        private Library _library;
        private List<SymbolSet> _symbolSets;
        //private jmsmlConfigEtl _etlConfig;

        private string _graphicRoot;
        private string _graphicExtension;

        public ETL(Librarian librarian)
        {
            _librarian = librarian;
            _library = _librarian.Library;
            _symbolSets = _librarian.SymbolSets;
            //_etlConfig = _librarian.ConfigData.etl;

            //_domainSeperator = _etlConfig.domainSeparator;
            //_pointSize = _etlConfig.pointSize;
            //_graphicRoot = _etlConfig.graphicRoot;
            //_graphicExtension = _etlConfig.graphicExtension;
        }

        private string _buildEntityString(SymbolSet s, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            string result = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);
            string code = "";

            result = result + ",";

            result = result + e.Label.Replace(',', '-');
            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

            result = result + ",";

            if (eType != null)
            {
                result = result + eType.Label.Replace(',', '-');
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            result = result + ",";

            if (eSubType != null)
            {
                result = result + eSubType.Label.Replace(',', '-');
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            result = result + "," + code + "," + Convert.ToString(e.GeometryType);

            return result;
        }

        private string _buildEntityDomainString(SymbolSet s, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            string result = s.Label.Replace(',', '-') + _domainSeperator + e.Label.Replace(',', '-');
            string code = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);

            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

            if (eType != null)
            {
                result = result + _domainSeperator + eType.Label.Replace(',', '-');
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            if (eSubType != null)
            {
                result = result + _domainSeperator + eSubType.Label.Replace(',', '-');
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            result = result + "," + code;

            return result;
        }

        private string _buildModifierString(SymbolSet s, string modNumber, ModifiersTypeModifier mod)
        {
            string result = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);

            result = result + "," + modNumber + ",";
            result = result + mod.Category.Replace(',', '-') + ",";
            result = result + mod.Label.Replace(',', '-') + ",";

            result = result + Convert.ToString(mod.ModifierCode.DigitOne) + Convert.ToString(mod.ModifierCode.DigitTwo);

            return result;
        }

        private string _buildModifierDomainString(SymbolSet s, string modNumber, ModifiersTypeModifier mod)
        {
            string result = s.Label.Replace(',', '-') + _domainSeperator + "Modifier " + modNumber + _domainSeperator + mod.Category.Replace(',', '-') + _domainSeperator + mod.Label.Replace(',', '-') + ",";

            result = result + Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo) +
                              Convert.ToString(mod.ModifierCode.DigitOne) + Convert.ToString(mod.ModifierCode.DigitTwo) +
                              modNumber;
            return result;
        }

        private void _exportEntities(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, bool domains = false)
        {
            string entityHeaders = "SymbolSet,Entity,EntityType,EntitySubType,Code,GeometryType";
            string entityDomainHeaders = "Name,Value";

            using (var w = new StreamWriter(path))
            {
                var line = string.Format("{0}", entityHeaders);

                if (domains)
                    line = string.Format("{0}", entityDomainHeaders);

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
                            exportAreas && e.GeometryType == GeometryType.AREA)
                        {

                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(e.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                if (domains)
                                    line = string.Format("{0}", _buildEntityDomainString(s, e, null, null));
                                else
                                    line = string.Format("{0}", _buildEntityString(s, e, null, null));

                                w.WriteLine(line);
                                w.Flush();
                            }

                            if (e.EntityTypes != null)
                            {
                                foreach (SymbolSetEntityEntityType eType in e.EntityTypes)
                                {
                                    if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                    {
                                        if (domains)
                                            line = string.Format("{0}", _buildEntityDomainString(s, e, eType, null));
                                        else
                                            line = string.Format("{0}", _buildEntityString(s, e, eType, null));

                                        w.WriteLine(line);
                                        w.Flush();
                                    }

                                    if (eType.EntitySubTypes != null)
                                    {
                                        foreach (SymbolSetEntityEntityTypeEntitySubType eSubType in eType.EntitySubTypes)
                                        {
                                            if (expression == "" || System.Text.RegularExpressions.Regex.IsMatch(eSubType.Label, expression, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                            {
                                                if (domains)
                                                    line = string.Format("{0}", _buildEntityDomainString(s, e, eType, eSubType));
                                                else
                                                    line = string.Format("{0}", _buildEntityString(s, e, eType, eSubType));

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

        private void _exportModifiers(string path, string symbolSetExpression = "", string expression = "", bool domains = false)
        {
            string modifierHeaders = "SymbolSet,ModifierNumber,Category,Name,Code";
            string modifierDomainHeaders = "Name,Value";

            using (var w = new StreamWriter(path))
            {
                var line = string.Format("{0}", modifierHeaders);

                if (domains)
                    line = string.Format("{0}", modifierDomainHeaders);

                w.WriteLine(line);
                w.Flush();

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
                                if (domains)
                                    line = string.Format("{0}", _buildModifierDomainString(s, "1", mod));
                                else
                                    line = string.Format("{0}", _buildModifierString(s, "1", mod));

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
                                if (domains)
                                    line = string.Format("{0}", _buildModifierDomainString(s, "2", mod));
                                else
                                    line = string.Format("{0}", _buildModifierString(s, "2", mod));

                                w.WriteLine(line);
                                w.Flush();
                            }
                        }
                    }
                }

                w.Close();
            }
        }

        private void _exportContext(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_Context.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryContext obj in _library.Contexts)
                {
                    w.WriteLine(Convert.ToString(obj.ContextCode) + ',' + obj.Label.Replace(',', '-'));
                    w.Flush();
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportStandardIdentity(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_StandardIdentity.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryStandardIdentity obj in _library.StandardIdentities)
                {
                    w.WriteLine(Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label.Replace(',', '-'));
                    w.Flush();
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportSymbolSet(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_SymbolSet.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryDimension dimension in _library.Dimensions)
                {
                    if (dimension.SymbolSets != null)
                    {
                        foreach (LibraryDimensionSymbolSetRef obj in dimension.SymbolSets)
                        {
                            w.WriteLine(Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label.Replace(',', '-'));
                            w.Flush();
                        }
                    }
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportStatus(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_Status.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryStatus obj in _library.Statuses)
                {
                    w.WriteLine(Convert.ToString(obj.StatusCode) + ',' + obj.Label.Replace(',', '-'));
                    w.Flush();
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportHQTFDummy(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_HQTFDummy.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryHQTFDummy obj in _library.HQTFDummies)
                {
                    w.WriteLine(Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label.Replace(',', '-'));
                    w.Flush();
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportAmplifier(string path, bool dataValidation)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_Amplifier.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryAmplifierGroup descript in _library.AmplifierGroups)
                {
                    if (descript.Amplifiers != null)
                    {
                        foreach (LibraryAmplifierGroupAmplifier obj in descript.Amplifiers)
                        {
                            w.WriteLine(Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label.Replace(',', '-'));
                            w.Flush();
                        }
                    }
                }

                if (dataValidation)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private string _cleanString(string s)
        {
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
            w.WriteLine("<" + name + ">");
            w.WriteLine("<DigitOne>" + digitOne + "</DigitOne>");
            w.WriteLine("<DigitTwo>" + digitTwo + "</DigitTwo>");
            w.WriteLine("</" + name + ">");
            w.Flush();
        }

        private void _writeEntity(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
        {
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
            w.WriteLine("<Modifier ID=\"" + id + "\" Label=\"" + label + "\" Category=\"TODO\" Graphic=\"" + graphic + "\">");
            _writeCode(w, "ModifierCode", one, two);
            w.WriteLine("</Modifier>");
            w.Flush();
        }

        private void _writeModifiers(string modPath, StreamWriter w, string setToDo, string modToDo)
        {
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
            _importCSV(path, modPath, symbolsetCode, legacyCode);
        }

        public void Export(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, bool asCodedDomain = false)
        {
            _exportEntities(path + "_Entities.csv", symbolSetExpression, expression, exportPoints, exportLines, exportAreas, asCodedDomain);
            _exportModifiers(path + "_Modifiers.csv", symbolSetExpression, expression, asCodedDomain);
        }

        public void ExportDomains(string path, bool dataValidation)
        {
            _exportContext(path, dataValidation);
            _exportStandardIdentity(path, dataValidation);
            _exportSymbolSet(path, dataValidation);
            _exportStatus(path, dataValidation);
            _exportHQTFDummy(path, dataValidation);
            _exportAmplifier(path, dataValidation);
        }
    }
}
