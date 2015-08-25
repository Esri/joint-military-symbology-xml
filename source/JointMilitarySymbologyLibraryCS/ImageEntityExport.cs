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

namespace JointMilitarySymbologyLibrary
{
    public class ImageEntityExport : EntityExport, IEntityExport
    {
        // Class designed to implement the export of a SymbolSet and one entity from that SymbolSet
        // by outputting image file path information, its name and the category of icon it falls within,
        // and the tags associated with that SymbolSet and entity.

        private bool _omitSource = false;
        private bool _omitLegacy = false;

        public ImageEntityExport(ConfigHelper configHelper, bool omitSource, bool omitLegacy)
        {
            _configHelper = configHelper;
            _omitSource = omitSource;
            _omitLegacy = omitLegacy;
        }

        string IEntityExport.Headers
        {
            get { return "filePath,pointSize,styleItemName,styleItemCategory,styleItemTags,styleItemUniqueId,styleItemGeometryType,notes"; }
        }

        string IEntityExport.Line(LibraryStandardIdentityGroup sig, SymbolSet ss, SymbolSetEntity e, SymbolSetEntityEntityType eType, EntitySubTypeType eSubType)
        {
            _notes = "";

            string result = "";
            string graphic = "";
            IconType iType = IconType.MAIN;

            string graphicPath = _configHelper.GetPath(ss.ID, FindEnum.FindEntities);
                         
            if (eSubType != null)
            {
                if (eSubType.Graphic != "" && eSubType.Icon != IconType.FULL_FRAME)
                    graphic = eSubType.Graphic;
                else
                    if (sig != null)
                    {
                        graphic = GrabGraphic(eSubType.CloverGraphic, eSubType.RectangleGraphic, eSubType.SquareGraphic, eSubType.DiamondGraphic, sig.GraphicSuffix);
                    }

                iType = eSubType.Icon;
            }
            else if (eType != null)
            {
                if (eType.Graphic != "" && eType.Icon != IconType.FULL_FRAME)
                    graphic = eType.Graphic;
                else
                    if (sig != null)
                    {
                        graphic = GrabGraphic(eType.CloverGraphic, eType.RectangleGraphic, eType.SquareGraphic, eType.DiamondGraphic, sig.GraphicSuffix);
                    }

                iType = eType.Icon;
            }
            else if (e != null)
            {
                if (e.Graphic != "" && e.Icon != IconType.FULL_FRAME)
                    graphic = e.Graphic;
                else
                    if (sig != null)
                    {
                        graphic = GrabGraphic(e.CloverGraphic, e.RectangleGraphic, e.SquareGraphic, e.DiamondGraphic, sig.GraphicSuffix);
                    }

                iType = e.Icon;
            }

            // Suppressed as considered redundant information

            //if (iType == IconType.NA)
            //    _notes = _notes + "icon is NA - entity is never to be drawn;";
            //else if(iType != IconType.MAIN)
            //    _notes = _notes + "icon is " + Convert.ToString(iType) + ";";

            string itemRootedPath = _configHelper.BuildRootedPath(graphicPath, graphic);
            string itemOriginalPath = _configHelper.BuildOriginalPath(graphicPath, graphic);

            if (!File.Exists(itemOriginalPath))
                _notes = _notes + "image file does not exist;";
            
            string itemName = BuildEntityItemName(sig, ss, e, eType, eSubType);
            string itemTags = BuildEntityItemTags(sig, ss, e, eType, eSubType, _omitSource, _omitLegacy);
            string itemID = BuildEntityCode(sig, ss, e, eType, eSubType);
            string itemGeometry = GeometryIs(e, eType, eSubType);
            string itemCategory = BuildEntityItemCategory(ss, iType, itemGeometry);
            
            result = itemRootedPath + "," +
                     Convert.ToString(_configHelper.PointSize) + "," +
                     itemName + "," +
                     itemCategory + "," +
                     itemTags + "," + 
                     itemID + "," +
                     itemGeometry + "," +
                     _notes;

            return result;
        }

        string IEntityExport.Line(LibraryStandardIdentityGroup sig, SymbolSet ss, EntitySubTypeType eSubType)
        {
            IEntityExport iEx = this;
            return iEx.Line(sig, ss, null, null, eSubType);
        }

        string IEntityExport.Line(EntitySubTypeType eSubType)
        {
            return "";
        }
    }
}
