using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JointMilitarySymbologyLibrary
{
    public class symbol
    {
        private bool _isValid = false;
        private librarian _librarian = null;
        private libraryVersion _version = null;
        private libraryContext _context = null;
        private libraryContextContextAmplifier _contextAmplifier = null;
        private libraryStandardIdentity _standardIdentity = null;
        private libraryDimension _dimension = null;
        private libraryAffiliation _affiliation = null;
        private libraryStatus _status = null;
        private libraryHqTFDummy _hqTFDummy = null;
        private libraryAmplifierGroup _amplifierGroup = null;
        private libraryAmplifierGroupAmplifier _amplifier = null;

        private symbolSet _symbolSet = null;
        private symbolSetEntity _entity = null;
        private symbolSetEntityEntityType _entityType = null;
        private symbolSetEntityEntityTypeEntitySubType _entitySubType = null;
        private modifiersTypeModifier _modifierOne = null;
        private modifiersTypeModifier _modifierTwo = null;

        private symbolSetLegacySymbol _legacySymbol = null;

        private sidc _sidc = new sidc();
        private string _legacySIDC = "SPGP-----------";

        internal symbol(librarian librarian, sidc sidc)
        {
            _librarian = librarian;
            _sidc = sidc;

            _updateFromCurrent();
            _buildLegacySIDC();
        }

        internal symbol(librarian librarian, string legacyStandard, string legacySIDC)
        {
            _librarian = librarian;
            _legacySIDC = legacySIDC;

            _updateFromLegacy();
            _buildSIDC();
        }

        public bool isValid
        {
            get
            {
                return _isValid;
            }
        }

        public sidc sidc
        {
            get
            {
                return _sidc;
            }

            set
            {
                _sidc = value;
                _updateFromCurrent();
                _buildLegacySIDC();        
            }
        }

        public string legacySIDC
        {
            get
            {
                return _legacySIDC;
            }

            set 
            { 
                _legacySIDC = value;
                _updateFromLegacy();
                _buildSIDC();
            }
        }

        public Image image
        {
            get
            {
                Image imgFrame = null;
                Image imgIcon = null;

                Image Canvas = new Bitmap(1108, 1327);

                if (_affiliation.graphic != "")
                {
                    if (File.Exists(_librarian.graphics + "/" + _affiliation.graphic))
                    {
                        imgFrame = Image.FromFile(_librarian.graphics + "/" + _affiliation.graphic);
                    }
                }

                if (_entity != null)
                {
                    if (_entity.graphic != "")
                    {
                        if (File.Exists(_librarian.graphics + "/" + _entity.graphic))
                        {
                            imgIcon = Image.FromFile(_librarian.graphics + "/" + _entity.graphic);

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

        public void saveImage(string fileName)
        {
            Image img = this.image;

            if(img != null)
                img.Save(fileName, ImageFormat.Png);
        }

        private void _buildSIDC()
        {
            _isValid = true;

            if (_version != null && _context != null &&
               _standardIdentity != null && _symbolSet != null && _status != null &&
               _hqTFDummy != null && _amplifierGroup != null && _amplifier != null)
            {
                _sidc.partAUInt = (uint)_version.versionCode.digitOne * 1000000000 +
                                    (uint)_version.versionCode.digitTwo * 100000000 +
                                    (uint)_context.contextCode * 10000000 +
                                    (uint)_standardIdentity.standardIdentityCode * 1000000 +
                                    (uint)_symbolSet.symbolSetCode.digitOne * 100000 +
                                    (uint)_symbolSet.symbolSetCode.digitTwo * 10000 +
                                    (uint)_status.statusCode * 1000 +
                                    (uint)_hqTFDummy.hqTFDummyCode * 100 +
                                    (uint)_amplifierGroup.amplifierGroupCode * 10 +
                                    (uint)_amplifier.amplifierCode;
            }
            else
            {
                _isValid = false;
            }

            if (_entity != null)
            {
                _sidc.partBUInt = (uint)_entity.entityCode.digitOne * 1000000000 +
                                     (uint)_entity.entityCode.digitTwo * 100000000;
            }
            else
            {
                _isValid = false;
            }

            if (_entityType != null)
            {
                _sidc.partBUInt = _sidc.partBUInt + (uint)_entityType.entityTypeCode.digitOne * 10000000 +
                                                          (uint)_entityType.entityTypeCode.digitTwo * 1000000;

            }

            if (_entitySubType != null)
            {
                _sidc.partBUInt = _sidc.partBUInt + (uint)_entitySubType.entitySubTypeCode.digitOne * 100000 +
                                                          (uint)_entitySubType.entitySubTypeCode.digitTwo * 10000;

            }

            if (_modifierOne != null)
            {
                _sidc.partBUInt = _sidc.partBUInt + (uint)_modifierOne.modifierCode.digitOne * 1000 +
                                                          (uint)_modifierOne.modifierCode.digitTwo * 100;

            }

            if (_modifierTwo != null)
            {
                _sidc.partBUInt = _sidc.partBUInt + (uint)_modifierTwo.modifierCode.digitOne * 10 +
                                                          (uint)_modifierTwo.modifierCode.digitTwo;

            }
        }

        private void _buildLegacySIDC()
        {
            _isValid = true;

            if (_symbolSet != null && _affiliation != null && _dimension != null &&
               _status != null && _amplifierGroup != null && _amplifier != null)
            {
                _legacySIDC = _symbolSet.legacyCodingSchemeCode[0].Value +
                              _affiliation.legacyStandardIdentityCode[0].Value +
                              _dimension.legacyDimensionCode[0].Value +
                              _status.legacyStatusCode[0].Value;

                if (_legacySymbol != null)
                {
                    _legacySIDC = _legacySIDC + _legacySymbol.legacyFunctionCode[0].Value;
                }
                else
                {
                    _legacySIDC = _legacySIDC + "------";
                    _isValid = false;
                }

                _legacySIDC = _legacySIDC + _amplifierGroup.legacyModifierCode[0].Value +
                                            _amplifier.legacyModifierCode[0].Value +
                                            "---";
            }
            else
                _isValid = false;
        }

        private void _updateFromCurrent()
        {
            string first10 = _sidc.partAString;
            string second10 = _sidc.partBString;

            _version = _librarian.version(Convert.ToUInt16(first10.Substring(0, 1)),
                                          Convert.ToUInt16(first10.Substring(1, 1)));
            
            _context = _librarian.context(Convert.ToUInt16(first10.Substring(2, 1)));
            _standardIdentity = _librarian.standardIdentity(Convert.ToUInt16(first10.Substring(3, 1)));
            _symbolSet = _librarian.symbolSet(Convert.ToUInt16(first10.Substring(4, 1)), Convert.ToUInt16(first10.Substring(5, 1)));
            _dimension = _librarian.dimensionBySymbolSet(_symbolSet.id);
            _affiliation = _librarian.affiliation(_context.id, _dimension.id, _standardIdentity.id);
            _status = _librarian.status(Convert.ToUInt16(first10.Substring(6, 1)));
            _hqTFDummy = _librarian.hqTFDummy(Convert.ToUInt16(first10.Substring(7, 1)));
            _amplifierGroup = _librarian.amplifierGroup(Convert.ToUInt16(first10.Substring(8, 1)));
            _amplifier = _librarian.amplifier(_amplifierGroup, Convert.ToUInt16(first10.Substring(9, 1)));
            _contextAmplifier = _librarian.contextAmplifier(_context, _affiliation.shape);

            _entity = _librarian.entity(_symbolSet, Convert.ToUInt16(second10.Substring(0, 1)), Convert.ToUInt16(second10.Substring(1, 1)));
            _entityType = _librarian.entityType(_entity, Convert.ToUInt16(second10.Substring(2, 1)), Convert.ToUInt16(second10.Substring(3, 1)));
            _entitySubType = _librarian.entitySubType(_entityType, Convert.ToUInt16(second10.Substring(4, 1)), Convert.ToUInt16(second10.Substring(5, 1)));
            _modifierOne = _librarian.modifierOne(_symbolSet, Convert.ToUInt16(second10.Substring(6, 1)), Convert.ToUInt16(second10.Substring(7, 1)));
            _modifierTwo = _librarian.modifierTwo(_symbolSet, Convert.ToUInt16(second10.Substring(8, 1)), Convert.ToUInt16(second10.Substring(9, 1)));
            _legacySymbol = _librarian.legacySymbol(_symbolSet, _entity, _entityType, _entitySubType, _modifierOne, _modifierTwo);
        }

        private void _updateFromLegacy()
        {
            _version = _librarian.version(1, 0);

            _affiliation = _librarian.affiliation(_legacySIDC.Substring(1, 1), _legacySIDC.Substring(2, 1));
            _context = _librarian.context(_affiliation.contextID);
            _standardIdentity = _librarian.standardIdentity(_affiliation.standardIdentityID);
            _dimension = _librarian.dimensionByLegacyCode(_legacySIDC.Substring(2, 1));
            _symbolSet = _librarian.symbolSet(_dimension.id, _legacySIDC.Substring(4, 6));
            _status = _librarian.status(_legacySIDC.Substring(3, 1));
            _hqTFDummy = _librarian.hqTFDummy(_legacySIDC.Substring(10, 1));
            _contextAmplifier = _librarian.contextAmplifier(_context, _affiliation.shape);
            _amplifier = _librarian.amplifier(_legacySIDC.Substring(11, 1));
            _amplifierGroup = _librarian.amplifierGroup(_amplifier);

            _legacySymbol = _librarian.legacySymbol(_symbolSet, _legacySIDC.Substring(4,6));

            if (_legacySymbol != null)
            {
                _entity = _librarian.entity(_symbolSet, _legacySymbol.entityID);
                _entityType = _librarian.entityType(_entity, _legacySymbol.entityTypeID);
                _entitySubType = _librarian.entitySubType(_entityType, _legacySymbol.entitySubTypeID);
                _modifierOne = _librarian.modifierOne(_symbolSet, _legacySymbol.modifierOneID);
                _modifierTwo = _librarian.modifierTwo(_symbolSet, _legacySymbol.modifierTwoID);
            }
        }
    }
}
