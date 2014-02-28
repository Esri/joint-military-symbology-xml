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
            symbolTable.Columns.Add("Name");
            symbolTable.Columns.Add("SymbolId");
            symbolTable.Columns.Add("StyleFile");
            symbolTable.Columns.Add("Category");
            symbolTable.Columns.Add("GeometryType");
            symbolTable.Columns.Add("Tags");

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string csvFileName = @"Data\SymbolInfo2525C.csv";
            string csvFullPath = System.IO.Path.Combine(basePath, csvFileName);

            foreach (string line in File.ReadLines(csvFullPath))
            {
                string[] values = line.Split(',');
                if (values.Length >= 6)
                {
                    string name = values[0];
                    string symbolId = values[1];
                    string styleFile = values[2];
                    string category = values[3];
                    string geoType = values[4];
                    string tags = values[5];

                    symbolTable.Rows.Add(name, symbolId, styleFile, category, geoType, tags);
                }
            }

            List<string> styleFiles = new List<string>() {"C2 UEI Air Track.style"}; 
            // TODO: add in the rest as they are avaliable: 
            /* 
            "C2 Military Operations.style", 
            "C2 UEI Ground Track Equipment.style",
            "C2 UEI Ground Track Installations.style", 
            "C2 UEI Ground Track Units.style", 
            "C2 UEI Sea Surface Track.style",
            "C2 UEI Space Track.style",
            "C2 UEI Special Operations Track.style",
            "C2 UEI Subsurface Track.style",
            "Military Emergency Management.style",
            "Military METOC.style",
            "Signals Intelligence.style",
            "Stability Operations.style" 
             */

            Librarian librarian = new Librarian(string.Empty);

            foreach (string styleFile in styleFiles)
            {
                var results = from myRow in symbolTable.AsEnumerable()
                              where myRow.Field<string>("StyleFile") == styleFile
                              select myRow;

                int resultCount = results.Count();

                foreach (DataRow row in results)
                {
                    string name = row["Name"] as string;
                    string symbolId = row["SymbolId"] as string;

                    // System.Diagnostics.Trace.WriteLine("Found Match: " + symbolId + ", " + name + ")");

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
                   
                } // end for each row

            } // end for each StyleFile
        }
    }
}
