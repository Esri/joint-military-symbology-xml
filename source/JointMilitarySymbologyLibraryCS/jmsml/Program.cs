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
using System.Diagnostics;
using System.Linq;
using System.Text;
using JointMilitarySymbologyLibrary;

namespace jmsml
{
    static class Program
    {
        private static Librarian _librarian = new Librarian();
        private static ETL _etl = new ETL(_librarian);
        private static Dictionary<string, ETLExportEnum> _exportAsLookup = new Dictionary<string, ETLExportEnum>();

        static void Run(string[] args)
        {
            _librarian.IsLogging = true;

            CommandLineArgs.I.parseArgs(args, "/e=false;/+=false;/-source=false;/-legacy=false;/xas=SIMPLE;/size=32;/asOriginal=false;/+amplifiers=false");

            string exportPath = CommandLineArgs.I.argAsString("/xe");
            string exportLegacyEntityBc2Path = CommandLineArgs.I.argAsString("/xleBc2");
            string exportLegacyEntityCPath = CommandLineArgs.I.argAsString("/xleC");
            string exportLegacyFrameBc2Path = CommandLineArgs.I.argAsString("/xlfBc2");
            string exportDomainPath = CommandLineArgs.I.argAsString("/b");
            string symbolSet = CommandLineArgs.I.argAsString("/s");
            string query = CommandLineArgs.I.argAsString("/q");
            string dimensionQuery = CommandLineArgs.I.argAsString("/qd");
            string identityQuery = CommandLineArgs.I.argAsString("/qi");
            string contextQuery = CommandLineArgs.I.argAsString("/qc");
            string help = CommandLineArgs.I.argAsString("/?");
            string xPoints = CommandLineArgs.I.argAsString("/p");
            string xLines = CommandLineArgs.I.argAsString("/l");
            string xAreas = CommandLineArgs.I.argAsString("/a");
            string importPath = CommandLineArgs.I.argAsString("/i");
            string legacyCode = CommandLineArgs.I.argAsString("/lc");
            string modPath = CommandLineArgs.I.argAsString("/m");
            string framePath = CommandLineArgs.I.argAsString("/xf");
            string amplifierPath = CommandLineArgs.I.argAsString("/xa");
            string contextPath = CommandLineArgs.I.argAsString("/xc");
            string hqTFFDPath = CommandLineArgs.I.argAsString("/xh");
            string ocaPath = CommandLineArgs.I.argAsString("/xo");
            string exportAs = CommandLineArgs.I.argAsString("/xas").ToUpper();
            string exportAVDPath = CommandLineArgs.I.argAsString("/xavd");
            string exportSchemas = CommandLineArgs.I.argAsString("/xschemas");

            string exportLegacy = CommandLineArgs.I.argAsString("/xl");
            string exportLookup = CommandLineArgs.I.argAsString("/xll");

            string legacyDest = CommandLineArgs.I.argAsString("/ild");
            string legacySrc = CommandLineArgs.I.argAsString("/ils");

            long size = CommandLineArgs.I.argAsLong("/size"); 

            bool dataValidation = (CommandLineArgs.I.argAsString("/e") != "false");
            bool appendFiles = (CommandLineArgs.I.argAsString("/+") != "false");
            bool omitSource = (CommandLineArgs.I.argAsString("/-source") != "false");
            bool omitLegacyTag = (CommandLineArgs.I.argAsString("/-legacy") != "false");
            bool asOriginal = (CommandLineArgs.I.argAsString("/asOriginal") != "false");
            bool includeAmplifiers = (CommandLineArgs.I.argAsString("/+amplifiers") != "false");

            if (help == "/?")
            {
                Console.WriteLine("jmsml.exe - Usage - Command line options are:");
                Console.WriteLine("");
                Console.WriteLine("/?\t\t\t: Help/Show command line options.");
                Console.WriteLine("");
                Console.WriteLine("/b\t\t\t: Export all coded base domain tables to a folder.");
                Console.WriteLine("/e\t\t\t: Add data validation when exporting domain tables.");
                Console.WriteLine("");
                Console.WriteLine("/i\t\t\t: Import raw data into stubbed out symbol set XML.");
                Console.WriteLine("/m\t\t\t: Path to a modifier file created when importing data.");
                Console.WriteLine("");
                Console.WriteLine("/ild\t\t\t: Import raw legacy data into an existing symbol set.");
                Console.WriteLine("/ils\t\t\t: The source file holding legacy data to be imported.");
                Console.WriteLine("");
                Console.WriteLine("/q=\"<expression>\"\t: Use regular expression to query on labels.");
                Console.WriteLine("/qc=\"<expression>\"\t: Use regular expression to query on context.");
                Console.WriteLine("/qd=\"<expression>\"\t: Use regular expression to query on dimension.");
                Console.WriteLine("/qi=\"<expression>\"\t: Use regular expression to query on standard identity.");
                Console.WriteLine("/s=\"<expression>\"\t: Use regular expression to query on symbol set labels.");
                Console.WriteLine("");
                Console.WriteLine("/xa=\"<pathname>\"\t: Export amplifiers.");
                Console.WriteLine("/xc=\"<pathname>\"\t: Export contexts.");
                Console.WriteLine("/xe=\"<pathname>\"\t: Export entities and modifiers.");
                Console.WriteLine("/xf=\"<pathname>\"\t: Export frames.");
                Console.WriteLine("/xh=\"<pathname>\"\t: Export HQ/TF/FD.");
                Console.WriteLine("/xl=\"<pathname>\"\t: Export legacy data (for testing).");
                Console.WriteLine("/xleBc2=\"<pathname>\"\t: Export legacy entities for 2525B Change 2.");
                Console.WriteLine("/xleC=\"<pathname>\"\t: Export legacy entities for 2525C.");
                Console.WriteLine("/xlfBc2=\"<pathname>\"\t: Export legacy frames for 2525B Change 2.");
                Console.WriteLine("/xll=\"<pathname>\"\t: Export legacy lookup table for 2525C.");
                Console.WriteLine("/xo=\"<pathname>\"\t: Export operational condition amplifiers.*");
                Console.WriteLine("/xas=\"<as_option>\"\t: Export as SIMPLE, DOMAIN, or IMAGE.");
                Console.WriteLine("");
                Console.WriteLine("/xavd=\"<pathname>\"t: Export amplifier value domains.");
                Console.WriteLine("");
                Console.WriteLine("/xschemas=\"<pathname>\"t: Export all schemas.");
                Console.WriteLine("");
                Console.WriteLine("/+\t\t\t: Append multiple e(x)port files together.");
                Console.WriteLine("/-source\t\t: Leave source file out of exported tags.");
                Console.WriteLine("/size=\"<pixels>\"\t: Specify the size for exported image information.");
                Console.WriteLine("/asOriginal\t\t: Map legacy SIDCs to original symbol components.");
                Console.WriteLine("/+amplifiers");
                Console.WriteLine("");
                Console.WriteLine("/a\t\t\t: Export symbols with AREA geometry.");
                Console.WriteLine("/l\t\t\t: Export symbols with LINE geometry.");
                Console.WriteLine("/p\t\t\t: Export symbols with POINT geometry.");
                Console.WriteLine("");
                Console.WriteLine("* - <pathname> argument has two comma seperated values.");
                Console.WriteLine("");
                Console.WriteLine("<Enter> to continue.");
                Console.ReadLine();
            }

            _exportAsLookup.Add("SIMPLE", ETLExportEnum.ETLExportSimple);
            _exportAsLookup.Add("DOMAIN", ETLExportEnum.ETLExportDomain);
            _exportAsLookup.Add("IMAGE", ETLExportEnum.ETLExportImage);

            ETLExportEnum _exportThisAs = _exportAsLookup[exportAs];

            if (exportLegacy != "")
            {
                _etl.ExportLegacy(exportLegacy);
            }

            if (exportLookup != "")
            {
                _etl.ExportLegacyLookup(exportLookup, "2525C", asOriginal, includeAmplifiers, appendFiles);
            }

            if (exportPath != "")
            {
                _etl.Export(exportPath, symbolSet, query, xPoints == "/p" || xLines == "" && xAreas == "", 
                                                                xLines == "/l" || xPoints == "" && xAreas == "",
                                                                xAreas == "/a" || xPoints == "" && xLines == "",
                                                                _exportThisAs,
                                                                appendFiles,
                                                                omitSource,
                                                                omitLegacyTag,
                                                                size);
            }

            if (exportLegacyEntityBc2Path != "")
            {
                _etl.ExportLegacyEntities(exportLegacyEntityBc2Path, "2525BC2", size);
            }

            if (exportLegacyEntityCPath != "")
            {
                _etl.ExportLegacyEntities(exportLegacyEntityCPath, "2525C", size);
            }

            if (exportLegacyFrameBc2Path != "")
            {
                _etl.ExportLegacyFrames(exportLegacyFrameBc2Path, "2525Bc2", size, appendFiles);
            }

            if (exportDomainPath != "")
            {
                _etl.ExportDomains(exportDomainPath, dataValidation, appendFiles);
            }

            if (importPath != "")
            {
                _etl.Import(importPath, modPath, symbolSet, legacyCode);
            }

            if (framePath != "")
            {
                _etl.ExportFrames(framePath, contextQuery, identityQuery, dimensionQuery, _exportThisAs, appendFiles, omitSource, omitLegacyTag, size);
            }

            if (amplifierPath != "")
            {
                _etl.ExportAmplifiers(amplifierPath, _exportThisAs, appendFiles, omitSource, omitLegacyTag, size, query);
            }

            if (contextPath != "")
            {
                _etl.ExportContext(contextPath, dataValidation, appendFiles);
            }

            if (hqTFFDPath != "")
            {
                _etl.ExportHQTFFD(hqTFFDPath, _exportThisAs, appendFiles, omitSource, omitLegacyTag, size);
            }

            if (ocaPath != "")
            {
                string[] paths = ocaPath.Split(',');

                if (paths.Count() == 2)
                {
                    _etl.ExportOCA(paths[0], paths[1], _exportThisAs, appendFiles, omitSource, omitLegacyTag, size);
                }
                else
                {
                    _etl.ExportOCA(ocaPath, "", _exportThisAs, appendFiles, omitSource, omitLegacyTag, size);
                }
            }

            if (legacySrc != "" && legacyDest != "")
            {
                _etl.ImportLegacyData(legacySrc, legacyDest);
            }

            if (exportAVDPath != "")
            {
                _etl.ExportAmplifierValueDomains(exportAVDPath, appendFiles);
            }

            if (exportSchemas != "")
            {
                _etl.ExportSchemas(exportSchemas);
            }
        }

        static int Main(string[] args)
        {
            // TODO Use a more robust arguments parser
            if (args.Any(arg => arg.Equals("/v") || arg.Equals("-v"))) // verbose?
                Trace.Listeners.Add(new ConsoleTraceListener(true));

            try
            {
                Run(args);
                return Environment.ExitCode;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Trace.TraceError(e.ToString());

                return Environment.ExitCode != 0
                     ? Environment.ExitCode : 100;
            }
        }
    }
}
