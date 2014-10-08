/*
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
using System.Data;
using System.IO;
using JointMilitarySymbologyLibrary;

namespace TestLegacyCodesCsv
{
    /// <summary>
    /// Sample code that loads a .csv file with 2525C attributes 
    /// and executes a Linq query on one of those attributes
    /// csv file generated from public data at https://github.com/Esri/military-features-data
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            DataTable symbolTable = new DataTable();

            // Warning: this order must match the .csv
            // Mil-2525C-Wildcards.csv format
            // Full SIDC,SIDC By Parts,Hierarchy Code,Name,Geometry
            symbolTable.Columns.Add("SymbolId");
            symbolTable.Columns.Add("SymbolIdByParts");
            symbolTable.Columns.Add("HierarchyCode");
            symbolTable.Columns.Add("Name");
            symbolTable.Columns.Add("Geometry");

            string csvFileName = @"Data\Mil-2525C-Wildcards.csv";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string csvFullPath = System.IO.Path.Combine(basePath, csvFileName);

            foreach (string line in File.ReadLines(csvFullPath))
            {
                if (line.StartsWith("#"))
                    continue;

                string[] values = line.Split(',');
                if (values.Length >= 4)
                {
                    string symbolId = values[0];
                    string symbolIdByParts = values[1];
                    string hierarchyCode = values[2];
                    string name = values[3];

                    // convention used in test data: pt, pl, pg (point, polyline, polygon)
                    string geometry = "pt"; // not every row/entry has this so default to "pt"
                    if (values.Length > 4)
                        geometry = values[4];

                    symbolTable.Rows.Add(symbolId, symbolIdByParts, name, geometry);
                }
            }

            System.Console.WriteLine("Check Debug/Trace Window for Output");

            Librarian librarian = new Librarian(string.Empty);

            var results = from row in symbolTable.AsEnumerable() select row;

            int resultCount = results.Count();

            foreach (DataRow row in results)
            {
                string name = row["Name"] as string;
                string symbolId = row["SymbolId"] as string;

                StringBuilder sb = new StringBuilder(symbolId);

                // Replace wildcards used
                if (symbolId[1] == '*')
                    sb[1] = 'F';
                if (symbolId[3] == '*')
                    sb[3] = 'P';

                // System.Diagnostics.Trace.WriteLine("Found Match: " + symbolId + ", " + name + ")");
                symbolId = sb.ToString();
                symbolId = symbolId.Replace("*", "-");

                // TODO: Add any other code needed to test these results here
                Symbol jmsSymbol = librarian.MakeSymbol("2525C", symbolId);

                if (jmsSymbol == null)
                {
                    System.Diagnostics.Trace.WriteLine(symbolId + " ==>> 2525C: " +
                    symbolId + " is null in 2525D.");
                    continue;
                }

                if ((jmsSymbol.SymbolStatus != SymbolStatusEnum.statusEnumOld) &&
                    (jmsSymbol.SymbolStatus != SymbolStatusEnum.statusEnumRetired))
                {
                    System.Diagnostics.Trace.WriteLine("Unexpected Value for: " + symbolId + " ==>> (" +
                    jmsSymbol.SIDC.PartAString + ", " + jmsSymbol.SIDC.PartBString + ")" + " : " + jmsSymbol.SymbolStatus);
                }

                // To see just "Retired" symbols uncomment:
                //if (jmsSymbol.SymbolStatus == SymbolStatusEnum.statusEnumRetired)
                //{
                //    System.Diagnostics.Trace.WriteLine("Retired Symbol: " + symbolId + " ==>> (" +
                //    jmsSymbol.SIDC.PartAString + ", " + jmsSymbol.SIDC.PartBString + ")" + " : " + jmsSymbol.SymbolStatus);
                //}

            } // end for each row

        } // Main
    } // Class 
} //Namespace
