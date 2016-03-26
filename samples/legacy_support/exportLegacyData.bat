setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xl="../../../../../samples/legacy_support/LegacyMappingTableCtoD.csv"
jmsml.exe /xllC="../../../../../samples/legacy_support/All_ID_Mapping_C_to_D.csv"
jmsml.exe /xllC="../../../../../samples/legacy_support/All_ID_Mapping_C_to_D_Original.csv" /asOriginal
endlocal