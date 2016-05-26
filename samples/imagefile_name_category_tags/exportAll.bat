setlocal
cd ..\..\source\JointMilitarySymbologyLibraryCS\jmsml\bin\Debug
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Activities-Source-Icons" /s="Activities" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Air-Source-Icons" /s="^Air|Air Missile$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Cyberspace-Source-Icons" /s="^Cyberspace$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Land-Unit-Source-Icons" /s="^Land Units|Land Civilian$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Land-Equipment-Source-Icons" /s="^Land Equipment$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Land-Installation-Source-Icons" /s="^Land Installation$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Sea-Surface-Source-Icons" /s="^Sea Surface$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Sea-Subsurface-Source-Icons" /s="^Sea Subsurface|Mine Warfare$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Space-Source-Icons" /s="^Space|Space Missile$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Sigint-Source-Icons" /s="Signals Intelligence" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-ControlMeasures-Source-Points" /p /s="^Control Measure$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-ControlMeasures-Source-Lines" /l /s="^Control Measure$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-ControlMeasures-Source-Areas" /a /s="^Control Measure$" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Weather-Source-Points" /p /s="Atmospheric|Oceanographic|Meteorological Space" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Weather-Source-Lines" /l /s="Atmospheric|Oceanographic|Meteorological Space" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-Weather-Source-Areas" /a /s="Atmospheric|Oceanographic|Meteorological Space" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xf="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xa="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xh="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xo="../../../../../samples/imagefile_name_category_tags/Military-Frame-And-Amplifier-Source-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy

REM *** Append all the image file information together in a single file (2525C and 2525D) ***

jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xf="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xa="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xh="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xo="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xleC="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /size="64"
jmsml.exe /xleBc2="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /size="64"
jmsml.exe /xlfBc2="../../../../../samples/imagefile_name_category_tags/Military-All-Icons" /+ /size="64"

REM *** Append all the image file information together in a single file (2525D-only) ***

jmsml.exe /xe="../../../../../samples/imagefile_name_category_tags/Military-All-2525D-Only-Icons" /+ /xas="IMAGE" /size="64" /-source
jmsml.exe /xf="../../../../../samples/imagefile_name_category_tags/Military-All-2525D-Only-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xa="../../../../../samples/imagefile_name_category_tags/Military-All-2525D-Only-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xh="../../../../../samples/imagefile_name_category_tags/Military-All-2525D-Only-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy
jmsml.exe /xo="../../../../../samples/imagefile_name_category_tags/Military-All-2525D-Only-Icons" /+ /xas="IMAGE" /size="64" /-source /-legacy

endlocal