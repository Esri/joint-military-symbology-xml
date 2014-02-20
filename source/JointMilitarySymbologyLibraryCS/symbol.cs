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
using NLog;

namespace JointMilitarySymbologyLibrary
{
    public enum SymbolStatusEnum
    {
        statusEnumNew,
        statusEnumOld,
        statusEnumRetired,
        statusEnumInvalid
    };

    public class Symbol
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private SymbolStatusEnum _symbolStat = SymbolStatusEnum.statusEnumInvalid;

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

        private SIDC _sidc = new SIDC(1000980000, 1000000000);
        private string _legacySIDC = "---------------";
        private string _tags = "";

        internal Symbol(Librarian librarian, SIDC sidc)
        {
            _librarian = librarian;
            _sidc = sidc;

            _UpdateFromCurrent();

            switch (_symbolStat)
            {
                case SymbolStatusEnum.statusEnumOld:
                    _BuildLegacySIDC();
                    break;
                case SymbolStatusEnum.statusEnumNew:
                    _legacySIDC = "---------------";
                    break;
            }

            _BuildTags();
        }

        internal Symbol(Librarian librarian, string legacyStandard, string legacySIDC)
        {
            _librarian = librarian;
            _legacySIDC = legacySIDC;

            _UpdateFromLegacy();

            switch (_symbolStat)
            {
                case SymbolStatusEnum.statusEnumOld:
                    _BuildSIDC();
                    break;
                case SymbolStatusEnum.statusEnumRetired:
                    _sidc.PartAUInt = 1000980000;
                    _sidc.PartBUInt = 1100000000;
                    break;
            }

            _BuildTags();
        }

        public SymbolStatusEnum SymbolStatus
        {
            get
            {
                return _symbolStat;
            }
        }

        public SIDC SIDC
        {
            get
            {
                return _sidc;
            }
        }

        public string LegacySIDC
        {
            get
            {
                return _legacySIDC;
            }
        }

        public GeometryType GeometryType
        {
            get
            {
                GeometryType geo = GeometryType.POINT;

                if (_entity != null)
                {
                    geo = _entity.GeometryType;
                }

                return geo;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }
        }

        private Image CreateImage()
        {
            Image imgFrame = null;
            Image imgIcon = null;

            Image Canvas = new Bitmap(1108, 1327);

            if (_affiliation != null)
            {
                if (_affiliation.Graphic != "")
                {
                    if (File.Exists(_librarian.Graphics + "/" + _affiliation.Graphic))
                    {
                        imgFrame = Image.FromFile(_librarian.Graphics + "/" + _affiliation.Graphic);
                    }
                    else
                    {
                        logger.Warn("Can't find graphic: " + _librarian.Graphics + "/" + _affiliation.Graphic);
                    }
                }
            }
            else
            {
                logger.Error("Affiliation not valid.");
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
                        Artist.DrawImage(imgIcon, 0, 0);

                        //Free up resources when required
                        Artist.Dispose();
                    }
                    else
                    {
                        logger.Warn("Can't find graphic: " + _librarian.Graphics + "/" + _entity.Graphic);
                    }
                }
            }
            else
            {
                logger.Error("Entity not valid.");
            }

            return Canvas;
        }

        internal Image Image
        {
            get
            {
                return CreateImage();
            }
        }

        internal void SaveImage(string fileName)
        {
            Image img = CreateImage();

            if (img != null)
                img.Save(fileName, ImageFormat.Png);
            else
                logger.Warn("No image to save");
        }

        private void _BuildTags()
        {
            if (_context != null)
            {
                if(! _tags.Contains(_context.Label))
                    _tags = _tags == "" ? _context.Label : _tags + "; " + _context.Label;
            }

            if(_standardIdentity != null)
            {
                if (!_tags.Contains(_standardIdentity.Label))
                    _tags = _tags == "" ? _standardIdentity.Label : _tags + "; " + _standardIdentity.Label;
            }

            if (_dimension != null)
            {
                if (!_tags.Contains(_dimension.Label))
                    _tags = _tags == "" ? _dimension.Label : _tags + "; " + _dimension.Label;
            }

            if(_symbolSet != null)
            {
                if (!_tags.Contains(_symbolSet.Label))
                    _tags = _tags == "" ? _symbolSet.Label : _tags + "; " + _symbolSet.Label; 
            }

            if(_entity != null)
            {
                if (!_tags.Contains(_entity.Label)) 
                    _tags = _tags == "" ? _entity.Label : _tags + "; " + _entity.Label;
            }

            if(_entityType != null)
            {
                if (!_tags.Contains(_entityType.Label))
                    _tags = _tags == "" ? _entityType.Label : _tags + "; " + _entityType.Label;
            }

            if(_entitySubType != null)
            {
                if (!_tags.Contains(_entitySubType.Label))
                    _tags = _tags == "" ? _entitySubType.Label : _tags + "; " + _entitySubType.Label;
            }

            if(_modifierOne != null)
            {
                if (!_tags.Contains(_modifierOne.Label))
                    _tags = _tags == "" ? _modifierOne.Label : _tags + "; " + _modifierOne.Label;
            }

            if(_modifierTwo != null)
            {
                if (!_tags.Contains(_modifierTwo.Label))
                    _tags = _tags == "" ? _modifierTwo.Label : _tags + "; " + _modifierTwo.Label;
            }

            if(_legacySymbol != null)
            {
                if (!_tags.Contains(_legacySIDC))
                    _tags = _tags == "" ? _legacySIDC : _tags + "; " + _legacySIDC;
            }
        }

        private void _BuildSIDC()
        {
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

            if (_entity != null)
            {
                _sidc.PartBUInt = (uint)_entity.EntityCode.DigitOne * 1000000000 +
                                     (uint)_entity.EntityCode.DigitTwo * 100000000;
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
                }

                _legacySIDC = _legacySIDC + _amplifierGroup.LegacyModifierCode[0].Value +
                                            _amplifier.LegacyModifierCode[0].Value +
                                            "---";
            }
        }

        private void _UpdateFromCurrent()
        {
            string first10 = _sidc.PartAString;
            string second10 = _sidc.PartBString;

            _librarian.ResetStatusCode();

            _version = _librarian.Version(Convert.ToUInt16(first10.Substring(0, 1)),
                                          Convert.ToUInt16(first10.Substring(1, 1)));
            
            _context = _librarian.Context(Convert.ToUInt16(first10.Substring(2, 1)));
            _standardIdentity = _librarian.StandardIdentity(Convert.ToUInt16(first10.Substring(3, 1)));
            _symbolSet = _librarian.SymbolSet(Convert.ToUInt16(first10.Substring(4, 1)), Convert.ToUInt16(first10.Substring(5, 1)));

            if (_symbolSet != null)
            {
                _dimension = _librarian.DimensionBySymbolSet(_symbolSet.ID);
            }

            if (_context != null && _dimension != null && _standardIdentity != null)
            {
                _affiliation = _librarian.Affiliation(_context.ID, _dimension.ID, _standardIdentity.ID);
            }

            _status = _librarian.Status(Convert.ToUInt16(first10.Substring(6, 1)));
            _hqTFDummy = _librarian.HQTFDummy(Convert.ToUInt16(first10.Substring(7, 1)));
            _amplifierGroup = _librarian.AmplifierGroup(Convert.ToUInt16(first10.Substring(8, 1)));

            if (_amplifierGroup != null)
            {
                _amplifier = _librarian.Amplifier(_amplifierGroup, Convert.ToUInt16(first10.Substring(9, 1)));
            }

            if (_context != null && _affiliation != null)
            {
                _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);
            }

            if (_symbolSet != null)
            {
                _entity = _librarian.Entity(_symbolSet, Convert.ToUInt16(second10.Substring(0, 1)), Convert.ToUInt16(second10.Substring(1, 1)));

                if (_entity != null)
                {
                    _entityType = _librarian.EntityType(_entity, Convert.ToUInt16(second10.Substring(2, 1)), Convert.ToUInt16(second10.Substring(3, 1)));
                }

                if (_entityType != null)
                {
                    _entitySubType = _librarian.EntitySubType(_entityType, Convert.ToUInt16(second10.Substring(4, 1)), Convert.ToUInt16(second10.Substring(5, 1)));
                }

                _modifierOne = _librarian.ModifierOne(_symbolSet, Convert.ToUInt16(second10.Substring(6, 1)), Convert.ToUInt16(second10.Substring(7, 1)));
                _modifierTwo = _librarian.ModifierTwo(_symbolSet, Convert.ToUInt16(second10.Substring(8, 1)), Convert.ToUInt16(second10.Substring(9, 1)));
                _legacySymbol = _librarian.LegacySymbol(_symbolSet, _entity, _entityType, _entitySubType, _modifierOne, _modifierTwo);
            }

            _librarian.LogConversionResult(_sidc.PartAString + ", " + _sidc.PartBString);
            
            _ValidateStatus();
        }

        private void _UpdateFromLegacy()
        {
            _librarian.ResetStatusCode();

            _version = _librarian.Version(1, 0);

            _affiliation = _librarian.Affiliation(_legacySIDC.Substring(1, 1), _legacySIDC.Substring(2, 1));

            if (_affiliation != null)
            {
                _context = _librarian.Context(_affiliation.ContextID);
                _standardIdentity = _librarian.StandardIdentity(_affiliation.StandardIdentityID);
            }
            
            _dimension = _librarian.DimensionByLegacyCode(_legacySIDC.Substring(2, 1));

            if (_dimension != null)
            {
                _symbolSet = _librarian.SymbolSet(_dimension.ID, _legacySIDC.Substring(4, 6));
            }

            _status = _librarian.Status(_legacySIDC.Substring(3, 1));
            _hqTFDummy = _librarian.HQTFDummy(_legacySIDC.Substring(10, 1));

            if (_context != null && _affiliation != null)
            {
                _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);
            }

            _amplifier = _librarian.Amplifier(_legacySIDC.Substring(11, 1));

            if (_amplifier != null)
            {
                _amplifierGroup = _librarian.AmplifierGroup(_amplifier);
            }

            if (_symbolSet != null)
            {
                _legacySymbol = _librarian.LegacySymbol(_symbolSet, _legacySIDC.Substring(4, 6));
            }

            if (_legacySymbol != null)
            {
                _entity = _librarian.Entity(_symbolSet, _legacySymbol.EntityID);
                _entityType = _librarian.EntityType(_entity, _legacySymbol.EntityTypeID);
                _entitySubType = _librarian.EntitySubType(_entityType, _legacySymbol.EntitySubTypeID);
                _modifierOne = _librarian.ModifierOne(_symbolSet, _legacySymbol.ModifierOneID);
                _modifierTwo = _librarian.ModifierTwo(_symbolSet, _legacySymbol.ModifierTwoID);
            }

            _librarian.LogConversionResult(_legacySIDC);
            
            _ValidateStatus();
        }

        private void _SetInvalidSymbolProps()
        {
            _version = _librarian.Version(1, 0);
            _context = _librarian.Context(0);
            _dimension = _librarian.Dimension("I");
            _standardIdentity = _librarian.StandardIdentity(0);
            _symbolSet = _librarian.SymbolSet(8, 8);
            _status = _librarian.Status(0);
            _hqTFDummy = _librarian.HQTFDummy(0);
            _amplifierGroup = _librarian.AmplifierGroup(0);
            _amplifier = _librarian.Amplifier(_amplifierGroup, 0);
            _affiliation = _librarian.Affiliation("REALITY", "INTERNAL", "SI_PENDING");
            _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);

            _entityType = null;
            _entitySubType = null;
            _modifierOne = null;
            _modifierTwo = null;
        }

        private void _ValidateStatus()
        {
            string s = Convert.ToString(0 - _librarian.StatusCode, 2);
            char[] bits = s.PadLeft(17, '0').ToCharArray();

            Array.Reverse(bits);

            if (bits[(int)StatusCodeEnum.statusCodeNoLegacySymbol] == '0' && bits[(int)StatusCodeEnum.statusCodeNoEntity] == '1')
            {
                // Retired symbol.  Everything was found, including a LegacySymbol
                // but there was no Entity found for that symbol.
                // Confirm retirement with Remarks check.
                if (_legacySymbol.Remarks == "Retired")
                {
                    //_SetInvalidSymbolProps();
                    _entity = _librarian.Entity(_symbolSet, 1, 1);
                    _symbolStat = SymbolStatusEnum.statusEnumRetired;
                }
                else
                {
                    // Remarks double check is missing
                    logger.Warn("Symbol retirement in question - check XML instance data");
                    _symbolStat = SymbolStatusEnum.statusEnumInvalid;
                }
            }
            else
            {
                if (bits[(int)StatusCodeEnum.statusCodeNoVersion] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoContext] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoDimension] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoStandardIdentity] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoSymbolSet] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoStatus] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoHQTFDummy] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoAmplifierGroup] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoAmplifier] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoAffiliation] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoContextAmplifier] == '1' ||
                    bits[(int)StatusCodeEnum.statusCodeNoEntity] == '1')
                {
                    _symbolStat = SymbolStatusEnum.statusEnumInvalid;
                }
                else
                {
                    if(_legacySymbol != null)
                    {
                        _symbolStat = SymbolStatusEnum.statusEnumOld;
                    }
                    else
                    {
                        _symbolStat = SymbolStatusEnum.statusEnumNew;
                    }
                }
            }
        }
    }
}
