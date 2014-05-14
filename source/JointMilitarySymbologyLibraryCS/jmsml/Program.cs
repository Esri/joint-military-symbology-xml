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

        static void Run(string[] args)
        {
            _librarian.IsLogging = true;

            CommandLineArgs.I.parseArgs(args, "/e=false;/+=false;/-source=false");

            string exportPath = CommandLineArgs.I.argAsString("/x");
            string exportDPath = CommandLineArgs.I.argAsString("/xd");
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
            string imagePath = CommandLineArgs.I.argAsString("/xi");
            string frameImagePath = CommandLineArgs.I.argAsString("/xf");

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
                Console.WriteLine("/x=\"<pathname>\"\t\t: Export entities and modifiers in simple format.");
                Console.WriteLine("/xd=\"<pathname>\"\t: Export entities and modifiers as coded domains.");
                Console.WriteLine("/xf=\"<pathname>\"\t: Export frames for style file input.");
                Console.WriteLine("/xi=\"<pathname>\"\t: Export entities and modifiers for style file input.");
                Console.WriteLine("");
                Console.WriteLine("<Enter> to continue.");
                Console.ReadLine();
            }

            if (exportPath != "")
            {
                _etl.Export(exportPath, symbolSet, query, xPoints == "/p" || xLines == "" && xAreas == "", 
                                                                xLines == "/l" || xPoints == "" && xAreas == "",
                                                                xAreas == "/a" || xPoints == "" && xLines == "",
                                                                ETLExportEnum.ETLExportSimple,
                                                                appendFiles,
                                                                omitSource);
            }

            if (exportDPath != "")
            {
                _etl.Export(exportDPath, symbolSet, query, xPoints == "/p" || xLines == "" && xAreas == "",
                                                                xLines == "/l" || xPoints == "" && xAreas == "",
                                                                xAreas == "/a" || xPoints == "" && xLines == "",
                                                                ETLExportEnum.ETLExportDomain,
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

            if (imagePath != "")
            {
                _etl.Export(imagePath, symbolSet, query, xPoints == "/p" || xLines == "" && xAreas == "",
                                                                xLines == "/l" || xPoints == "" && xAreas == "",
                                                                xAreas == "/a" || xPoints == "" && xLines == "",
                                                                ETLExportEnum.ETLExportImage,
                                                                appendFiles,
                                                                omitSource);
            }

            if (frameImagePath != "")
            {
                _etl.ExportFrames(frameImagePath, contextQuery, identityQuery, dimensionQuery, ETLExportEnum.ETLExportImage, omitSource);
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
