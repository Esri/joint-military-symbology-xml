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
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace JointMilitarySymbologyLibrary
{
    public class librarian
    {
        private string _configPath = "../../Resources/jmsml.config";
        private jmsmlConfig _configData;
        private library _library;

        private List<symbolSet> _symbolSets = new List<symbolSet>();

        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public librarian(string configPath = "")
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

            if (File.Exists(_configPath))
            {
                FileStream fs = new FileStream(_configPath, FileMode.Open);

                if (fs.CanRead)
                {
                    _configData = (jmsmlConfig)serializer.Deserialize(fs);

                    //
                    // Deserialize the library's base xml to get the base contents of the symbology standard
                    //

                    serializer = new XmlSerializer(typeof(library));

                    string path = _configData.libraryPath + "/" + _configData.libraryName;

                    if (File.Exists(path))
                    {
                        fs = new FileStream(path, FileMode.Open);

                        if (fs.CanRead)
                        {
                            this._library = (library)serializer.Deserialize(fs);

                            //
                            // Deserialize each symbolSet xml
                            //

                            foreach (libraryDimension dimension in this._library.dimensions)
                            {
                                foreach (libraryDimensionSymbolSetRef ssRef in dimension.symbolSets)
                                {
                                    path = _configData.libraryPath + "/" + ssRef.instance;
                                    if (File.Exists(path))
                                    {
                                        fs = new FileStream(path, FileMode.Open);

                                        if (fs.CanRead)
                                        {
                                            serializer = new XmlSerializer(typeof(symbolSet));
                                            symbolSet ss = (symbolSet)serializer.Deserialize(fs);

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

        public library library 
        {
            get
            {
                return this._library;
            }
        }

        public string graphics
        {
            get
            {
                return this._configData.graphicPath;
            }
        }

        public libraryVersion version(ushort codeOne, ushort codeTwo)
        {
            libraryVersion retObj = null;

            foreach (libraryVersion lObj in this._library.versions)
            {
                if (lObj.versionCode.digitOne == codeOne &&
                    lObj.versionCode.digitTwo == codeTwo)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryContext context(ushort code)
        {
            libraryContext retObj = null;

            foreach (libraryContext lObj in this._library.contexts)
            {
                if (lObj.contextCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryContext context(string id)
        {
            libraryContext retObj = null;

            foreach (libraryContext lObj in this._library.contexts)
            {
                if (lObj.id == id)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryDimension dimension(string id)
        {
            libraryDimension retObj = null;

            foreach (libraryDimension lObj in this._library.dimensions)
            {
                if (lObj.id == id)
                {
                    retObj = lObj;
                    break;
                }
            }

            return retObj;
        }

        public libraryDimension dimensionBySymbolSet(string symbolSetID)
        {
            libraryDimension retObj = null;

            foreach (libraryDimension lObj in this._library.dimensions)
            {
                foreach(libraryDimensionSymbolSetRef ssRef in lObj.symbolSets)
                {
                    if (ssRef.id == symbolSetID)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public libraryDimension dimensionByLegacyCode(string code)
        {
            libraryDimension retObj = null;

            foreach (libraryDimension lObj in this._library.dimensions)
            {
                foreach (legacyLetterCodeType lObj2 in lObj.legacyDimensionCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public libraryStandardIdentity standardIdentity(ushort code)
        {
            libraryStandardIdentity retObj = null;

            foreach (libraryStandardIdentity lObj in this._library.standardIdentities)
            {
                if (lObj.standardIdentityCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryStandardIdentity standardIdentity(string id)
        {
            libraryStandardIdentity retObj = null;

            foreach (libraryStandardIdentity lObj in this._library.standardIdentities)
            {
                if (lObj.id == id)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public symbolSet symbolSet(ushort symbolSetCodeOne, ushort symbolSetCodeTwo)
        {
            symbolSet retObj = null;

            foreach (symbolSet lObj in this._symbolSets)
            {
                if (lObj.symbolSetCode.digitOne == symbolSetCodeOne &&
                    lObj.symbolSetCode.digitTwo == symbolSetCodeTwo)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public symbolSet symbolSet(string dimensionID, string code)
        {
            symbolSet retObj = null;

            foreach (symbolSet lObj in this._symbolSets)
            {
                if (lObj.dimensionID == dimensionID)
                {
                    if (lObj.legacySymbols != null)
                    {
                        foreach (symbolSetLegacySymbol lObj2 in lObj.legacySymbols)
                        {
                            foreach (legacyFunctionCodeType lObj3 in lObj2.legacyFunctionCode)
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

        public libraryStatus status(ushort code)
        {
            libraryStatus retObj = null;

            foreach (libraryStatus lObj in this._library.statuses)
            {
                if (lObj.statusCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryStatus status(string code)
        {
            libraryStatus retObj = null;

            foreach (libraryStatus lObj in this._library.statuses)
            {
                foreach (legacyLetterCodeType lObj2 in lObj.legacyStatusCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public libraryHqTFDummy hqTFDummy(ushort code)
        {
            libraryHqTFDummy retObj = null;

            foreach (libraryHqTFDummy lObj in this._library.hqTFDummies)
            {
                if (lObj.hqTFDummyCode == code)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryHqTFDummy hqTFDummy(string code)
        {
            libraryHqTFDummy retObj = null;

            foreach (libraryHqTFDummy lObj in this._library.hqTFDummies)
            {
                foreach (legacyLetterCodeType lObj2 in lObj.legacyHQTFDummyCode)
                {
                    if (lObj2.Value == code)
                    {
                        return lObj;
                    }
                }
            }

            if(retObj == null)
            {
                retObj = this.hqTFDummy(0);
            }

            return retObj;
        }

        public libraryAmplifierGroup amplifierGroup(ushort code)
        {
            libraryAmplifierGroup retObj = null;

            foreach (libraryAmplifierGroup lObj in this._library.amplifierGroups)
            {
                if (lObj.amplifierGroupCode == code)
                {

                    return lObj;
                }
            }

            return retObj;
        }

        public libraryAmplifierGroup amplifierGroup(libraryAmplifierGroupAmplifier amplifier)
        {
            libraryAmplifierGroup retObj = null;

            foreach (libraryAmplifierGroup lObj in this._library.amplifierGroups)
            {
                foreach (libraryAmplifierGroupAmplifier lObj2 in lObj.amplifiers)
                {
                    if (lObj2.Equals(amplifier))
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public libraryAmplifierGroupAmplifier amplifier(libraryAmplifierGroup group, ushort code)
        {
            libraryAmplifierGroupAmplifier retObj = null;

            if (group != null)
            {
                foreach (libraryAmplifierGroupAmplifier lObj in group.amplifiers)
                {
                    if (lObj.amplifierCode == code)
                    {
                        return lObj;
                    }
                }
            }

            return retObj;
        }

        public libraryAmplifierGroupAmplifier amplifier(ushort groupCode, ushort code)
        {
            libraryAmplifierGroup group = this.amplifierGroup(groupCode);

            libraryAmplifierGroupAmplifier retObj = this.amplifier(group, code);

            return retObj;
        }

        public libraryAmplifierGroupAmplifier amplifier(string code)
        {
            libraryAmplifierGroupAmplifier retObj = null;

            foreach (libraryAmplifierGroup lObj in this._library.amplifierGroups)
            {
                foreach (libraryAmplifierGroupAmplifier lObj2 in lObj.amplifiers)
                {
                    foreach (legacyLetterCodeType lObj3 in lObj2.legacyModifierCode)
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

        public libraryAffiliation affiliation(string contextID, string dimensionID, string standardIdentityID)
        {
            libraryAffiliation retObj = null;

            foreach (libraryAffiliation lObj in this._library.affiliations)
            {
                if (lObj.contextID == contextID && lObj.dimensionID == dimensionID && lObj.standardIdentityID == standardIdentityID)
                {
                    return lObj;
                }
            }

            return retObj;
        }

        public libraryAffiliation affiliation(string legacyStandardIdentityCode, string legacyDimensionCode)
        {
            libraryAffiliation retObj = null;

            foreach(libraryAffiliation lObj in this._library.affiliations)
            {
                if (lObj.legacyStandardIdentityCode != null)
                {
                    foreach (legacyLetterCodeType lObj2 in lObj.legacyStandardIdentityCode)
                    {
                        if (lObj2.Value == legacyStandardIdentityCode)
                        {
                            libraryDimension lDim = this.dimension(lObj.dimensionID);
                            
                            foreach (legacyLetterCodeType lObj3 in lDim.legacyDimensionCode)
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

        public libraryContextContextAmplifier contextAmplifier(libraryContext context, shapeType shape)
        {
            libraryContextContextAmplifier retObj = null;

            if (context != null)
            {
                if (context.contextAmplifiers != null)
                {
                    foreach (libraryContextContextAmplifier lObj in context.contextAmplifiers)
                    {
                        if (lObj.shape == shape)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public symbolSetEntity entity(symbolSet symbolSet, ushort entityCodeOne, ushort entityCodeTwo)
        {
            symbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.entities != null)
                {
                    foreach (symbolSetEntity lObj in symbolSet.entities)
                    {
                        if (lObj.entityCode.digitOne == entityCodeOne &&
                            lObj.entityCode.digitTwo == entityCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }
  
            return retObj;
        }

        public symbolSetEntity entity(symbolSet symbolSet, string entityID)
        {
            symbolSetEntity retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.entities != null)
                {
                    foreach (symbolSetEntity lObj in symbolSet.entities)
                    {
                        if (lObj.id == entityID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public symbolSetEntityEntityType entityType(symbolSetEntity entity, ushort entityTypeCodeOne, ushort entityTypeCodeTwo)
        {
            symbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.entityTypes != null)
                {
                    foreach (symbolSetEntityEntityType lObj in entity.entityTypes)
                    {
                        if (lObj.entityTypeCode.digitOne == entityTypeCodeOne &&
                            lObj.entityTypeCode.digitTwo == entityTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public symbolSetEntityEntityType entityType(symbolSetEntity entity, string entityTypeID)
        {
            symbolSetEntityEntityType retObj = null;

            if (entity != null)
            {
                if (entity.entityTypes != null)
                {
                    foreach (symbolSetEntityEntityType lObj in entity.entityTypes)
                    {
                        if (lObj.id == entityTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public symbolSetEntityEntityTypeEntitySubType entitySubType(symbolSetEntityEntityType entityType, ushort entitySubTypeCodeOne, ushort entitySubTypeCodeTwo)
        {
            symbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.entitySubTypes != null)
                {
                    foreach (symbolSetEntityEntityTypeEntitySubType lObj in entityType.entitySubTypes)
                    {
                        if (lObj.entitySubTypeCode.digitOne == entitySubTypeCodeOne &&
                            lObj.entitySubTypeCode.digitTwo == entitySubTypeCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public symbolSetEntityEntityTypeEntitySubType entitySubType(symbolSetEntityEntityType entityType, string entitySubTypeID)
        {
            symbolSetEntityEntityTypeEntitySubType retObj = null;

            if (entityType != null)
            {
                if (entityType.entitySubTypes != null)
                {
                    foreach (symbolSetEntityEntityTypeEntitySubType lObj in entityType.entitySubTypes)
                    {
                        if (lObj.id == entitySubTypeID)
                        {
                            return lObj;
                        }
                    }
                }
            }

            return retObj;
        }

        public modifiersTypeModifier modifierOne(symbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            modifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if (symbolSet.sectorOneModifiers != null)
                {
                    foreach (modifiersTypeModifier lObj in symbolSet.sectorOneModifiers)
                    {
                        if (lObj.modifierCode.digitOne == modifierCodeOne &&
                           lObj.modifierCode.digitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public modifiersTypeModifier modifierOne(symbolSet symbolSet, string modifierID)
        {
            modifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.sectorOneModifiers != null)
                {
                    foreach (modifiersTypeModifier lObj in symbolSet.sectorOneModifiers)
                    {
                        if (lObj.id == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public modifiersTypeModifier modifierTwo(symbolSet symbolSet, ushort modifierCodeOne, ushort modifierCodeTwo)
        {
            modifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.sectorTwoModifiers != null)
                {
                    foreach (modifiersTypeModifier lObj in symbolSet.sectorTwoModifiers)
                    {
                        if (lObj.modifierCode.digitOne == modifierCodeOne &&
                           lObj.modifierCode.digitTwo == modifierCodeTwo)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public modifiersTypeModifier modifierTwo(symbolSet symbolSet, string modifierID)
        {
            modifiersTypeModifier retObj = null;

            if (symbolSet != null)
                if(symbolSet.sectorTwoModifiers != null)
                {
                    foreach (modifiersTypeModifier lObj in symbolSet.sectorTwoModifiers)
                    {
                        if (lObj.id == modifierID)
                        {
                            return lObj;
                        }
                    }
                }

            return retObj;
        }

        public symbolSetLegacySymbol legacySymbol(symbolSet symbolSet, string functionCode)
        {
            symbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                foreach (symbolSetLegacySymbol lObj in symbolSet.legacySymbols)
                {
                    foreach (legacyFunctionCodeType lObj2 in lObj.legacyFunctionCode)
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

        public symbolSetLegacySymbol legacySymbol(symbolSet symbolSet, 
                                                  symbolSetEntity entity, 
                                                  symbolSetEntityEntityType entityType, 
                                                  symbolSetEntityEntityTypeEntitySubType entitySubType, 
                                                  modifiersTypeModifier modifierOne, 
                                                  modifiersTypeModifier modifierTwo)
        {
            symbolSetLegacySymbol retObj = null;

            if (symbolSet != null)
            {
                if (symbolSet.legacySymbols != null)
                {
                    int match = 0;

                    foreach (symbolSetLegacySymbol lObj in symbolSet.legacySymbols)
                    {
                        if(entity != null)
                        {
                            if (lObj.entityID != "NA")
                            {
                                if (lObj.entityID == entity.id)
                                    match++;
                            }
                        }
                        else if(lObj.entityID == "NA")
                            match++;

                        if(entityType != null)
                        {
                            if (lObj.entityTypeID != "NA")
                            {
                                if (lObj.entityTypeID == entityType.id)
                                    match++;
                            }
                        }
                        else if(lObj.entityTypeID == "NA")
                            match++;

                        if(entitySubType != null)
                        {
                            if (lObj.entitySubTypeID != "NA")
                            {
                                if (lObj.entitySubTypeID == entitySubType.id)
                                    match++;
                            }
                        }
                        else if(lObj.entitySubTypeID == "NA")
                            match++;

                        if(modifierOne != null)
                        {
                            if (lObj.modifierOneID != "NA")
                            {
                                if (lObj.modifierOneID == modifierOne.id)
                                    match++;
                            }
                        }
                        else if(lObj.modifierOneID == "NA")
                            match++;

                        if(modifierTwo != null)
                        {
                            if (lObj.modifierTwoID != "NA")
                            {
                                if (lObj.modifierTwoID == modifierTwo.id)
                                    match++;
                            }
                        }
                        else if(lObj.modifierTwoID == "NA")
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

        public symbol makeSymbol(string legacyStandard, string legacySIDC)
        {
            return new symbol(this, legacyStandard, legacySIDC);
        }

        public symbol makeSymbol(sidc sidc)
        {
            return new symbol(this, sidc);
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
