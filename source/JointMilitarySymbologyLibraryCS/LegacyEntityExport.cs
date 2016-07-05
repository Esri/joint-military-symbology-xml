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
using System.Threading.Tasks;
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class LegacyEntityExport : EntityExport
    {
        private long _size;
        private string _standard;
        private ImageEntityExport _entityExport;

        public LegacyEntityExport(ConfigHelper configHelper, string standard, long size)
        {
            _configHelper = configHelper;
            _standard = standard;
            _size = size;

            _entityExport = new ImageEntityExport(_configHelper, true, true);
        }

        public string Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,styleItemUniqueId,styleItemGeometryType,notes"; }
        }

        public string Line(LibraryStandardIdentityGroup sig, SymbolSet ss, SymbolSetLegacySymbol symbol, LegacyEntityType entity, LegacyFunctionCodeType code)
        {
            string result = "";
            string graphic = "";
            string suffix = "";

            _notes = "";

            string graphicPath = "";
            switch (code.LimitUseTo)
            {
                case "2525Bc2":
                    graphicPath = _configHelper.GetPath("JMSML_2525BC2", FindEnum.Find2525BC2);
                    suffix = "(2525B)";
                    break;
                case "2525C":
                    graphicPath = _configHelper.GetPath("JMSML_2525C", FindEnum.Find2525C);
                    suffix = "(2525C)";
                    break;
                default:
                    graphicPath = _configHelper.GetPath("JMSML_2525C", FindEnum.Find2525C);
                    break;
            }

            if (entity.Graphic != "" && entity.Icon != IconType.FULL_FRAME)
                graphic = entity.Graphic;
            else
                graphic = GrabGraphic(entity.CloverGraphic, entity.RectangleGraphic, entity.SquareGraphic, entity.DiamondGraphic, sig.GraphicSuffix);

            string id = graphic.Substring(0, graphic.Length - 4); ;

            IconType iType = entity.Icon;
            string geometryType = GeometryIs(entity.GeometryType);

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";

            result = result + itemRootedPath;
            result = result + "," + Convert.ToString(_configHelper.PointSize);
            result = result + "," + BuildEntityItemName(sig, ss, symbol, entity, code); 
            result = result + "," + BuildEntityItemCategory(ss, iType, geometryType);
            result = result + "," + BuildEntityItemTags(sig, ss, symbol, entity, code);
            result = result + "," + id;
            result = result + "," + geometryType;
            result = result + "," + _notes;

            return result;
        }
    }
}
