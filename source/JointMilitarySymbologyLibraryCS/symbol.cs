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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class Symbol
    {
        private bool _isValid = false;
        private Librarian _librarian = null;
        private LibraryVersion _version = null;
        private LibraryContext _context = null;
        private LibraryContextContextAmplifier _contextAmplifier = null;
        private LibraryStandardIdentity _standardIdentity = null;
        private LibraryDimension _dimension = null;
        private LibraryAffiliation _affiliation = null;
        private LibraryStatus _status = null;
        private LibraryHQTFDummy _hqTFDummy = null;
        private LibraryAmplifierGroup _amplifierGroup = null;
        private LibraryAmplifierGroupAmplifier _amplifier = null;

        private SymbolSet _symbolSet = null;
        private SymbolSetEntity _entity = null;
        private SymbolSetEntityEntityType _entityType = null;
        private SymbolSetEntityEntityTypeEntitySubType _entitySubType = null;
        private ModifiersTypeModifier _modifierOne = null;
        private ModifiersTypeModifier _modifierTwo = null;

        private SymbolSetLegacySymbol _legacySymbol = null;

        private SIDC _sidc = new SIDC();
        private string _legacySIDC = "SPGP-----------";

        internal Symbol(Librarian librarian, SIDC sidc)
        {
            _librarian = librarian;
            _sidc = sidc;

            _UpdateFromCurrent();
            _BuildLegacySIDC();
        }

        internal Symbol(Librarian librarian, string legacyStandard, string legacySIDC)
        {
            _librarian = librarian;
            _legacySIDC = legacySIDC;

            _UpdateFromLegacy();
            _BuildSIDC();
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        public SIDC SIDC
        {
            get
            {
                return _sidc;
            }

            set
            {
                _sidc = value;
                _UpdateFromCurrent();
                _BuildLegacySIDC();        
            }
        }

        public string LegacySIDC
        {
            get
            {
                return _legacySIDC;
            }

            set 
            { 
                _legacySIDC = value;
                _UpdateFromLegacy();
                _BuildSIDC();
            }
        }

        public Image Image
        {
            get
            {
                Image imgFrame = null;
                Image imgIcon = null;

                Image Canvas = new Bitmap(1108, 1327);

                if (_affiliation.Graphic != "")
                {
                    if (File.Exists(_librarian.Graphics + "/" + _affiliation.Graphic))
                    {
                        imgFrame = Image.FromFile(_librarian.Graphics + "/" + _affiliation.Graphic);
                    }
                }

                if (_entity != null)
                {
                    if (_entity.Graphic != "")
                    {
                        if (File.Exists(_librarian.Graphics + "/" + _entity.Graphic))
                        {
                            imgIcon = Image.FromFile(_librarian.Graphics + "/" + _entity.Graphic);

                            //Frame to define the dimentions
                            Rectangle Frame = new Rectangle(0, 0, 267, 320);
                            Rectangle Frame2 = new Rectangle(42, 153, 1024, 1020);

                            //To do drawing and stuffs
                            Graphics Artist = Graphics.FromImage(Canvas);

                            //Draw the layers on Canvas
                            Artist.DrawImage(imgFrame, 0, 0);
                            //Artist.DrawImage(imgFrame, Frame, Frame, 0, 0);
                            //Artist.DrawImage(imgIcon, 0, 0);
                            //Artist.DrawImage(imgIcon, 84, 307, Frame, GraphicsUnit.Pixel);
                            Artist.DrawImage(imgIcon, 0, 0);

                            //Free up resources when required
                            Artist.Dispose();
                        }
                    }
                }

                return Canvas;
            }
        }

        public void SaveImage(string fileName)
        {
            Image img = this.Image;

            if(img != null)
                img.Save(fileName, ImageFormat.Png);
        }

        private void _BuildSIDC()
        {
            _isValid = true;

            if (_version != null && _context != null &&
               _standardIdentity != null && _symbolSet != null && _status != null &&
               _hqTFDummy != null && _amplifierGroup != null && _amplifier != null)
            {
                _sidc.PartAUInt = (uint)_version.VersionCode.DigitOne * 1000000000 +
                                    (uint)_version.VersionCode.DigitTwo * 100000000 +
                                    (uint)_context.ContextCode * 10000000 +
                                    (uint)_standardIdentity.StandardIdentityCode * 1000000 +
                                    (uint)_symbolSet.SymbolSetCode.DigitOne * 100000 +
                                    (uint)_symbolSet.SymbolSetCode.DigitTwo * 10000 +
                                    (uint)_status.StatusCode * 1000 +
                                    (uint)_hqTFDummy.HQTFDummyCode * 100 +
                                    (uint)_amplifierGroup.AmplifierGroupCode * 10 +
                                    (uint)_amplifier.AmplifierCode;
            }
            else
            {
                _isValid = false;
            }

            if (_entity != null)
            {
                _sidc.PartBUInt = (uint)_entity.EntityCode.DigitOne * 1000000000 +
                                     (uint)_entity.EntityCode.DigitTwo * 100000000;
            }
            else
            {
                _isValid = false;
            }

            if (_entityType != null)
            {
                _sidc.PartBUInt = _sidc.PartBUInt + (uint)_entityType.EntityTypeCode.DigitOne * 10000000 +
                                                          (uint)_entityType.EntityTypeCode.DigitTwo * 1000000;

            }

            if (_entitySubType != null)
            {
                _sidc.PartBUInt = _sidc.PartBUInt + (uint)_entitySubType.EntitySubTypeCode.DigitOne * 100000 +
                                                          (uint)_entitySubType.EntitySubTypeCode.DigitTwo * 10000;

            }

            if (_modifierOne != null)
            {
                _sidc.PartBUInt = _sidc.PartBUInt + (uint)_modifierOne.ModifierCode.DigitOne * 1000 +
                                                          (uint)_modifierOne.ModifierCode.DigitTwo * 100;

            }

            if (_modifierTwo != null)
            {
                _sidc.PartBUInt = _sidc.PartBUInt + (uint)_modifierTwo.ModifierCode.DigitOne * 10 +
                                                          (uint)_modifierTwo.ModifierCode.DigitTwo;

            }
        }

        private void _BuildLegacySIDC()
        {
            _isValid = true;

            if (_symbolSet != null && _affiliation != null && _dimension != null &&
               _status != null && _amplifierGroup != null && _amplifier != null)
            {
                _legacySIDC = _symbolSet.LegacyCodingSchemeCode[0].Value +
                              _affiliation.LegacyStandardIdentityCode[0].Value +
                              _dimension.LegacyDimensionCode[0].Value +
                              _status.LegacyStatusCode[0].Value;

                if (_legacySymbol != null)
                {
                    _legacySIDC = _legacySIDC + _legacySymbol.LegacyFunctionCode[0].Value;
                }
                else
                {
                    _legacySIDC = _legacySIDC + "------";
                    _isValid = false;
                }

                _legacySIDC = _legacySIDC + _amplifierGroup.LegacyModifierCode[0].Value +
                                            _amplifier.LegacyModifierCode[0].Value +
                                            "---";
            }
            else
                _isValid = false;
        }

        private void _UpdateFromCurrent()
        {
            string first10 = _sidc.PartAString;
            string second10 = _sidc.PartBString;

            _version = _librarian.Version(Convert.ToUInt16(first10.Substring(0, 1)),
                                          Convert.ToUInt16(first10.Substring(1, 1)));
            
            _context = _librarian.Context(Convert.ToUInt16(first10.Substring(2, 1)));
            _standardIdentity = _librarian.StandardIdentity(Convert.ToUInt16(first10.Substring(3, 1)));
            _symbolSet = _librarian.SymbolSet(Convert.ToUInt16(first10.Substring(4, 1)), Convert.ToUInt16(first10.Substring(5, 1)));
            _dimension = _librarian.DimensionBySymbolSet(_symbolSet.ID);
            _affiliation = _librarian.Affiliation(_context.ID, _dimension.ID, _standardIdentity.ID);
            _status = _librarian.Status(Convert.ToUInt16(first10.Substring(6, 1)));
            _hqTFDummy = _librarian.HQTFDummy(Convert.ToUInt16(first10.Substring(7, 1)));
            _amplifierGroup = _librarian.AmplifierGroup(Convert.ToUInt16(first10.Substring(8, 1)));
            _amplifier = _librarian.Amplifier(_amplifierGroup, Convert.ToUInt16(first10.Substring(9, 1)));
            _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);

            _entity = _librarian.Entity(_symbolSet, Convert.ToUInt16(second10.Substring(0, 1)), Convert.ToUInt16(second10.Substring(1, 1)));
            _entityType = _librarian.EntityType(_entity, Convert.ToUInt16(second10.Substring(2, 1)), Convert.ToUInt16(second10.Substring(3, 1)));
            _entitySubType = _librarian.EntitySubType(_entityType, Convert.ToUInt16(second10.Substring(4, 1)), Convert.ToUInt16(second10.Substring(5, 1)));
            _modifierOne = _librarian.ModifierOne(_symbolSet, Convert.ToUInt16(second10.Substring(6, 1)), Convert.ToUInt16(second10.Substring(7, 1)));
            _modifierTwo = _librarian.ModifierTwo(_symbolSet, Convert.ToUInt16(second10.Substring(8, 1)), Convert.ToUInt16(second10.Substring(9, 1)));
            _legacySymbol = _librarian.LegacySymbol(_symbolSet, _entity, _entityType, _entitySubType, _modifierOne, _modifierTwo);
        }

        private void _UpdateFromLegacy()
        {
            _version = _librarian.Version(1, 0);

            _affiliation = _librarian.Affiliation(_legacySIDC.Substring(1, 1), _legacySIDC.Substring(2, 1));
            _context = _librarian.Context(_affiliation.ContextID);
            _standardIdentity = _librarian.StandardIdentity(_affiliation.StandardIdentityID);
            _dimension = _librarian.DimensionByLegacyCode(_legacySIDC.Substring(2, 1));
            _symbolSet = _librarian.SymbolSet(_dimension.ID, _legacySIDC.Substring(4, 6));
            _status = _librarian.Status(_legacySIDC.Substring(3, 1));
            _hqTFDummy = _librarian.HQTFDummy(_legacySIDC.Substring(10, 1));
            _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);
            _amplifier = _librarian.Amplifier(_legacySIDC.Substring(11, 1));
            _amplifierGroup = _librarian.AmplifierGroup(_amplifier);

            _legacySymbol = _librarian.LegacySymbol(_symbolSet, _legacySIDC.Substring(4,6));

            if (_legacySymbol != null)
            {
                _entity = _librarian.Entity(_symbolSet, _legacySymbol.EntityID);
                _entityType = _librarian.EntityType(_entity, _legacySymbol.EntityTypeID);
                _entitySubType = _librarian.EntitySubType(_entityType, _legacySymbol.EntitySubTypeID);
                _modifierOne = _librarian.ModifierOne(_symbolSet, _legacySymbol.ModifierOneID);
                _modifierTwo = _librarian.ModifierTwo(_symbolSet, _legacySymbol.ModifierTwoID);
            }
        }
    }
}
