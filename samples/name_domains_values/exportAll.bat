setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Air" /s="^Air$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Air_Missile" /s="^Air Missile$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Space" /s="^Space$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Space_Missile" /s="^Space Missile$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Land_Unit" /s="^Land Units$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Land_Unit_Civilian" /s="^Land Civilian$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Land_Equipment" /s="^Land Equipment$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Land_Installation" /s="^Land Installation$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Sea_Surface" /s="^Sea Surface$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Sea_Subsurface" /s="^Sea Subsurface$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Sea_Subsurface_Mine_Warfare" /s="^Mine Warfare$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Activities" /s="^Activities$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Cyberspace" /s="^Cyberspace$" /xas="DOMAIN"

REM Added Dismounted for App6
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Dismounted" /s="^Dismounted" /xas="DOMAIN"

jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Control_Measure_Point" /p /s="^Control Measure$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Control_Measure_Line" /l /s="^Control Measure$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_Control_Measure_Area" /a /s="^Control Measure$" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Atmospheric_Point" /p /s="Atmospheric" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Oceanographic_Point" /p /s="Oceanographic" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Space_Point" /p /s="Meteorological Space" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Atmospheric_Line" /l /s="Atmospheric" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Oceanographic_Line" /l /s="Oceanographic" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Space_Line" /l /s="Meteorological Space" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Atmospheric_Area" /a /s="Atmospheric" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Oceanographic_Area" /a /s="Oceanographic" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/Coded_Domain_METOC_Space_Area" /a /s="Meteorological Space" /xas="DOMAIN"
jmsml.exe /xf="../../../../../samples/name_domains_values/Coded_Domain_Identity" /xas="DOMAIN"
jmsml.exe /xa="../../../../../samples/name_domains_values/Coded_Domain_Echelons" /xas="DOMAIN" /q="Echelon"
jmsml.exe /xa="../../../../../samples/name_domains_values/Coded_Domain_Mobilities" /xas="DOMAIN" /q="mobility"
jmsml.exe /xa="../../../../../samples/name_domains_values/Coded_Domain_Arrays" /xas="DOMAIN" /q="array"
jmsml.exe /xh="../../../../../samples/name_domains_values/Coded_Domain_HQTFFD" /xas="DOMAIN"
jmsml.exe /xo="../../../../../samples/name_domains_values/Coded_Domain_Operational_Condition_Amplifier,../../../../../samples/name_domains_values/Coded_Domain_Statuses" /xas="DOMAIN"
jmsml.exe /xc="../../../../../samples/name_domains_values/Coded_Domain_Context"
jmsml.exe /xavd="../../../../../samples/name_domains_values/Coded_Domain"

REM *** Append all coded domain values together in a single file ***

jmsml.exe /xe="../../../../../samples/name_domains_values/All_Coded_Domain_Values" /xas="DOMAIN"
jmsml.exe /xe="../../../../../samples/name_domains_values/All_Coded_Domain_Values" /xas="DOMAIN" /+

endlocal