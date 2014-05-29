setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Activities-Source-Icons" /s="Activities" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Air-Source-Icons" /s="^Air|Air Missile$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Cyberspace-Source-Icons" /s="^Cyberspace$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Land-Source-Icons" /s="^Land Units|Land Civilian|Land Equipment|Land Installation$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Sea-Source-Icons" /s="^Sea Surface|Sea Subsurface|Mine Warfare$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Space-Source-Icons" /s="^Space|Space Missile$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Sigint-Source-Icons" /s="Signals Intelligence" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-ControlMeasures-Source-Icons" /s="^Control Measure$" /+
jmsml.exe /xi="../../../../../samples/imagefile_name_category_tags/Military-Weather-Source-Icons" /s="Atmospheric|Oceanographic|Meteorological Space" /+
jmsml.exe /xf="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons"
jmsml.exe /xa="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /+
jmsml.exe /xh="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /+
endlocal