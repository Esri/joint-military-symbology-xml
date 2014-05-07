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

namespace JointMilitarySymbologyLibrary
{
    class SimpleEntityExport : EntityExport, IEntityExport
    {
        // This class implements IEntityExport to export a SymbolSet and a given
        // entity within that SymbolSet.  The export format is very simple, breaking
        // down each part of the hierarchical nature of the entity structure and
        // documenting the Label attributes for each element in that "equation".

        string IEntityExport.Headers
        {
            get { return "SymbolSet,Entity,EntityType,EntitySubType,Code,GeometryType"; }
        }

        string IEntityExport.Line(SymbolSet ss, SymbolSetEntity e, SymbolSetEntityEntityType eType, SymbolSetEntityEntityTypeEntitySubType eSubType)
        {
            GeometryType geoType = GeometryType.POINT;

            string result = Convert.ToString(ss.SymbolSetCode.DigitOne) + Convert.ToString(ss.SymbolSetCode.DigitTwo);
            string code = "";
            
            result = result + ",";

            result = result + e.Label.Replace(',', '-');
            code = code + Convert.ToString(e.EntityCode.DigitOne) + Convert.ToString(e.EntityCode.DigitTwo);
            geoType = e.GeometryType;

            result = result + ",";

            if (eType != null)
            {
                result = result + eType.Label.Replace(',', '-');
                code = code + Convert.ToString(eType.EntityTypeCode.DigitOne) + Convert.ToString(eType.EntityTypeCode.DigitTwo);
                geoType = eType.GeometryType;
            }
            else
                code = code + "00";

            result = result + ",";

            if (eSubType != null)
            {
                result = result + eSubType.Label.Replace(',', '-');
                code = code + Convert.ToString(eSubType.EntitySubTypeCode.DigitOne) + Convert.ToString(eSubType.EntitySubTypeCode.DigitTwo);
                geoType = eSubType.GeometryType;
            }
            else
                code = code + "00";

            result = result + "," + code + "," + _geometryList[(int)geoType];

            return result;
        }
    }
}
