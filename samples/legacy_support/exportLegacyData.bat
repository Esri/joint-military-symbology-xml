setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xl="../../../../../samples/legacy_support/LegacyMappingTableCtoD.csv"
jmsml.exe /xll="../../../../../samples/legacy_support/All_ID_Mapping_Latest.csv" /+amplifiers
jmsml.exe /xll="../../../../../samples/legacy_support/All_ID_Mapping_Original.csv" /asOriginal /+amplifiers
endlocal