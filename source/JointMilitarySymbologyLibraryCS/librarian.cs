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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace JointMilitarySymbologyLibrary
{
    internal enum StatusCodeEnum
    {
        statusCodeNoVersion,
        statusCodeNoContext,
        statusCodeNoDimension,
        statusCodeNoStandardIdentity,
        statusCodeNoSymbolSet,
        statusCodeNoStatus,
        statusCodeNoHQTFDummy,
        statusCodeNoAmplifierGroup,
        statusCodeNoAmplifier,
        statusCodeNoAffiliation,
        statusCodeNoContextAmplifier,
        statusCodeNoEntity,
        statusCodeNoEntityType,
        statusCodeNoEntitySubType,
        statusCodeNoModifierOne,
        statusCodeNoModifierTwo,
        statusCodeNoLegacySymbol
    }

    public class Librarian
    {
        private const string _domainSeperator = " : ";

        private string _configPath;
        private jmsmlConfig _configData;
        private Library _library;

        private bool _logConversion = true;
        private int _statusFlag = 0;

        private Symbol _invalidSymbol;
        private Symbol _retiredSymbol;

        private List<SymbolSet> _symbolSets = new List<SymbolSet>();
        private List<string> _statusMessages = new List<string> {"Version Not Found",
                                                                 "Context Not Found",
                                                                 "Dimension Not Found",
                                                                 "Standard Identity Not Found",
                                                                 "Symbol Set Not Found",
                                                                 "Status Not Found",
                                                                 "HQ/TF/Dummy Not Found",
                                                                 "Amplifier Group Not Found",
                                                                 "Amplifier Not Found",
                                                                 "Affiliation Not Found",
                                                                 "Context Amplifier Not Found",
                                                                 "Entity Not Found",
                                                                 "Entity Type Not Found",
                                                                 "Entity SubType Not Found",
                                                                 "Modifier One Not Found",
                                                                 "Modifier Two Not Found",
                                                                 "Legacy Symbol Not Found"};

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public Librarian(string configPath = "")
        {
            InitializeNLog();

            //
            // Deserialize the configuration xml to get the location of the symbology library
            //

            XmlSerializer serializer = new XmlSerializer(typeof(jmsmlConfig));

            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            if (configPath != "")
            {
                _configPath = configPath;
            }
            else
            {
                string s = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _configPath = Path.Combine(s, "jmsml.config");
            }

            if (File.Exists(_configPath))
            {
                FileStream fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read);

                if (fs.CanRead)
                {
                    _configData = (jmsmlConfig)serializer.Deserialize(fs);

                    //
                    // Deserialize the library's base xml to get the base contents of the symbology standard
                    //

                    serializer = new XmlSerializer(typeof(Library));

                    string path = _configData.libraryPath + "/" + _configData.libraryName;

                    if (File.Exists(path))
                    {
                        fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                        if (fs.CanRead)
                        {
                            this._library = (Library)serializer.Deserialize(fs);

                            //
                            // Deserialize each symbolSet xml
                            //

                            foreach (LibraryDimension dimension in this._library.Dimensions)
                            {
                                foreach (LibraryDimensionSymbolSetRef ssRef in dimension.SymbolSets)
                                {
                                    path = _configData.libraryPath + "/" + ssRef.Instance;
                                    if (File.Exists(path))
                                    {
                                        fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                                        if (fs.CanRead)
                                        {
                                            serializer = new XmlSerializer(typeof(SymbolSet));
                                            SymbolSet ss = (SymbolSet)serializer.Deserialize(fs);

                                            if (ss != null)
                                            {
                                                _symbolSets.Add(ss);
                                            }
                                        }
                                        else
                                        {
                                            logger.Error("Unreadable symbol set: " + path);
                                        }
                                    }
                                    else
                                    {
                                        logger.Error("Symbol set is missing: " + path);
                                    }
                                }
                            }

                            //
                            // Create special invalid and retired symbols for this library
                            //

                            _invalidSymbol = new Symbol(this, SIDC.INVALID);
                            _retiredSymbol = new Symbol(this, SIDC.RETIRED);
                        }
                        else
                        {
                            logger.Error("Unreadable symbol library: " + path);
                        }
                    }
                    else
                    {
                        logger.Error("Specified library is missing: " + path);
                    }
                }
                else
                {
                    logger.Error("Unreadable config file: " + _configPath);
                }
            }
            else
            {
                logger.Error("Config file is missing: " + _configPath);
            }
        }

        private void InitializeNLog()
        {
            Target logTarget = new FileTarget();

            ((FileTarget)logTarget).FileName = @"${specialfolder:MyDocuments}/jmsml/logs/${shortdate}.log";

            ((FileTarget)logTarget).Layout = new SimpleLayout("${longdate} | ${level} | ${callsite} | ${message} | ${exception:format=ToString}");

            /*
             * Write the log asynchronously
             */
            AsyncTargetWrapper wrapper = new AsyncTargetWrapper();
            wrapper.WrappedTarget = logTarget;
            wrapper.QueueLimit = 5000;
            wrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;

            SimpleConfigurator.ConfigureForTargetLogging(wrapper, LogLevel.Debug);
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }

        private string _buildEntityString(SymbolSet s, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            string result = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);
            string code = "";

            result = result + ",";

            result = result + "\"" + e.Label + "\"";
            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

            result = result + ",";

            if (eType != null)
            {
                result = result + "\"" + eType.Label + "\"";
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
            }
            else
                code = code + "00";
            
            result = result + ",";

            if (eSubType != null)
            {
                result = result + "\"" + eSubType.Label + "\"";
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            result = result + "," + code + "," + Convert.ToString(e.GeometryType);

            return result;
        }

        private string _buildEntityDomainString(SymbolSet s, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            string result = "\"" + s.Label + _domainSeperator + e.Label;
            string code = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);

            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);

            if (eType != null)
            {
                result = result + _domainSeperator + eType.Label;
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            if (eSubType != null)
            {
                result = result + _domainSeperator + eSubType.Label;
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
            }
            else
                code = code + "00";

            result = result + "\"," + code;

            return result;
        }

        private string _buildModifierString(SymbolSet s, string modNumber, ModifiersTypeModifier mod)
        {
            string result = Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo);

            result = result + "," + modNumber + ",";
            result = result + "\"" + mod.Category + "\",";
            result = result + "\"" + mod.Label + "\",";

            result = result + Convert.ToString(mod.ModifierCode.DigitOne) + Convert.ToString(mod.ModifierCode.DigitTwo);

            return result;
        }

        private string _buildModifierDomainString(SymbolSet s, string modNumber, ModifiersTypeModifier mod)
        {
            string result = "\"" + s.Label + _domainSeperator + "Modifier " + modNumber + _domainSeperator + mod.Label + "\"" + ",";

            result = result + Convert.ToString(s.SymbolSetCode.DigitOne) + Convert.ToString(s.SymbolSetCode.DigitTwo) +
                              Convert.ToString(mod.ModifierCode.DigitOne) + Convert.ToString(mod.ModifierCode.DigitTwo) +
                              modNumber;
            return result;
        }

        private void _exportEntities(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, bool domains = false)
        {
            string entityHeaders = "SymbolSet,Entity,EntityType,EntitySubType,Code,GeometryType";
            string entityDomainHeaders = "Name,Value";

            using(var w = new StreamWriter(path))
            {
                var line = string.Format("{0}", entityHeaders);

                if (domains)
                    line = string.Format("{0}", entityDomainHeaders);

                w.WriteLine(line);
                w.Flush();

                foreach(SymbolSet s in _symbolSets)
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
                                if(domains)
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
                                        if(domains)
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
                                                if(domains)
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
            string modifierHeaders = "SymbolSet,ModifierNumber,Category,Modifier,Code";
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

                    if(s.SectorOneModifiers != null)
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

                    if(s.SectorTwoModifiers != null)
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

        private void _exportContext(string path, bool asEsri)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_Context.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryContext obj in _library.Contexts)
                {
                    w.WriteLine(Convert.ToString(obj.ContextCode) + ',' + obj.Label);
                    w.Flush();
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportStandardIdentity(string path, bool asEsri)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_StandardIdentity.csv"))
            {
                w.WriteLine(headers);

                foreach(LibraryStandardIdentity obj in _library.StandardIdentities)
                {
                    w.WriteLine(Convert.ToString(obj.StandardIdentityCode) + ',' + obj.Label);
                    w.Flush();
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportSymbolSet(string path, bool asEsri)
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
                            w.WriteLine(Convert.ToString(obj.SymbolSetCode.DigitOne) + Convert.ToString(obj.SymbolSetCode.DigitTwo) + ',' + obj.Label);
                            w.Flush();
                        }
                    }
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportStatus(string path, bool asEsri)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_Status.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryStatus obj in _library.Statuses)
                {
                    w.WriteLine(Convert.ToString(obj.StatusCode) + ',' + obj.Label);
                    w.Flush();
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportHQTFDummy(string path, bool asEsri)
        {
            string headers = "Code,Value";

            using (var w = new StreamWriter(path + "\\jmsml_HQTFDummy.csv"))
            {
                w.WriteLine(headers);

                foreach (LibraryHQTFDummy obj in _library.HQTFDummies)
                {
                    w.WriteLine(Convert.ToString(obj.HQTFDummyCode) + ',' + obj.Label);
                    w.Flush();
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void _exportAmplifier(string path, bool asEsri)
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
                            w.WriteLine(Convert.ToString(descript.AmplifierGroupCode) + Convert.ToString(obj.AmplifierCode) + ',' + obj.Label);
                            w.Flush();
                        }
                    }
                }

                if (asEsri)
                {
                    w.WriteLine("-1,NotSet");
                    w.Flush();
                }

                w.Close();
            }
        }

        private void WriteCode(StreamWriter w, string name, string digitOne, string digitTwo)
        {
            w.WriteLine("<" + name + ">");
            w.WriteLine("<DigitOne>" + digitOne + "</DigitOne>");
            w.WriteLine("<DigitTwo>" + digitTwo + "</DigitTwo>");
            w.WriteLine("</" + name + ">");
            w.Flush();
        }

        private void WriteEntity(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
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
            WriteCode(w, "EntityCode", one, two);
            w.Flush();
        }

        private void WriteEntityType(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
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
            WriteCode(w, "EntityTypeCode", one, two);
            w.Flush();
        }

        private void WriteEntitySubType(ref int mode, StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            switch (mode)
            {
                case 1:
                    w.WriteLine("<EntitySubTypes>");
                    break;
            }

            mode = 2;

            w.WriteLine("<EntitySubType ID=\"" + id + "\" Label=\"" + label + "\" Graphic=\"" + graphic + "\">");
            WriteCode(w, "EntitySubTypeCode", one, two);
            w.WriteLine("</EntitySubType>");
            w.Flush();
        }

        private void WriteModifier(StreamWriter w, string id, string label, string one, string two, string graphic)
        {
            w.WriteLine("<Modifier ID=\"" + id + "\" Label=\"" + label + "\" Category=\"TODO\" Graphic=\"" + graphic + "\">");
            WriteCode(w, "ModifierCode", one, two);
            w.WriteLine("</Modifier>");
            w.Flush();
        }

        private void WriteModifiers(string modPath, StreamWriter w, string setToDo, string modToDo)
        {
            StreamReader rm1 = new StreamReader(modPath);

            while (!rm1.EndOfStream)
            {
                string line = rm1.ReadLine();
                string[] tokens = line.Split(',');

                string modSet = tokens[1].PadLeft(2,'0');
                string modNo = tokens[2];

                if(setToDo == modSet && modToDo == modNo)
                {
                    string mod = tokens[0];
                    string id = mod.ToUpper();
                    id = id.Replace('/', '_');
                    id = id.Replace(' ', '_');
                    id = id.Replace('-', '_');
                    id = id.Replace('(', '_');
                    id = id.Replace(')', '_');
                    id = id.Trim();
                    id = id + "_MOD";

                    string modCode = tokens[3].PadLeft(2, '0');

                    WriteModifier(w, id, mod, modCode.Substring(0,1), modCode.Substring(1,1), setToDo + modCode + modToDo + ".svg");
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

            WriteCode(w, "SymbolSetCode", ssCode.Substring(0, 1), ssCode.Substring(1, 1));

            if (legacyCode != "")
            {
                w.WriteLine("<LegacyCodingSchemeCode Name=\"2525C\">" + legacyCode + "</LegacyCodingSchemeCode>");
            }

            w.WriteLine("<Entities>");
            w.Flush();

            int mode = -1;
           
            while(!r.EndOfStream)
            {
                line = r.ReadLine();

                string[] tokens = line.Split(',');

                ss = tokens[0];
                entity = tokens[1];
                entityType = tokens[2];
                entitySubType = tokens[3];

                if (ss == ssCode)
                {
                    codeE = tokens[4].Substring(0, 2);
                    codeET = tokens[4].Substring(2, 2);
                    codeEST = tokens[4].Substring(4, 2);

                    graphic = ss + tokens[4] + ".svg";

                    if (entityType == "")
                    {
                        id = entity.ToUpper();
                        id = id.Replace('/', '_');
                        id = id.Replace(' ', '_');
                        id = id.Replace('-', '_');
                        id = id.Replace('(', '_');
                        id = id.Replace(')', '_');
                        id = id.Trim();

                        WriteEntity(ref mode, w, id, entity, codeE.Substring(0, 1), codeE.Substring(1, 1), graphic);
                    }
                    else
                    {
                        if (entitySubType == "")
                        {
                            id = entityType.ToUpper();
                            id = id.Replace('/', '_');
                            id = id.Replace(' ', '_');
                            id = id.Replace('-', '_');
                            id = id.Replace('(', '_');
                            id = id.Replace(')', '_');
                            id = id.Trim();

                            WriteEntityType(ref mode, w, id, entityType, codeET.Substring(0, 1), codeET.Substring(1, 1), graphic);
                        }
                        else
                        {
                            id = entitySubType.ToUpper();
                            id = id.Replace('/', '_');
                            id = id.Replace(' ', '_');
                            id = id.Replace('-', '_');
                            id = id.Replace('(', '_');
                            id = id.Replace(')', '_');
                            id = id.Trim();

                            WriteEntitySubType(ref mode, w, id, entitySubType, codeEST.Substring(0, 1), codeEST.Substring(1, 1), graphic);
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
            WriteModifiers(modPath, w, ssCode, "1");
            w.WriteLine("</SectorOneModifiers>");

            w.WriteLine("<SectorTwoModifiers>");
            WriteModifiers(modPath, w, ssCode, "2");
            w.WriteLine("</SectorTwoModifiers>");

            w.WriteLine("</SymbolSet>");
            w.Flush();
        }

        internal void LogConversionResult(string converting)
        {
            if (_logConversion)
            {
                string s = Convert.ToString(0 - _statusFlag, 2);
                char[] bits = s.PadLeft(17, '0').ToCharArray();
                Array.Reverse(bits);

                logger.Info("----------------------------");
                logger.Info("Converting: " + converting +  " (" + _statusFlag + ")");

                for (int i = 0; i < 17; i++)
                {
                    if (bits[i] == '1')
                    {
                        logger.Info(_statusMessages[i]);
                    }
                }

                logger.Info("----------------------------");
            }
        }

        internal void ResetStatusCode()
        {
            _statusFlag = 0;
        }

        internal int StatusCode
        {
            get
            {
                return _statusFlag;
            }
        }

        internal Library Library 
        {
            get
            {
                return this._library;
            }
        }

        internal string Graphics
        {
            get
            {
                return this._configData.graphicPath;
            }
        }

        internal LibraryAmplifier SymbolAmplifier(string id)
        {
            LibraryAmplifier retObj = null;

            foreach (LibraryAmplifier lObj in this._library.Amplifiers)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            logger.Warn("Label " + id + " was not found in the base library file.");

            return retObj;
        }

        internal LibraryDrawRule DrawRule(string id)
        {
            LibraryDrawRule retObj = null;

            foreach (LibraryDrawRule lObj in this._library.DrawRules)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            logger.Warn("DrawRule " + id + " was not found in the base library file.");

            return retObj;
        }

        internal LibraryVersion Version(ushort codeOne, ushort codeTwo)
        {
            LibraryVersion retObj = null;

            foreach (LibraryVersion lObj in this._library.Versions)
            {
                if (lObj.VersionCode.DigitOne == codeOne &&
                    lObj.VersionCode.DigitTwo == codeTwo)
                {
                    return lObj;
                }
            }

            _statusFlag -= 1;

            return retObj;
        }

        internal LibraryContext Context(ushort code)
        {
            LibraryContext retObj = null;

            foreach (LibraryContext lObj in this._library.Contexts)
            {
                if (lObj.ContextCode == code)
                {
                    return lObj;
                }
            }

            _statusFlag -= 2;

            return retObj;
        }

        internal LibraryContext Context(string id)
        {
            LibraryContext retObj = null;

            foreach (LibraryContext lObj in this._library.Contexts)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            _statusFlag -= 2;

            return retObj;
        }

        internal LibraryDimension Dimension(string id)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            _statusFlag -= 4;

            return retObj;
        }

        internal LibraryDimension DimensionBySymbolSet(string symbolSetID)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                foreach(LibraryDimensionSymbolSetRef ssRef in lObj.SymbolSets)
                {
                    if (ssRef.ID == symbolSetID)
                    {
                        return lObj;
                    }
                }
            }

            _statusFlag -= 4;

            return retObj;
        }

        internal LibraryDimension DimensionByLegacyCode(string code)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                foreach (LegacyLetterCodeType lObj2 in lObj.LegacyDimensionCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            _statusFlag -= 4;

            return retObj;
        }

        internal LibraryStandardIdentity StandardIdentity(ushort code)
        {
            LibraryStandardIdentity retObj = null;

            foreach (LibraryStandardIdentity lObj in this._library.StandardIdentities)
            {
                if (lObj.StandardIdentityCode == code)
                {
                    return lObj;
                }
            }

            _statusFlag -= 8;

            return retObj;
        }

        internal LibraryStandardIdentity StandardIdentity(string id)
        {
            LibraryStandardIdentity retObj = null;

            foreach (LibraryStandardIdentity lObj in this._library.StandardIdentities)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            _statusFlag -= 8;

            return retObj;
        }

        internal SymbolSet SymbolSet(ushort symbolSetCodeOne, ushort symbolSetCodeTwo)
        {
            SymbolSet retObj = null;

            foreach (SymbolSet lObj in this._symbolSets)
            {
                if (lObj.SymbolSetCode.DigitOne == symbolSetCodeOne &&
                    lObj.SymbolSetCode.DigitTwo == symbolSetCodeTwo)
                {
                    return lObj;
                }
            }

            _statusFlag -= 16;

            return retObj;
        }

        internal SymbolSet SymbolSet(string dimensionID, string code)
        {
            SymbolSet retObj = null;

            foreach (SymbolSet lObj in this._symbolSets)
            {
                if (lObj.DimensionID == dimensionID)
                {
                    if (lObj.LegacySymbols != null)
                    {
                        foreach (SymbolSetLegacySymbol lObj2 in lObj.LegacySymbols)
                        {
                            foreach (LegacyFunctionCodeType lObj3 in lObj2.LegacyFunctionCode)
                            {
                                if (lObj3.Value == code)
                                {
                                    return lObj;
                                }
                            }
                        }
                    }
                }
            }

            _statusFlag -= 16;

            return retObj;
        }

        internal LibraryStatus Status(ushort code)
        {
            LibraryStatus retObj = null;

            foreach (LibraryStatus lObj in this._library.Statuses)
            {
                if (lObj.StatusCode == code)
                {
                    return lObj;
                }
            }

            _statusFlag -= 32;

            return retObj;
        }

        internal LibraryStatus Status(string code)
        {
            LibraryStatus retObj = null;

            foreach (LibraryStatus lObj in this._library.Statuses)
            {
                if (lObj.LegacyStatusCode != null)
                {
                    foreach (LegacyLetterCodeType lObj2 in lObj.LegacyStatusCode)
                    {
                        if (lObj2.Value == code)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 32;

            return retObj;
        }

        internal LibraryHQTFDummy HQTFDummy(ushort code)
        {
            LibraryHQTFDummy retObj = null;

            foreach (LibraryHQTFDummy lObj in this._library.HQTFDummies)
            {
                if (lObj.HQTFDummyCode == code)
                {
                    return lObj;
                }
            }

            _statusFlag -= 64;

            return retObj;
        }

        internal LibraryHQTFDummy HQTFDummy(string code)
        {
            LibraryHQTFDummy retObj = null;

            foreach (LibraryHQTFDummy lObj in this._library.HQTFDummies)
            {
                if (lObj.LegacyHQTFDummyCode != null)
                {
                    foreach (LegacyLetterCodeType lObj2 in lObj.LegacyHQTFDummyCode)
                    {
                        if (lObj2.Value == code)
                        {
                            return lObj;
                        }
                    }
                }
            }

            if(retObj == null)
            {
                retObj = this.HQTFDummy(0);
            }

            return retObj;
        }

        internal LibraryAmplifierGroup AmplifierGroup(ushort code)
        {
            LibraryAmplifierGroup retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                if (lObj.AmplifierGroupCode == code)
                {

                    return lObj;
                }
            }

            _statusFlag -= 128;

            return retObj;
        }

        internal LibraryAmplifierGroup AmplifierGroup(LibraryAmplifierGroupAmplifier amplifier)
        {
            LibraryAmplifierGroup retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                foreach (LibraryAmplifierGroupAmplifier lObj2 in lObj.Amplifiers)
                {
                    if (lObj2.Equals(amplifier))
                    {
                        return lObj;
                    }
                }
            }

            _statusFlag -= 128;

            return retObj;
        }

        internal LibraryAmplifierGroupAmplifier Amplifier(LibraryAmplifierGroup group, ushort code)
        {
            LibraryAmplifierGroupAmplifier retObj = null;

            if (group != null)
            {
                foreach (LibraryAmplifierGroupAmplifier lObj in group.Amplifiers)
                {
                    if (lObj.AmplifierCode == code)
                    {
                        return lObj;
                    }
                }
            }

            _statusFlag -= 256;

            return retObj;
        }

        internal LibraryAmplifierGroupAmplifier Amplifier(ushort groupCode, ushort code)
        {
            LibraryAmplifierGroup group = this.AmplifierGroup(groupCode);

            LibraryAmplifierGroupAmplifier retObj = this.Amplifier(group, code);

            return retObj;
        }

        internal LibraryAmplifierGroupAmplifier Amplifier(string code)
        {
            LibraryAmplifierGroupAmplifier retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                if (lObj.Amplifiers != null)
                {
                    foreach (LibraryAmplifierGroupAmplifier lObj2 in lObj.Amplifiers)
                    {
                        if (lObj2.LegacyModifierCode != null)
                        {
                            foreach (LegacyLetterCodeType lObj3 in lObj2.LegacyModifierCode)
                            {
                                if (lObj3.Value == code)
                                {
                                    return lObj2;
                                }
                            }
                        }
                    }
                }
            }

            _statusFlag -= 256;

            return retObj;
        }

        internal LibraryAffiliation Affiliation(string contextID, string dimensionID, string standardIdentityID)
        {
            LibraryAffiliation retObj = null;

            foreach (LibraryAffiliation lObj in this._library.Affiliations)
            {
                if (lObj.ContextID == contextID && lObj.DimensionID == dimensionID && lObj.StandardIdentityID == standardIdentityID)
                {
                    return lObj;
                }
            }

            _statusFlag -= 512;

            return retObj;
        }

        internal LibraryAffiliation Affiliation(string legacyStandardIdentityCode, string legacyDimensionCode)
        {
            LibraryAffiliation retObj = null;

            foreach(LibraryAffiliation lObj in this._library.Affiliations)
            {
                if (lObj.LegacyStandardIdentityCode != null)
                {
                    foreach (LegacyLetterCodeType lObj2 in lObj.LegacyStandardIdentityCode)
                    {
                        if (lObj2.Value == legacyStandardIdentityCode)
                        {
                            LibraryDimension lDim = this.Dimension(lObj.DimensionID);
                            
                            foreach (LegacyLetterCodeType lObj3 in lDim.LegacyDimensionCode)
                            {
                                if(lObj3.Value == legacyDimensionCode)
                                    return lObj;
                            }
                        }
                    }
                }
            }

            _statusFlag -= 512;

            return retObj;
        }

        internal LibraryContextContextAmplifier ContextAmplifier(LibraryContext context, ShapeType shape)
        {
            LibraryContextContextAmplifier retObj = null;

            if (context != null)
            {
                if (context.ContextAmplifiers != null)
                {
                    foreach (LibraryContextContextAmplifier lObj in context.ContextAmplifiers)
                    {
                        if (lObj.Shape == shape)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 1024;

            return retObj;
        }

        internal SymbolSetEntity Entity(SymbolSet symbolSet, ushort entityCodeOne, ushort entityCodeTwo)
        {
            SymbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.Entities != null)
                {
                    foreach (SymbolSetEntity lObj in symbolSet.Entities)
                    {
                        if (lObj.EntityCode.DigitOne == entityCodeOne &&
                            lObj.EntityCode.DigitTwo == entityCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 2048;

            return retObj;
        }

        internal SymbolSetEntity Entity(SymbolSet symbolSet, string entityID)
        {
            SymbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.Entities != null)
                {
                    foreach (SymbolSetEntity lObj in symbolSet.Entities)
                    {
                        if (lObj.ID == entityID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 2048;

            return retObj;
        }

        internal SymbolSetEntityEntityType EntityType(SymbolSetEntity entity, ushort entityTypeCodeOne, ushort entityTypeCodeTwo)
        {
            SymbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.EntityTypes != null)
                {
                    foreach (SymbolSetEntityEntityType lObj in entity.EntityTypes)
                    {
                        if (lObj.EntityTypeCode.DigitOne == entityTypeCodeOne &&
                            lObj.EntityTypeCode.DigitTwo == entityTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 4096;

            return retObj;
        }

        internal SymbolSetEntityEntityType EntityType(SymbolSetEntity entity, string entityTypeID)
        {
            SymbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.EntityTypes != null)
                {
                    foreach (SymbolSetEntityEntityType lObj in entity.EntityTypes)
                    {
                        if (lObj.ID == entityTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 4096;

            return retObj;
        }

        internal SymbolSetEntityEntityTypeEntitySubType EntitySubType(SymbolSetEntityEntityType entityType, ushort entitySubTypeCodeOne, ushort entitySubTypeCodeTwo)
        {
            SymbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.EntitySubTypes != null)
                {
                    foreach (SymbolSetEntityEntityTypeEntitySubType lObj in entityType.EntitySubTypes)
                    {
                        if (lObj.EntitySubTypeCode.DigitOne == entitySubTypeCodeOne &&
                            lObj.EntitySubTypeCode.DigitTwo == entitySubTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 8192;

            return retObj;
        }

        internal SymbolSetEntityEntityTypeEntitySubType EntitySubType(SymbolSetEntityEntityType entityType, string entitySubTypeID)
        {
            SymbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.EntitySubTypes != null)
                {
                    foreach (SymbolSetEntityEntityTypeEntitySubType lObj in entityType.EntitySubTypes)
                    {
                        if (lObj.ID == entitySubTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            _statusFlag -= 8192;

            return retObj;
        }

        internal ModifiersTypeModifier ModifierOne(SymbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if (symbolSet.SectorOneModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorOneModifiers)
                    {
                        if (lObj.ModifierCode.DigitOne == modifierCodeOne &&
                           lObj.ModifierCode.DigitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            _statusFlag -= 16384;

            return retObj;
        }

        internal ModifiersTypeModifier ModifierOne(SymbolSet symbolSet, string modifierID)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorOneModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorOneModifiers)
                    {
                        if (lObj.ID == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            _statusFlag -= 16384;

            return retObj;
        }

        internal ModifiersTypeModifier ModifierTwo(SymbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorTwoModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorTwoModifiers)
                    {
                        if (lObj.ModifierCode.DigitOne == modifierCodeOne &&
                           lObj.ModifierCode.DigitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            _statusFlag -= 32768;

            return retObj;
        }

        internal ModifiersTypeModifier ModifierTwo(SymbolSet symbolSet, string modifierID)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorTwoModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorTwoModifiers)
                    {
                        if (lObj.ID == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            _statusFlag -= 32768;

            return retObj;
        }

        internal SymbolSetLegacySymbol LegacySymbol(SymbolSet symbolSet, string functionCode)
        {
            SymbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.LegacySymbols != null)
                {
                    foreach (SymbolSetLegacySymbol lObj in symbolSet.LegacySymbols)
                    {
                        foreach (LegacyFunctionCodeType lObj2 in lObj.LegacyFunctionCode)
                        {
                            if (lObj2.Value == functionCode)
                            {
                                return lObj;
                            }
                        }
                    }
                }
            }

            _statusFlag -= 65536;

            return retObj;
        }

        internal SymbolSetLegacySymbol LegacySymbol(SymbolSet symbolSet, 
                                                  SymbolSetEntity entity, 
                                                  SymbolSetEntityEntityType entityType, 
                                                  SymbolSetEntityEntityTypeEntitySubType entitySubType, 
                                                  ModifiersTypeModifier modifierOne, 
                                                  ModifiersTypeModifier modifierTwo)
        {
            SymbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.LegacySymbols != null)
                {
                    int match = 0;

                    foreach (SymbolSetLegacySymbol lObj in symbolSet.LegacySymbols)
                    {
                        if(entity != null)
                        {
                            if (lObj.EntityID != "NA")
                            {
                                if (lObj.EntityID == entity.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntityID == "NA")
                            match++;

                        if(entityType != null)
                        {
                            if (lObj.EntityTypeID != "NA")
                            {
                                if (lObj.EntityTypeID == entityType.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntityTypeID == "NA")
                            match++;

                        if(entitySubType != null)
                        {
                            if (lObj.EntitySubTypeID != "NA")
                            {
                                if (lObj.EntitySubTypeID == entitySubType.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntitySubTypeID == "NA")
                            match++;

                        if(modifierOne != null)
                        {
                            if (lObj.ModifierOneID != "NA")
                            {
                                if (lObj.ModifierOneID == modifierOne.ID)
                                    match++;
                            }
                        }
                        else if(lObj.ModifierOneID == "NA")
                            match++;

                        if(modifierTwo != null)
                        {
                            if (lObj.ModifierTwoID != "NA")
                            {
                                if (lObj.ModifierTwoID == modifierTwo.ID)
                                    match++;
                            }
                        }
                        else if(lObj.ModifierTwoID == "NA")
                            match++;

                        if(match == 5)
                        {
                            return lObj;
                        }
                        
                        match = 0;
                    }
                }
            }

            _statusFlag -= 65536;

            return retObj;
        }

        public Symbol InvalidSymbol
        {
            get
            {
                return _invalidSymbol;
            }
        }

        public Symbol RetiredSymbol
        {
            get
            {
                return _retiredSymbol;
            }
        }

        public bool IsLogging
        {
            get
            {
                return _logConversion;
            }

            set
            {
                _logConversion = value;
            }
        }

        public Symbol MakeSymbol(string legacyStandard, string legacySIDC)
        {
            Symbol s = null;

            if (legacySIDC.Length == 15)
            {
                s = new Symbol(this, legacyStandard, legacySIDC);

                if (s.SymbolStatus == SymbolStatusEnum.statusEnumInvalid)
                {
                    logger.Warn("SIDC " + legacySIDC + " is an invalid symbol.");
                    s = null;
                }
            }
            else
            {
                logger.Error("SIDC " + legacySIDC + " is not 15 characters in length.");
            }

            return s;
        }

        public Symbol MakeSymbol(UInt32 partA, UInt32 partB)
        {
            SIDC sid = new SIDC(partA, partB);
            Symbol s = new Symbol(this, sid);

            if (s.SymbolStatus == SymbolStatusEnum.statusEnumInvalid)
            {
                s = null;
            }

            return s;
        }

        public Symbol MakeSymbol(SIDC sidc)
        {
            Symbol s = new Symbol(this, sidc);

            if (s.SymbolStatus == SymbolStatusEnum.statusEnumInvalid)
            {
                s = null;
            }

            return s;
        }

        public void Export(string path, string symbolSetExpression = "", string expression = "", bool exportPoints = true, bool exportLines = true, bool exportAreas = true, bool asCodedDomain = false)
        {
            _exportEntities(path + "_Entities.csv", symbolSetExpression, expression, exportPoints, exportLines, exportAreas, asCodedDomain);
            _exportModifiers(path + "_Modifiers.csv", symbolSetExpression, expression, asCodedDomain);
        }

        public void ExportDomains(string path, bool asEsri)
        {
            _exportContext(path, asEsri);
            _exportStandardIdentity(path, asEsri);
            _exportSymbolSet(path, asEsri);
            _exportStatus(path, asEsri);
            _exportHQTFDummy(path, asEsri);
            _exportAmplifier(path, asEsri);
        }

        public void Import(string path, string modPath, string symbolsetCode, string legacyCode)
        {
            _importCSV(path, modPath, symbolsetCode, legacyCode);
        }
    }
}
