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
using System.Threading.Tasks;
using NLog;
using Svg;

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
        // Instances of the Symbol class can only be created by a Librarian object.
        // A Symbol object represents one possible 2525 symbol, in multiple standards
        // if possible.

        // When created with a given SIDC, the Symbol object uses search facilities in
        // its parent Library object to construct that symbol from JMSML elements.  In
        // so doing, it also tries to construct the symbol for legacy standards (2525C,
        // 2525B, etc.

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private static string _blankLegacySIDC = "---------------";
        private static string _blankLegacyFunction = "------";
        private static string _blankLegacyTail = "---";

        private SymbolStatusEnum _symbolStat = SymbolStatusEnum.statusEnumInvalid;

        private Librarian _librarian = null;
        private LibraryVersion _version = null;
        private LibraryContext _context = null;
        private LibraryContextContextAmplifier _contextAmplifier = null;
        private LibraryStandardIdentity _standardIdentity = null;
        private LibraryStandardIdentityGroup _sig = null;
        private LibraryDimension _dimension = null;
        private LibraryAffiliation _affiliation = null;
        private LibraryStatus _status = null;
        private LibraryHQTFDummy _hqTFDummy = null;
        private LibraryAmplifierGroup _amplifierGroup = null;
        private LibraryAmplifierGroupAmplifier _amplifier = null;

        private SymbolSet _symbolSet = null;
        private SymbolSetEntity _entity = null;
        private SymbolSetEntityEntityType _entityType = null;
        private EntitySubTypeType _entitySubType = null;
        private ModifiersTypeModifier _modifierOne = null;
        private ModifiersTypeModifier _modifierTwo = null;

        private SymbolSetLegacySymbol _legacySymbol = null;

        private SIDC _sidc = new SIDC();
        private string _legacySIDC;

        private Dictionary<string,string> _names = new Dictionary<string,string>();

        private string _tags = "";
        private List<Dictionary<string, string>> _labels = new List<Dictionary<string, string>>();
        private Dictionary<string, string> _drawRule = new Dictionary<string, string>();
        private string _drawNote = "";

        private ConfigHelper _configHelper;
        private List<string> _graphics = new List<string>();

        private bool _colorBarOCA = true;

        internal Symbol(Librarian librarian, SIDC sidc, bool drawColorBars = true)
        {
            _librarian = librarian;
            _sidc = sidc;
            _colorBarOCA = drawColorBars;

            _legacySIDC = _blankLegacySIDC;
            _configHelper = new ConfigHelper(_librarian);

            _UpdateFromCurrent();

            switch (_symbolStat)
            {
                case SymbolStatusEnum.statusEnumOld:
                    _BuildLegacySIDC();
                    break;
                case SymbolStatusEnum.statusEnumNew:
                    _legacySIDC = _blankLegacySIDC;
                    break;
            }

            _BuildNames();
            _BuildTags();
            _BuildLabels();
            _BuildDrawRule();
            _BuildGraphics();
        }

        internal Symbol(Librarian librarian, string legacyStandard, string legacySIDC, bool drawColorBars = true)
        {
            _librarian = librarian;
            _legacySIDC = legacySIDC;
            _configHelper = new ConfigHelper(_librarian);

            _UpdateFromLegacy();

            switch (_symbolStat)
            {
                case SymbolStatusEnum.statusEnumOld:
                    _BuildSIDC();
                    break;
                case SymbolStatusEnum.statusEnumRetired:
                    _sidc.PartAUInt = SIDC.RETIRED.PartAUInt;
                    _sidc.PartBUInt = SIDC.RETIRED.PartBUInt;
                    break;
            }

            _BuildNames();
            _BuildTags();
            _BuildLabels();
            _BuildDrawRule();
            _BuildGraphics();
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

        public List<string> LegacySIDCs
        {
            get
            {
                List<string> newList = new List<string>();

                if (_legacySymbol != null)
                {
                    if (_legacySymbol.LegacyFunctionCode != null)
                    {
                        foreach (LegacyFunctionCodeType functionCode in _legacySymbol.LegacyFunctionCode)
                        {
                            string sidc = _BuildLegacySIDC(functionCode);

                            if (sidc != "")
                                newList.Add(sidc);
                        }
                    }
                }

                return newList;
            }
        }

        public bool ColorBarOCA
        {
            get
            {
                return _colorBarOCA;
            }
        }

        public IconType IconType
        {
            get
            {
                IconType icon = JointMilitarySymbologyLibrary.IconType.NA;

                if (_entitySubType != null)
                    icon = _entitySubType.Icon;
                else if (_entityType != null)
                    icon = _entityType.Icon;
                else if (_entity != null)
                    icon = _entity.Icon;

                return icon;
            }
        }

        public GeometryType GeometryType
        {
            get
            {
                GeometryType geo = GeometryType.POINT;

                if (_entitySubType != null)
                    geo = _entitySubType.GeometryType;
                else if (_entityType != null)
                    geo = _entityType.GeometryType;
                else if (_entity != null)
                    geo = _entity.GeometryType;

                return geo;
            }
        }

        public Dictionary<string,string> Names
        {
            get
            {
                return _names;
            }
        }

        public string Tags
        {
            get
            {
                return _tags;
            }
        }

        public List<Dictionary<string, string>> Labels
        {
            get
            {
                return _labels;
            }
        }

        public Dictionary<string, string> DrawRule
        {
            get
            {
                return _drawRule;
            }
        }

        public string DrawNote
        {
            get
            {
                return _drawNote;
            }
        }

        public List<string> Graphics
        {
            get
            {
                return _graphics;
            }
        }

        public Bitmap Bitmap(int width, int height)
        {
            if (_graphics.Count == 0)
                return null;

            Bitmap bm = new Bitmap(width, height);

            foreach (string graphic in _graphics)
            {
                if(File.Exists(graphic))
                {
                    SvgDocument doc = SvgDocument.Open(graphic);
                    if (doc.Height > height)
                    {
                        doc.Width = (int)(((double)doc.Width / (double)doc.Height) * (double)height);
                        doc.Height = height;
                    }
                    doc.Draw(bm);
                }
                else
                    logger.Warn(graphic + " missing.");
            }

            return bm;
        }

        private Dictionary<string, string> _CreateLabelDictionary(FieldListTypeField field)
        {
            Dictionary<string, string> label = new Dictionary<string, string>();

            label.Add("Name", field.Name);
            label.Add("Label", field.Label);
            label.Add("Description", field.Description);
            label.Add("Remarks", field.Remarks);
            label.Add("X", Convert.ToString(field.X));
            label.Add("Y", Convert.ToString(field.Y));

            LibraryAmplifier amp = _librarian.SymbolAmplifier(field.AmplifierID);
            if (amp != null)
            {
                label.Add("Type", Convert.ToString(amp.Type));
                label.Add("Length", Convert.ToString(amp.Length));
            }

            return label;
        }

        private void _BuildLabels()
        {
            if (_dimension != null)
            {
                foreach (FieldListTypeField field in _dimension.Fields)
                {
                    _labels.Add(_CreateLabelDictionary(field));
                }
            }
        }

        private void _CreateDrawRuleDictionary(string id)
        {
            LibraryDrawRule drawRule = _librarian.DrawRule(id);
            
            if (drawRule != null)
            {
                _drawRule.Add("Name", drawRule.ID);
                _drawRule.Add("AnchorPoints", drawRule.AnchorPoints);
                _drawRule.Add("SizeShape", drawRule.SizeShape);
                _drawRule.Add("Orientation", drawRule.Orientation);
            }
            else
            {
                logger.Error("Draw rule " + id + "could not be found.");
            }
        }

        private void _BuildDrawRule()
        {
            string id = "";
            _drawRule.Clear();
            _drawNote = "";

            if(_entitySubType != null)
            {
                id = _entitySubType.DrawRuleID;
                _drawNote = _entitySubType.DrawNote;
            } 
            else if(_entityType != null)
            {
                id = _entityType.DrawRuleID;
                _drawNote = _entityType.DrawNote;
            } 
            else if (_entity != null)
            {
                id = _entity.DrawRuleID;
                _drawNote = _entity.DrawNote;
            }

            if(id != "")
                _CreateDrawRuleDictionary(id);
        }

        private void _BuildEntityTypeGraphics()
        {
            string graphic = "";

            string path = _configHelper.GetPath(_symbolSet.ID, FindEnum.FindEntities);

            if (_entityType != null)
            {
                if (_entityType.Icon == IconType.FULL_FRAME)
                {
                    switch (_sig.ID)
                    {
                        case "SIG_UNKNOWN":
                            graphic = _entityType.CloverGraphic;
                            break;

                        case "SIG_FRIEND":
                            graphic = _entityType.RectangleGraphic;
                            break;

                        case "SIG_NEUTRAL":
                            graphic = _entityType.SquareGraphic;
                            break;

                        case "SIG_HOSTILE":
                            graphic = _entityType.DiamondGraphic;
                            break;
                    }
                }
                else if (_entityType.Graphic != "")
                    graphic = _entityType.Graphic;
            }
            else if (_entity != null)
            {
                if (_entity.Icon == IconType.FULL_FRAME)
                {
                    switch (_sig.ID)
                    {
                        case "SIG_UNKNOWN":
                            graphic = _entity.CloverGraphic;
                            break;

                        case "SIG_FRIEND":
                            graphic = _entity.RectangleGraphic;
                            break;

                        case "SIG_NEUTRAL":
                            graphic = _entity.SquareGraphic;
                            break;

                        case "SIG_HOSTILE":
                            graphic = _entity.DiamondGraphic;
                            break;
                    }
                }
                else if (_entity.Graphic != "")
                    graphic = _entity.Graphic;
            }

            if(graphic != null)
                _graphics.Add(_configHelper.BuildOriginalPath(path, graphic));
        }

        private void _BuildGraphics()
        {
            // Creates a list of fully qualified pathnames for the svg files needed to
            // display this particular symbol.

            string path = "";
            string graphic = "";

            // Find the frame svg

            if (_affiliation != null)
            {
                path = _configHelper.GetPath(_context.ID, FindEnum.FindFrames);
                path = _configHelper.BuildOriginalPath(path, (_status.StatusCode == 1 && _affiliation.PlannedGraphic != "") ? _affiliation.PlannedGraphic : _affiliation.Graphic);
                _graphics.Add(path);
            }

            // Now add a single svg for the main/central icon, starting with the entitySubType, if any

            if (_symbolSet != null)
            {
                graphic = "";

                if (_entitySubType != null)
                {
                    if(_entitySubType.Icon == IconType.SPECIAL)
                        path = _configHelper.GetPath(_symbolSet.ID, FindEnum.FindSpecials);
                    else
                        path = _configHelper.GetPath(_symbolSet.ID, FindEnum.FindEntities);

                    if (_entitySubType.Icon == IconType.FULL_FRAME || _entitySubType.Icon == IconType.SPECIAL)
                    {
                        switch (_sig.ID)
                        {
                            case "SIG_UNKNOWN":
                                graphic = _entitySubType.CloverGraphic;
                                break;

                            case "SIG_FRIEND":
                                graphic = _entitySubType.RectangleGraphic;
                                break;

                            case "SIG_NEUTRAL":
                                graphic = _entitySubType.SquareGraphic;
                                break;

                            case "SIG_HOSTILE":
                                graphic = _entitySubType.DiamondGraphic;
                                break;
                        }
                    }
                    else if (_entitySubType.Graphic != "")
                        graphic = _entitySubType.Graphic;

                    _graphics.Add(_configHelper.BuildOriginalPath(path, graphic));

                    if (_entitySubType.Icon == IconType.SPECIAL)
                        _BuildEntityTypeGraphics();
                }
                else
                    _BuildEntityTypeGraphics();
            }

            // Let's add the modifiers

            if (_modifierOne != null)
            {
                path = _configHelper.GetPath(_symbolSet.ID, FindEnum.FindModifierOnes);
                path = _configHelper.BuildOriginalPath(path, _modifierOne.Graphic);
                _graphics.Add(path);
            }

            if (_modifierTwo != null)
            {
                path = _configHelper.GetPath(_symbolSet.ID, FindEnum.FindModifierTwos);
                path = _configHelper.BuildOriginalPath(path, _modifierTwo.Graphic);
                _graphics.Add(path);
            }

            // Amplifier?

            if (_sig != null && _amplifierGroup != null && _amplifier != null)
            {
                if (_amplifier.Graphics != null)
                {
                    switch (_amplifierGroup.AmplifierGroupCode)
                    {
                        case 1:
                        case 2:
                            path = _configHelper.GetPath("", FindEnum.FindEchelons);
                            break;

                        case 3:
                        case 4:
                        case 5:
                            path = _configHelper.GetPath("", FindEnum.FindMobilities);
                            break;

                        case 6:
                            path = _configHelper.GetPath("", FindEnum.FindAuxiliaryEquipment);
                            break;
                    }

                    graphic = "";

                    foreach (LibraryAmplifierGroupAmplifierGraphic lag in _amplifier.Graphics)
                    {
                        if (lag.StandardIdentityGroup == _sig.ID)
                        {
                            graphic = lag.Graphic;
                        }
                    }

                    if (graphic != "")
                    {
                        path = _configHelper.BuildOriginalPath(path, graphic);
                        _graphics.Add(path);
                    }
                }
            }

            // HQTFFD?

            if (_sig != null && _hqTFDummy != null)
            {
                if (_hqTFDummy.Graphics != null)
                {
                    path = _configHelper.GetPath("", FindEnum.FindHQTFFD);

                    graphic = "";

                    foreach (LibraryHQTFDummyGraphic hag in _hqTFDummy.Graphics)
                    {
                        if (hag.StandardIdentityGroup == _sig.ID && hag.Dimension == _dimension.ID)
                        {
                            graphic = hag.Graphic;
                            break;
                        }
                    }

                    if (graphic != "")
                    {
                        path = _configHelper.BuildOriginalPath(path, graphic);
                        _graphics.Add(path);
                    }
                }
            }

            // OCA?

            if (_status != null && _dimension != null && _standardIdentity != null)
            {
                if (_status.Graphics != null)
                {
                    graphic = "";

                    if (_colorBarOCA && _status.StatusCode > 1)
                    {
                        foreach (LibraryStatusGraphic g in _status.Graphics)
                        {
                            if (g.StandardIdentity == _standardIdentity.ID && g.Dimension == _dimension.ID)
                            {
                                graphic = g.Graphic;
                                break;
                            }
                        }
                    }
                    else if (_status.Graphic != null)
                    {
                        graphic = _status.Graphic;
                    }

                    if (graphic != "")
                    {
                        path = _configHelper.GetPath("", FindEnum.FindOCA);
                        path = _configHelper.BuildOriginalPath(path, graphic);
                        _graphics.Add(path);
                    }
                }
            }
        }

        private void _BuildNames()
        {
            _names.Clear();

            EntityExport ee = new DomainEntityExport(_configHelper);
            _names.Add("Entity",ee.NameIt(this.IconType == JointMilitarySymbologyLibrary.IconType.FULL_FRAME ? _sig : null, _symbolSet, _entity, _entityType, _entitySubType));

            ModifierExport me = new DomainModifierExport(_configHelper);
            _names.Add("ModifierOne", me.NameIt(_symbolSet, "1", _modifierOne));
            _names.Add("ModifierTwo", me.NameIt(_symbolSet, "2", _modifierTwo));

            FrameExport fe = new DomainFrameExport(_configHelper);
            _names.Add("Frame", fe.NameIt(_context, _dimension, _standardIdentity, _status));

            HQTFFDExport he = new DomainHQTFFDExport(_configHelper);
            _names.Add("HQTFFD", he.NameIt(_sig, _dimension, _hqTFDummy));

            AmplifierExport ae = new DomainAmplifierExport(_configHelper);
            _names.Add("Amplifier", ae.NameIt(_amplifierGroup, _amplifier, _sig));

            OCAExport oe = new DomainOCAExport(_configHelper);
            _names.Add("OCA", oe.NameIt(_standardIdentity, _dimension, _status));
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
            // Builds a current (2525D) SIDC from the JMSML Library elements currently associated 
            // with this symbol.

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

        private string _BuildLegacySIDC(LegacyFunctionCodeType functionCode)
        {
            string result = "";

            if (_symbolSet != null && _affiliation != null && _dimension != null &&
               _status != null && _amplifierGroup != null && _amplifier != null && _context.ContextCode != 2)
            {

                if (_legacySymbol != null)
                {
                    // Schema
                    if (functionCode.SchemaOverride != "")
                        result = functionCode.SchemaOverride;
                    else
                        result = _symbolSet.LegacyCodingSchemeCode[0].Value;

                    // Standard Identity
                    if (functionCode.StandardIdentityOverride != "")
                        result = result + functionCode.StandardIdentityOverride;
                    else
                        result = result + _affiliation.LegacyStandardIdentityCode[0].Value;

                    // Dimension
                    if (functionCode.DimensionOverride != "")
                        result = result + functionCode.DimensionOverride;
                    else
                        result = result + _dimension.LegacyDimensionCode[0].Value;

                    // Status
                    if (functionCode.StatusOverride != "")
                        result = result + functionCode.StatusOverride;
                    else
                        result = result + _status.LegacyStatusCode[0].Value;

                    // Function
                    result = result + functionCode.Value;
                }
                else
                {
                    result = _symbolSet.LegacyCodingSchemeCode[0].Value +
                                  _affiliation.LegacyStandardIdentityCode[0].Value +
                                  _dimension.LegacyDimensionCode[0].Value +
                                  _status.LegacyStatusCode[0].Value +
                                  _blankLegacyFunction;
                }

                // HQTFFD
                if (functionCode.HQTFFDOverride != "")
                    result = result + functionCode.HQTFFDOverride;
                else
                    result = result + _amplifierGroup.LegacyModifierCode[0].Value;

                // Amplifier
                if (functionCode.AmplifierOverride != "")
                    result = result + functionCode.AmplifierOverride;
                else
                    result = result + _amplifier.LegacyModifierCode[0].Value;

                //Tail
                if (functionCode.TailOverride != "")
                    result = result + functionCode.TailOverride;
                else
                    result = result + _blankLegacyTail;
            }

            return result;
        }

        private void _BuildLegacySIDC()
        {
            // Builds an older/legacy SIDC (for 2525C, 2525B, etc) from the JMSML Library elements currently associated 
            // with this symbol.

            _legacySIDC = _BuildLegacySIDC(_legacySymbol.LegacyFunctionCode[0]);
        }

        private void _UpdateFromCurrent()
        {
            // Search for the appropriate JMSML Library elements, given the current (2525D)
            // SIDC for this Symbol.

            string first10 = _sidc.PartAString;
            string second10 = _sidc.PartBString;

            _librarian.ResetStatusCode();

            _version = _librarian.Version(Convert.ToUInt16(first10.Substring(0, 1)),
                                          Convert.ToUInt16(first10.Substring(1, 1)));
            
            _context = _librarian.Context(Convert.ToUInt16(first10.Substring(2, 1)));
            _standardIdentity = _librarian.StandardIdentity(Convert.ToUInt16(first10.Substring(3, 1)));
            _sig = _librarian.StandardIdentityGroup(_standardIdentity);
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

                    if (_entitySubType == null)
                    {
                        _entitySubType = _librarian.EntitySubType(_symbolSet, Convert.ToUInt16(second10.Substring(4, 1)), Convert.ToUInt16(second10.Substring(5, 1)));
                    }
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
            // Search for the appropriate JMSML Library elements, given the older/legacy (2525C, 2525B, etc.)
            // SIDC for this Symbol.

            _librarian.ResetStatusCode();

            _version = _librarian.Version(1, 0);

            _legacySymbol = _librarian.LegacySymbol(_legacySIDC, ref _dimension, ref _symbolSet);

            if (_legacySymbol != null)
            {
                _affiliation = _librarian.AffiliationByLegacyCode(_legacySIDC.Substring(1, 1), _dimension.ID);

                if (_affiliation != null)
                {
                    _context = _librarian.Context(_affiliation.ContextID);
                    _standardIdentity = _librarian.StandardIdentity(_affiliation.StandardIdentityID);
                    _sig = _librarian.StandardIdentityGroup(_standardIdentity);
                }

                _status = _librarian.Status(_legacySIDC.Substring(3, 1), _legacySIDC.Substring(0, 1));
                _hqTFDummy = _librarian.HQTFDummy(_legacySIDC.Substring(10, 1));

                if (_context != null && _affiliation != null)
                {
                    _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);
                }

                _amplifier = _librarian.Amplifier(_legacySIDC.Substring(11, 1), _legacySIDC.Substring(0, 1));

                if (_amplifier != null)
                {
                    _amplifierGroup = _librarian.AmplifierGroup(_amplifier);
                }

                _entity = _librarian.Entity(_symbolSet, _legacySymbol.EntityID);
                _entityType = _librarian.EntityType(_entity, _legacySymbol.EntityTypeID);
                _entitySubType = _librarian.EntitySubType(_symbolSet, _entityType, _legacySymbol.EntitySubTypeID);
                _modifierOne = _librarian.ModifierOne(_symbolSet, _legacySymbol.ModifierOneID);
                _modifierTwo = _librarian.ModifierTwo(_symbolSet, _legacySymbol.ModifierTwoID);
            }

            _librarian.LogConversionResult(_legacySIDC);
            
            _ValidateStatus();
        }

        private void _SetInvalidSymbolProps()
        {
            // If a symbol can't be recognized, this method sets up
            // the current symbol to represent the special Invalid symbol.

            _version = _librarian.Version(1, 0);
            _context = _librarian.Context(0);
            _dimension = _librarian.Dimension("INTERNAL");
            _standardIdentity = _librarian.StandardIdentity(1);
            _symbolSet = _librarian.SymbolSet(9, 8);
            _status = _librarian.Status(0);
            _hqTFDummy = _librarian.HQTFDummy(0);
            _amplifierGroup = _librarian.AmplifierGroup(0);
            _amplifier = _librarian.Amplifier(_amplifierGroup, 0);
            _affiliation = _librarian.Affiliation("REALITY", "INTERNAL", "SI_UNKNOWN");
            _contextAmplifier = _librarian.ContextAmplifier(_context, _affiliation.Shape);
            _entity = _librarian.Entity(_symbolSet, "INVALID");
            _entityType = null;
            _entitySubType = null;
            _modifierOne = null;
            _modifierTwo = null;
        }

        private void _ValidateStatus()
        {
            // Validates the legacy translation of this Symbol.  If there
            // are issues with the conversion then those issues are logged externally.

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
                    _SetInvalidSymbolProps();
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
