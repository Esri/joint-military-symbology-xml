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
using System.Threading.Tasks;
using NLog;

namespace JointMilitarySymbologyLibrary
{
    public class LegacyETL
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private ConfigHelper _helper;
        private JMSMLConfigETLConfig _etlConfig;
        private Librarian _lib;
        private List<string> _icons = new List<string>();

        public LegacyETL(ConfigHelper helper, JMSMLConfigETLConfig config)
        {
            _helper = helper;
            _etlConfig = config;

            _lib = _helper.Librarian;
        }

        private int _exportOCAs(StreamWriter w, bool isFirst, string standard, int id)
        {
            IOCAExport ocaExport = new LegacyOCAExport(_helper, standard);

            if (isFirst)
            {
                string headers = ocaExport.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryStatus status in _lib.Library.Statuses)
            {
                logger.Info("Exporting status graphics for: " + status.Name);

                try
                {
                    if (status.Graphic != null)
                    {
                        string line = id.ToString() + "," + ocaExport.Line(status);
                        id++;

                        w.WriteLine(line);
                        w.Flush();
                    }

                    if (status.Graphics != null)
                    {
                        foreach (LibraryStatusGraphic graphic in status.Graphics)
                        {
                            string line = id.ToString() + "," + ocaExport.Line(status, graphic);
                            id++;

                            w.WriteLine(line);
                            w.Flush();
                        }
                    }
                }

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return id;
        }

        private int _exportHQTFFDs(StreamWriter w, bool isFirst, string standard, int id)
        {
            LegacyHQTFFDExport legacyHQTFFD = new LegacyHQTFFDExport(_helper, standard);
            IHQTFFDExport hqTFFDEx = (IHQTFFDExport)legacyHQTFFD;

            if (isFirst)
            {
                string headers = hqTFFDEx.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryHQTFDummy hqtffd in _lib.Library.HQTFDummies)
            {
                logger.Info("Exporting HQTFFD graphics for: " + hqtffd.Name);

                try
                {
                    if (hqtffd.Graphics != null)
                    {
                        foreach (LegacyLetterCodeType legacyCode in hqtffd.LegacyHQTFDummyCode)
                        {
                            foreach (LibraryHQTFDummyGraphic graphic in hqtffd.Graphics)
                            {
                                string line = id.ToString() + "," + legacyHQTFFD.Line(legacyCode.CodingSchemeLetter, hqtffd, graphic);
                                id++;

                                w.WriteLine(line);
                                w.Flush();
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return id;
        }

        private int _exportAmplifiers(StreamWriter w, bool isFirst, string standard, int id)
        {
            LegacyAmplifierExport legacyAmplifierExport = new LegacyAmplifierExport(_helper, standard);
            IAmplifierExport amplifierEx = (IAmplifierExport)legacyAmplifierExport;

            if (isFirst)
            {
                string headers = amplifierEx.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryAmplifierGroup group in _lib.Library.AmplifierGroups)
            {
                logger.Info("Exporting amplifiers for: " + group.Name);

                try
                {
                    foreach (LibraryAmplifierGroupAmplifier amplifier in group.Amplifiers)
                    {
                        if (amplifier.Graphics != null)
                        {
                            foreach (LegacyLetterCodeType legacyModifier in group.LegacyModifierCode)
                            {
                                foreach (LibraryAmplifierGroupAmplifierGraphic graphic in amplifier.Graphics)
                                {
                                    string line = id.ToString() + "," + legacyAmplifierExport.Line(legacyModifier.CodingSchemeLetter, group, amplifier, graphic);
                                    id++;

                                    w.WriteLine(line);
                                    w.Flush();
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return id;
        }

        private int _exportSymbols(StreamWriter w, bool isFirst, string standard, int id, bool asOriginal)
        {
            LegacySymbolExport symbolExport = new LegacySymbolExport(_helper, standard);

            if (isFirst)
            {
                string headers = symbolExport.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }
             
            foreach (SymbolSet ss in _lib.SymbolSets)
            {
                logger.Info("Exporting symbols for: " + ss.ID);

                try
                {
                    if (ss.LegacySymbols != null)
                    {
                        foreach (SymbolSetLegacySymbol legacySymbol in ss.LegacySymbols)
                        {
                            if (!legacySymbol.IsDuplicate)
                            {
                                // This first part handles those legacy symbols being mapped to Current/Latest (2525D) symbols.

                                if (legacySymbol.EntityID != "NA" && legacySymbol.EntityID != "UNSPECIFIED" && (asOriginal == false || legacySymbol.LegacyEntity == null))
                                {
                                    LegacyFunctionCodeType[] functionCodes = _helper.LegacyFunctions(legacySymbol.LegacyFunctionCode, standard);

                                    //if (functionCodes.Count() > 1)
                                    //    logger.Info("LegacyFunctionCode count : " + legacySymbol.ID + " = " + functionCodes.Count());

                                    foreach (LegacyFunctionCodeType functionCode in functionCodes)
                                    {
                                        string line = id.ToString() + "," + symbolExport.Line(ss, legacySymbol, functionCode);
                                        id++;

                                        w.WriteLine(line);
                                        w.Flush();
                                    }
                                }

                                // This second part handles those legacy symbols being mapped to Original symbols or that are Retired and no longer have a
                                // 2525D equivalent.

                                else if ((legacySymbol.Remarks == "Retired" || asOriginal) && legacySymbol.LegacyEntity != null)
                                {
                                    foreach (LegacyEntityType legacyEntity in legacySymbol.LegacyEntity)
                                    {
                                        LegacyFunctionCodeType[] functionCodes = _helper.LegacyFunctions(legacyEntity.LegacyFunctionCode, standard);

                                        foreach (LegacyFunctionCodeType functionCode in functionCodes)
                                        {
                                            if (functionCode.LimitUseTo != "2525Bc2" || asOriginal)
                                            {
                                                string line = id.ToString() + "," + symbolExport.Line(ss, legacySymbol, legacyEntity, functionCode);
                                                id++;

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

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return id;
        }

        private int _exportFrames(StreamWriter w, bool isFirst, string standard, int id, bool asOriginal)
        {
            IFrameExport frameEx = new LegacyFrameExport(_helper, standard);

            if (isFirst)
            {
                string headers = frameEx.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryAffiliation affiliation in _lib.Library.Affiliations)
            {
                logger.Info("Exporting frames for: " + affiliation.ID);

                try
                {
                    if (affiliation.LegacyFrames != null)
                    {
                        LegacyFrameExport fe = (LegacyFrameExport)frameEx;
                        fe.Affiliation = affiliation;

                        foreach (LegacyLetterCodeType legacyFrame in affiliation.LegacyFrames)
                        {
                            if (legacyFrame.LimitUseTo != "2525Bc2" || asOriginal)
                            {
                                if (legacyFrame.Function == "")
                                {
                                    fe.LegacyFrame = legacyFrame;

                                    LibraryContext context = _lib.Context(affiliation.ContextID);
                                    LibraryStandardIdentity identity = _lib.StandardIdentity(affiliation.StandardIdentityID);
                                    LibraryDimension dimension = _lib.Dimension(affiliation.DimensionID);

                                    string line = id.ToString() + "," + frameEx.Line(_lib, context, identity, dimension, _lib.Status(0), false, false);
                                    id++;

                                    w.WriteLine(line);
                                    w.Flush();

                                    if (legacyFrame.IsPlanned)
                                    {
                                        LibraryStatus status = _lib.Status(1);
                                        status.LabelAlias = "Planned";
                                        line = id.ToString() + "," + frameEx.Line(_lib, context, identity, dimension, status, false, false);
                                        status.LabelAlias = "";
                                        id++;

                                        w.WriteLine(line);
                                        w.Flush();
                                    }
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            return id;
        }

        private void _exportLegacyEntities(StreamWriter w, bool isFirst, string standard, long size)
        {
            LegacyEntityExport entityExport = new LegacyEntityExport(_helper, standard, size);

            if (isFirst)
            {
                string headers = entityExport.Headers;
                
                w.WriteLine(headers);
                w.Flush();
            }

            foreach (SymbolSet ss in _lib.SymbolSets)
            {
                // For each symbol set in JMSML...

                logger.Info("Exporting legacy entities for: " + ss.ID);

                try
                {
                    if (ss.LegacySymbols != null)
                    {
                        foreach (SymbolSetLegacySymbol legacySymbol in ss.LegacySymbols)
                        {
                            // For each legacy symbol in that symbol set...

                            if (legacySymbol.LegacyEntity != null)
                            {
                                foreach (LegacyEntityType legacyEntity in legacySymbol.LegacyEntity)
                                {
                                    // Get the list of Legacy Function IDs for the specified legacy version of the standard

                                    if (legacyEntity.LegacyFunctionCode != null)
                                    {
                                        LegacyFunctionCodeType functionCode = _helper.LegacyFunction(legacyEntity.LegacyFunctionCode, standard);

                                        if (functionCode != null)
                                        {

                                            string line = "";

                                            // If the icon is Full Frame then four lines need to be exported, to reflect the four icon shapes.
                                            // Else just write out one line for non-Full-Frame.

                                            if (legacyEntity.Icon == IconType.FULL_FRAME)
                                            {
                                                foreach (LibraryStandardIdentityGroup sig in _lib.Library.StandardIdentityGroups)
                                                {
                                                    line = string.Format("{0}", entityExport.Line(sig, ss, legacySymbol, legacyEntity, functionCode));

                                                    w.WriteLine(line);
                                                    w.Flush();
                                                }
                                            }
                                            else
                                            {
                                                line = string.Format("{0}", entityExport.Line(null, ss, legacySymbol, legacyEntity, functionCode));

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

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        private void _exportLegacyFrames(StreamWriter w, bool isFirst, string standard, long size)
        {
            IFrameExport frameEx = new LegacyFrameGraphicExport(_helper, standard);

            _icons.Clear();

            if (isFirst)
            {
                w.WriteLine(frameEx.Headers);
                w.Flush();
            }

            foreach (LibraryAffiliation affiliation in _lib.Library.Affiliations)
            {
                logger.Info("Exporting legacy frames for: " + affiliation.ID);

                try
                {
                    if (affiliation.LegacyFrames != null)
                    {
                        LegacyFrameGraphicExport fe = (LegacyFrameGraphicExport)frameEx;
                        fe.Affiliation = affiliation;

                        foreach (LegacyLetterCodeType legacyFrame in affiliation.LegacyFrames)
                        {
                            if (legacyFrame.LimitUseTo == standard && legacyFrame.Function == "")
                            {
                                fe.LegacyFrame = legacyFrame;

                                string id = fe.IDIt(_lib.Status(0));

                                if (!_icons.Contains(id))
                                {
                                    _icons.Add(id);

                                    LibraryContext context = _lib.Context(affiliation.ContextID);
                                    LibraryStandardIdentity identity = _lib.StandardIdentity(affiliation.StandardIdentityID);
                                    LibraryDimension dimension = _lib.Dimension(affiliation.DimensionID);

                                    string line = frameEx.Line(_lib, context, identity, dimension, _lib.Status(0), false, false);

                                    w.WriteLine(line);
                                    w.Flush();

                                    if (legacyFrame.IsPlanned)
                                    {
                                        LibraryStatus status = _lib.Status(1);

                                        id = fe.IDIt(status);

                                        if (!_icons.Contains(id))
                                        {
                                            _icons.Add(id);

                                            status.LabelAlias = "Planned";
                                            line = frameEx.Line(_lib, context, identity, dimension, status, false, false);
                                            status.LabelAlias = "";

                                            w.WriteLine(line);
                                            w.Flush();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        public void ExportLegacyLookup(string path, string standard, bool asOriginal, bool includeAmplifiers, bool append)
        {
            using (StreamWriter stream = new StreamWriter(path, append))
            {
                int id = 0;

                id = _exportFrames(stream, true, standard, id, asOriginal);
                id = _exportSymbols(stream, false, standard, id, asOriginal);
                id = _exportSymbols(stream, false, "2525BC2", id, asOriginal);

                if (includeAmplifiers)
                {
                    id = _exportAmplifiers(stream, false, "2525C", id);
                    id = _exportHQTFFDs(stream, false, "2525C", id);
                    id = _exportOCAs(stream, false, "2525C", id);
                }
            }
        }

        public void ExportLegacyEntities(string path, string standard, long size)
        {
            path = path + ".csv";

            bool didFileExist = File.Exists(path);

            using (StreamWriter stream = new StreamWriter(path, didFileExist))
            {
                _exportLegacyEntities(stream, !didFileExist, standard, size);
            }
        }

        public void ExportLegacyFrames(string path, string standard, long size, bool append = false)
        {
            path = path + ".csv";

            using (StreamWriter stream = new StreamWriter(path, append))
            {
                _exportLegacyFrames(stream, !append, standard, size);
            }
        }
    }
}
