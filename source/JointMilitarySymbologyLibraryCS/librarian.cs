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
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace JointMilitarySymbologyLibrary
{
    public class Librarian
    {
        private string _configPath;
        private jmsmlConfig _configData;
        private Library _library;

        private List<SymbolSet> _symbolSets = new List<SymbolSet>();

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public Librarian(string configPath = "")
        {
            InitializeNLog();

            //
            // Deserialize the configuration xml to get the location of the symbology library
            //

            XmlSerializer serializer = new XmlSerializer(typeof(jmsmlConfig));

            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

            if (configPath != "")
            {
                _configPath = configPath;
            }
            else
            {
                string s = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _configPath = Path.Combine(s, "jmsml.config");
            }

            if (File.Exists(_configPath))
            {
                FileStream fs = new FileStream(_configPath, FileMode.Open, FileAccess.Read);

                if (fs.CanRead)
                {
                    _configData = (jmsmlConfig)serializer.Deserialize(fs);

                    //
                    // Deserialize the library's base xml to get the base contents of the symbology standard
                    //

                    serializer = new XmlSerializer(typeof(Library));

                    string path = _configData.libraryPath + "/" + _configData.libraryName;

                    if (File.Exists(path))
                    {
                        fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                        if (fs.CanRead)
                        {
                            this._library = (Library)serializer.Deserialize(fs);

                            //
                            // Deserialize each symbolSet xml
                            //

                            foreach (LibraryDimension dimension in this._library.Dimensions)
                            {
                                foreach (LibraryDimensionSymbolSetRef ssRef in dimension.SymbolSets)
                                {
                                    path = _configData.libraryPath + "/" + ssRef.Instance;
                                    if (File.Exists(path))
                                    {
                                        fs = new FileStream(path, FileMode.Open, FileAccess.Read);

                                        if (fs.CanRead)
                                        {
                                            serializer = new XmlSerializer(typeof(SymbolSet));
                                            SymbolSet ss = (SymbolSet)serializer.Deserialize(fs);

                                            if (ss != null)
                                            {
                                                _symbolSets.Add(ss);
                                            }
                                        }
                                        else
                                        {
                                            logger.Error("Unreadable symbol set: " + path);
                                        }
                                    }
                                    else
                                    {
                                        logger.Error("Symbol set is missing: " + path);
                                    }
                                }
                            }
                        }
                        else
                        {
                            logger.Error("Unreadable symbol library: " + path);
                        }
                    }
                    else
                    {
                        logger.Error("Specified library is missing: " + path);
                    }
                }
                else
                {
                    logger.Error("Unreadable config file: " + _configPath);
                }
            }
            else
            {
                logger.Error("Config file is missing: " + _configPath);
            }
        }

        private void InitializeNLog()
        {
            Target logTarget = new FileTarget();

            ((FileTarget)logTarget).FileName = @"${specialfolder:MyDocuments}/jmsml/logs/${shortdate}.log";

            ((FileTarget)logTarget).Layout = new SimpleLayout("${longdate} | ${level} | ${callsite} | ${message} | ${exception:format=ToString}");

            /*
             * Write the log asynchronously
             */
            AsyncTargetWrapper wrapper = new AsyncTargetWrapper();
            wrapper.WrappedTarget = logTarget;
            wrapper.QueueLimit = 5000;
            wrapper.OverflowAction = AsyncTargetWrapperOverflowAction.Grow;

            SimpleConfigurator.ConfigureForTargetLogging(wrapper, LogLevel.Debug);
        }

        public Library Library 
        {
            get
            {
                return this._library;
            }
        }

        public string Graphics
        {
            get
            {
                return this._configData.graphicPath;
            }
        }

        public LibraryVersion Version(ushort codeOne, ushort codeTwo)
        {
            LibraryVersion retObj = null;

            foreach (LibraryVersion lObj in this._library.Versions)
            {
                if (lObj.VersionCode.DigitOne == codeOne &&
                    lObj.VersionCode.DigitTwo == codeTwo)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryContext Context(ushort code)
        {
            LibraryContext retObj = null;

            foreach (LibraryContext lObj in this._library.Contexts)
            {
                if (lObj.ContextCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryContext Context(string id)
        {
            LibraryContext retObj = null;

            foreach (LibraryContext lObj in this._library.Contexts)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryDimension Dimension(string id)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                if (lObj.ID == id)
                {
                    retObj = lObj;
                    break;
                }
            }

            return retObj;
        }

        public LibraryDimension DimensionBySymbolSet(string symbolSetID)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                foreach(LibraryDimensionSymbolSetRef ssRef in lObj.SymbolSets)
                {
                    if (ssRef.ID == symbolSetID)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public LibraryDimension DimensionByLegacyCode(string code)
        {
            LibraryDimension retObj = null;

            foreach (LibraryDimension lObj in this._library.Dimensions)
            {
                foreach (LegacyLetterCodeType lObj2 in lObj.LegacyDimensionCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public LibraryStandardIdentity StandardIdentity(ushort code)
        {
            LibraryStandardIdentity retObj = null;

            foreach (LibraryStandardIdentity lObj in this._library.StandardIdentities)
            {
                if (lObj.StandardIdentityCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryStandardIdentity StandardIdentity(string id)
        {
            LibraryStandardIdentity retObj = null;

            foreach (LibraryStandardIdentity lObj in this._library.StandardIdentities)
            {
                if (lObj.ID == id)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public SymbolSet SymbolSet(ushort symbolSetCodeOne, ushort symbolSetCodeTwo)
        {
            SymbolSet retObj = null;

            foreach (SymbolSet lObj in this._symbolSets)
            {
                if (lObj.SymbolSetCode.DigitOne == symbolSetCodeOne &&
                    lObj.SymbolSetCode.DigitTwo == symbolSetCodeTwo)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public SymbolSet SymbolSet(string dimensionID, string code)
        {
            SymbolSet retObj = null;

            foreach (SymbolSet lObj in this._symbolSets)
            {
                if (lObj.DimensionID == dimensionID)
                {
                    if (lObj.LegacySymbols != null)
                    {
                        foreach (SymbolSetLegacySymbol lObj2 in lObj.LegacySymbols)
                        {
                            foreach (LegacyFunctionCodeType lObj3 in lObj2.LegacyFunctionCode)
                            {
                                if (lObj3.Value == code)
                                {
                                    return lObj;
                                }
                            }
                        }
                    }
                }
            }

            return retObj;
        }

        public LibraryStatus Status(ushort code)
        {
            LibraryStatus retObj = null;

            foreach (LibraryStatus lObj in this._library.Statuses)
            {
                if (lObj.StatusCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryStatus Status(string code)
        {
            LibraryStatus retObj = null;

            foreach (LibraryStatus lObj in this._library.Statuses)
            {
                if (lObj.LegacyStatusCode != null)
                {
                    foreach (LegacyLetterCodeType lObj2 in lObj.LegacyStatusCode)
                    {
                        if (lObj2.Value == code)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public LibraryHQTFDummy HQTFDummy(ushort code)
        {
            LibraryHQTFDummy retObj = null;

            foreach (LibraryHQTFDummy lObj in this._library.HQTFDummies)
            {
                if (lObj.HQTFDummyCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryHQTFDummy HQTFDummy(string code)
        {
            LibraryHQTFDummy retObj = null;

            foreach (LibraryHQTFDummy lObj in this._library.HQTFDummies)
            {
                foreach (LegacyLetterCodeType lObj2 in lObj.LegacyHQTFDummyCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            if(retObj == null)
            {
                retObj = this.HQTFDummy(0);
            }

            return retObj;
        }

        public LibraryAmplifierGroup AmplifierGroup(ushort code)
        {
            LibraryAmplifierGroup retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                if (lObj.AmplifierGroupCode == code)
                {

                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryAmplifierGroup AmplifierGroup(LibraryAmplifierGroupAmplifier amplifier)
        {
            LibraryAmplifierGroup retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                foreach (LibraryAmplifierGroupAmplifier lObj2 in lObj.Amplifiers)
                {
                    if (lObj2.Equals(amplifier))
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public LibraryAmplifierGroupAmplifier Amplifier(LibraryAmplifierGroup group, ushort code)
        {
            LibraryAmplifierGroupAmplifier retObj = null;

            if (group != null)
            {
                foreach (LibraryAmplifierGroupAmplifier lObj in group.Amplifiers)
                {
                    if (lObj.AmplifierCode == code)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public LibraryAmplifierGroupAmplifier Amplifier(ushort groupCode, ushort code)
        {
            LibraryAmplifierGroup group = this.AmplifierGroup(groupCode);

            LibraryAmplifierGroupAmplifier retObj = this.Amplifier(group, code);

            return retObj;
        }

        public LibraryAmplifierGroupAmplifier Amplifier(string code)
        {
            LibraryAmplifierGroupAmplifier retObj = null;

            foreach (LibraryAmplifierGroup lObj in this._library.AmplifierGroups)
            {
                foreach (LibraryAmplifierGroupAmplifier lObj2 in lObj.Amplifiers)
                {
                    foreach (LegacyLetterCodeType lObj3 in lObj2.LegacyModifierCode)
                    {
                        if (lObj3.Value == code)
                        {
                            return lObj2;
                        }
                    }
                }
            }
            
            return retObj;
        }

        public LibraryAffiliation Affiliation(string contextID, string dimensionID, string standardIdentityID)
        {
            LibraryAffiliation retObj = null;

            foreach (LibraryAffiliation lObj in this._library.Affiliations)
            {
                if (lObj.ContextID == contextID && lObj.DimensionID == dimensionID && lObj.StandardIdentityID == standardIdentityID)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public LibraryAffiliation Affiliation(string legacyStandardIdentityCode, string legacyDimensionCode)
        {
            LibraryAffiliation retObj = null;

            foreach(LibraryAffiliation lObj in this._library.Affiliations)
            {
                if (lObj.LegacyStandardIdentityCode != null)
                {
                    foreach (LegacyLetterCodeType lObj2 in lObj.LegacyStandardIdentityCode)
                    {
                        if (lObj2.Value == legacyStandardIdentityCode)
                        {
                            LibraryDimension lDim = this.Dimension(lObj.DimensionID);
                            
                            foreach (LegacyLetterCodeType lObj3 in lDim.LegacyDimensionCode)
                            {
                                if(lObj3.Value == legacyDimensionCode)
                                    return lObj;
                            }
                        }
                    }
                }
            }

            return retObj;
        }

        public LibraryContextContextAmplifier ContextAmplifier(LibraryContext context, ShapeType shape)
        {
            LibraryContextContextAmplifier retObj = null;

            if (context != null)
            {
                if (context.ContextAmplifiers != null)
                {
                    foreach (LibraryContextContextAmplifier lObj in context.ContextAmplifiers)
                    {
                        if (lObj.Shape == shape)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetEntity Entity(SymbolSet symbolSet, ushort entityCodeOne, ushort entityCodeTwo)
        {
            SymbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.Entities != null)
                {
                    foreach (SymbolSetEntity lObj in symbolSet.Entities)
                    {
                        if (lObj.EntityCode.DigitOne == entityCodeOne &&
                            lObj.EntityCode.DigitTwo == entityCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }
  
            return retObj;
        }

        public SymbolSetEntity Entity(SymbolSet symbolSet, string entityID)
        {
            SymbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.Entities != null)
                {
                    foreach (SymbolSetEntity lObj in symbolSet.Entities)
                    {
                        if (lObj.ID == entityID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetEntityEntityType EntityType(SymbolSetEntity entity, ushort entityTypeCodeOne, ushort entityTypeCodeTwo)
        {
            SymbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.EntityTypes != null)
                {
                    foreach (SymbolSetEntityEntityType lObj in entity.EntityTypes)
                    {
                        if (lObj.EntityTypeCode.DigitOne == entityTypeCodeOne &&
                            lObj.EntityTypeCode.DigitTwo == entityTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetEntityEntityType EntityType(SymbolSetEntity entity, string entityTypeID)
        {
            SymbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.EntityTypes != null)
                {
                    foreach (SymbolSetEntityEntityType lObj in entity.EntityTypes)
                    {
                        if (lObj.ID == entityTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetEntityEntityTypeEntitySubType EntitySubType(SymbolSetEntityEntityType entityType, ushort entitySubTypeCodeOne, ushort entitySubTypeCodeTwo)
        {
            SymbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.EntitySubTypes != null)
                {
                    foreach (SymbolSetEntityEntityTypeEntitySubType lObj in entityType.EntitySubTypes)
                    {
                        if (lObj.EntitySubTypeCode.DigitOne == entitySubTypeCodeOne &&
                            lObj.EntitySubTypeCode.DigitTwo == entitySubTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetEntityEntityTypeEntitySubType EntitySubType(SymbolSetEntityEntityType entityType, string entitySubTypeID)
        {
            SymbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.EntitySubTypes != null)
                {
                    foreach (SymbolSetEntityEntityTypeEntitySubType lObj in entityType.EntitySubTypes)
                    {
                        if (lObj.ID == entitySubTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public ModifiersTypeModifier ModifierOne(SymbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if (symbolSet.SectorOneModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorOneModifiers)
                    {
                        if (lObj.ModifierCode.DigitOne == modifierCodeOne &&
                           lObj.ModifierCode.DigitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public ModifiersTypeModifier ModifierOne(SymbolSet symbolSet, string modifierID)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorOneModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorOneModifiers)
                    {
                        if (lObj.ID == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public ModifiersTypeModifier ModifierTwo(SymbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorTwoModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorTwoModifiers)
                    {
                        if (lObj.ModifierCode.DigitOne == modifierCodeOne &&
                           lObj.ModifierCode.DigitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public ModifiersTypeModifier ModifierTwo(SymbolSet symbolSet, string modifierID)
        {
            ModifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.SectorTwoModifiers != null)
                {
                    foreach (ModifiersTypeModifier lObj in symbolSet.SectorTwoModifiers)
                    {
                        if (lObj.ID == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public SymbolSetLegacySymbol LegacySymbol(SymbolSet symbolSet, string functionCode)
        {
            SymbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                foreach (SymbolSetLegacySymbol lObj in symbolSet.LegacySymbols)
                {
                    foreach (LegacyFunctionCodeType lObj2 in lObj.LegacyFunctionCode)
                    {
                        if (lObj2.Value == functionCode)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public SymbolSetLegacySymbol LegacySymbol(SymbolSet symbolSet, 
                                                  SymbolSetEntity entity, 
                                                  SymbolSetEntityEntityType entityType, 
                                                  SymbolSetEntityEntityTypeEntitySubType entitySubType, 
                                                  ModifiersTypeModifier modifierOne, 
                                                  ModifiersTypeModifier modifierTwo)
        {
            SymbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.LegacySymbols != null)
                {
                    int match = 0;

                    foreach (SymbolSetLegacySymbol lObj in symbolSet.LegacySymbols)
                    {
                        if(entity != null)
                        {
                            if (lObj.EntityID != "NA")
                            {
                                if (lObj.EntityID == entity.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntityID == "NA")
                            match++;

                        if(entityType != null)
                        {
                            if (lObj.EntityTypeID != "NA")
                            {
                                if (lObj.EntityTypeID == entityType.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntityTypeID == "NA")
                            match++;

                        if(entitySubType != null)
                        {
                            if (lObj.EntitySubTypeID != "NA")
                            {
                                if (lObj.EntitySubTypeID == entitySubType.ID)
                                    match++;
                            }
                        }
                        else if(lObj.EntitySubTypeID == "NA")
                            match++;

                        if(modifierOne != null)
                        {
                            if (lObj.ModifierOneID != "NA")
                            {
                                if (lObj.ModifierOneID == modifierOne.ID)
                                    match++;
                            }
                        }
                        else if(lObj.ModifierOneID == "NA")
                            match++;

                        if(modifierTwo != null)
                        {
                            if (lObj.ModifierTwoID != "NA")
                            {
                                if (lObj.ModifierTwoID == modifierTwo.ID)
                                    match++;
                            }
                        }
                        else if(lObj.ModifierTwoID == "NA")
                            match++;

                        if(match == 5)
                        {
                            return lObj;
                        }
                        
                        match = 0;
                    }
                }
            }

            return retObj;
        }

        public Symbol MakeSymbol(string legacyStandard, string legacySIDC)
        {
            return new Symbol(this, legacyStandard, legacySIDC);
        }

        public Symbol MakeSymbol(UInt32 partA, UInt32 partB)
        {
            SIDC sid = new SIDC(partA, partB);
            return new Symbol(this, sid);
        }

        public Symbol MakeSymbol(SIDC sidc)
        {
            return new Symbol(this, sidc);
        }

        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
        }

        private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Console.WriteLine("Unknown attribute " +
            attr.Name + "='" + attr.Value + "'");
        }
    }
}
