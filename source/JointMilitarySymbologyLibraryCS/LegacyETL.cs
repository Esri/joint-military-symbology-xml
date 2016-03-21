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

namespace JointMilitarySymbologyLibrary
{
    public class LegacyETL
    {
        private ConfigHelper _helper;
        private JMSMLConfigETLConfig _etlConfig;
        private Librarian _lib;

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

            return id;
        }

        private int _exportHQTFFDs(StreamWriter w, bool isFirst, string standard, int id)
        {
            IHQTFFDExport hqTFFDEx = new LegacyHQTFFDExport(_helper, standard);

            if (isFirst)
            {
                string headers = hqTFFDEx.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryHQTFDummy hqtffd in _lib.Library.HQTFDummies)
            {
                if (hqtffd.Graphics != null)
                {
                    foreach (LibraryHQTFDummyGraphic graphic in hqtffd.Graphics)
                    {
                        string line = id.ToString() + "," + hqTFFDEx.Line(hqtffd, graphic);
                        id++;

                        w.WriteLine(line);
                        w.Flush();
                    }
                }
            }

            return id;
        }

        private int _exportAmplifiers(StreamWriter w, bool isFirst, string standard, int id)
        {
            IAmplifierExport amplifierEx = new LegacyAmplifierExport(_helper, standard);

            if (isFirst)
            {
                string headers = amplifierEx.Headers;
                headers = "id," + headers;

                w.WriteLine(headers);
                w.Flush();
            }

            foreach (LibraryAmplifierGroup group in _lib.Library.AmplifierGroups)
            {
                foreach (LibraryAmplifierGroupAmplifier amplifier in group.Amplifiers)
                {
                    if (amplifier.Graphics != null)
                    {
                        foreach (LibraryAmplifierGroupAmplifierGraphic graphic in amplifier.Graphics)
                        {
                            string line = id.ToString() + "," + amplifierEx.Line(group, amplifier, graphic);
                            id++;

                            w.WriteLine(line);
                            w.Flush();
                        }
                    }
                }
            }

            return id;
        }

        private int _exportSymbols(StreamWriter w, bool isFirst, string standard, int id)
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
                if (ss.LegacySymbols != null)
                {
                    foreach (SymbolSetLegacySymbol legacySymbol in ss.LegacySymbols)
                    {
                        if (legacySymbol.EntityID != "NA" && legacySymbol.EntityID != "UNSPECIFIED")
                        {
                            LegacyFunctionCodeType[] functionCodes = _helper.LegacyFunctions(legacySymbol.LegacyFunctionCode, standard);

                            foreach (LegacyFunctionCodeType functionCode in functionCodes)
                            {
                                string line = id.ToString() + "," + symbolExport.Line(ss, legacySymbol, functionCode);
                                id++;

                                w.WriteLine(line);
                                w.Flush();
                            }
                        }
                        else if (legacySymbol.Remarks == "Retired" && legacySymbol.LegacyEntity != null)
                        {
                            foreach (LegacyEntityType legacyEntity in legacySymbol.LegacyEntity)
                            {
                                LegacyFunctionCodeType[] functionCodes = _helper.LegacyFunctions(legacyEntity.LegacyFunctionCode, standard);

                                foreach (LegacyFunctionCodeType functionCode in functionCodes)
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

            return id;
        }

        private int _exportFrames(StreamWriter w, bool isFirst, string standard, int id)
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
                if (affiliation.LegacyFrames != null)
                {
                    LegacyFrameExport fe = (LegacyFrameExport)frameEx;
                    fe.Affiliation = affiliation;

                    foreach (LegacyLetterCodeType legacyFrame in affiliation.LegacyFrames)
                    {
                        if (legacyFrame.Name == standard && legacyFrame.Function == "")
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

                                if(legacyEntity.LegacyFunctionCode != null)
                                {
                                    LegacyFunctionCodeType functionCode = _helper.LegacyFunction(legacyEntity.LegacyFunctionCode, standard);

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

        public void ExportLegacyLookup(string path, string standard)
        {
            using (StreamWriter stream = new StreamWriter(path, false))
            {
                int id = 0;

                id = _exportFrames(stream, true, "2525C", id);
                id = _exportSymbols(stream, false, "2525C", id);
                id = _exportAmplifiers(stream, false, "2525C", id);
                id = _exportHQTFFDs(stream, false, "2525C", id);
                id = _exportOCAs(stream, false, "2525C", id);
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
    }
}
