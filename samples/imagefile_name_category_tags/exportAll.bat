setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Activities-Source-Icons-Test" /s="Activities" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Air-Source-Icons-Test" /s="^Air|Air Missile$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Cyberspace-Source-Icons-Test" /s="^Cyberspace$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Land-Source-Icons-Test" /s="^Land Units|Land Civilian|Land Equipment|Land Installation$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Sea-Source-Icons-Test" /s="^Sea Surface|Sea Subsurface|Mine Warfare$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Space-Source-Icons-Test" /s="^Space|Space Missile$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Sigint-Source-Icons-Test" /s="Signals Intelligence" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-ControlMeasures-Source-Icons-Test" /s="^Control Measure$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Weather-Source-Icons-Test" /s="Atmospheric|Oceanographic|Meteorological Space" /+
endlocal