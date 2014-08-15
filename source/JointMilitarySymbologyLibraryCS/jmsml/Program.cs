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

            CommandLineArgs.I.parseArgs(args, "/e=false;/+=false;/-source=false;/xas=SIMPLE");

            string exportPath = CommandLineArgs.I.argAsString("/xe");
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
            string hqTFFDPath = CommandLineArgs.I.argAsString("/xh");
            string ocaPath = CommandLineArgs.I.argAsString("/xo");
            string exportAs = CommandLineArgs.I.argAsString("/xas").ToUpper();

            bool dataValidation = (CommandLineArgs.I.argAsString("/e") != "false");
            bool appendFiles = (CommandLineArgs.I.argAsString("/+") != "false");
            bool omitSource = (CommandLineArgs.I.argAsString("/-source") != "false");

            if (help == "/?")
            {
                Console.WriteLine("jmsml.exe - Usage - Command line options are:");
                Console.WriteLine("");
                Console.WriteLine("/?\t\t\t: Help/Show command line options.");
                Console.WriteLine("/+\t\t\t: Append multiple e(x)port files together.");
                Console.WriteLine("/-source\t\t: Leave source file out of exported tags.");
                Console.WriteLine("/a\t\t\t: Export symbols with AREA geometry.");
                Console.WriteLine("/b\t\t\t: Export all coded base domain tables to a folder.");
                Console.WriteLine("/e\t\t\t: Add data validation when exporting domain tables.");
                Console.WriteLine("/i\t\t\t: Import raw CSV into stubbed out symbol set XML.");
                Console.WriteLine("/l\t\t\t: Export symbols with LINE geometry.");
                Console.WriteLine("/p\t\t\t: Export symbols with POINT geometry.");
                Console.WriteLine("/q=\"<expression>\"\t: Use regular expression to query on labels.");
                Console.WriteLine("/qc=\"<expression>\"\t: Use regular expression to query on context.");
                Console.WriteLine("/qd=\"<expression>\"\t: Use regular expression to query on dimension.");
                Console.WriteLine("/qi=\"<expression>\"\t: Use regular expression to query on standard identity.");
                Console.WriteLine("/s=\"<expression>\"\t: Use regular expression to query on symbol set labels.");
                Console.WriteLine("");
                Console.WriteLine("/xa=\"<pathname>\"\t: Export amplifiers.");
                Console.WriteLine("/xe=\"<pathname>\"\t: Export entities and modifiers.");
                Console.WriteLine("/xf=\"<pathname>\"\t: Export frames.");
                Console.WriteLine("/xh=\"<pathname>\"\t: Export HQ/TF/FD.");
                Console.WriteLine("/xo=\"<pathname>\"\t: Export operational condition amplifiers.");
                Console.WriteLine("/xas=\"<as_option>\"\t: Export as SIMPLE, DOMAIN, or IMAGE.");
                Console.WriteLine("");
                Console.WriteLine("<Enter> to continue.");
                Console.ReadLine();
            }

            _exportAsLookup.Add("SIMPLE", ETLExportEnum.ETLExportSimple);
            _exportAsLookup.Add("DOMAIN", ETLExportEnum.ETLExportDomain);
            _exportAsLookup.Add("IMAGE", ETLExportEnum.ETLExportImage);

            ETLExportEnum _exportThisAs = _exportAsLookup[exportAs];

            if (exportPath != "")
            {
                _etl.Export(exportPath, symbolSet, query, xPoints == "/p" || xLines == "" && xAreas == "", 
                                                                xLines == "/l" || xPoints == "" && xAreas == "",
                                                                xAreas == "/a" || xPoints == "" && xLines == "",
                                                                _exportThisAs,
                                                                appendFiles,
                                                                omitSource);
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
                _etl.ExportFrames(framePath, contextQuery, identityQuery, dimensionQuery, _exportThisAs, appendFiles, omitSource);
            }

            if (amplifierPath != "")
            {
                _etl.ExportAmplifiers(amplifierPath, _exportThisAs, appendFiles, omitSource);
            }

            if (hqTFFDPath != "")
            {
                _etl.ExportHQTFFD(hqTFFDPath, _exportThisAs, appendFiles, omitSource);
            }

            if (ocaPath != "")
            {
                _etl.ExportOCA(ocaPath, _exportThisAs, appendFiles, omitSource);
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
