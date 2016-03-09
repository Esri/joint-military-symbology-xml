### Creating Legacy Icons using 2525D format.
1. Open these [template octagon SVG's](https://github.com/Esri/joint-military-symbology-xml/tree/master/svg/MIL_STD_2525D_Symbols) in separate views in illustrator. They are Bounding Octagon.svg, BoundingOctagon_H.svg, and BoundingOctagon_V svg.
2. Obtain your desired SVG icon from one of the retired symbol sets.
3. Open the retired SVG icon in illustrator. Explore the layers in the icon and remove any unnecessary elements of the icon (usually the frame).
4. Navigate to one of the template octagon SVG's in illustrator. Select it, copy it, and paste it into the same view as your retired icon SVG. 
5. Select the desired elements of your icon SVG (it may be easier to select from the layers panel instead of selecting directly in the view/artboard).
6. Re-size the icon SVG so that the edges of the SVG are touching the appropriate parts of the template octagon. You can either re-size the elements directly in the artboard, or you may manipulate the W and H values that are docked above the artboard.
7. When you are done re-sizing the icon SVG, navigate to the layers panel and make the template octagon invisible (click on the eye icon so that it goes away).
8. Go to steps 8 and 9 of the full frame instructions to review saving your SVG. Ignore the section regarding Friendly, Neutral, and Hostile symbols. 


### Creating Full-Frame Legacy Icons using 2525D format.

*This process was done using Adobe Illustrator CS6.*

1. Open these 4 [template SVG's](https://github.com/Esri/joint-military-symbology-xml/tree/master/svg/MIL_STD_2525D_Symbols/Frames/Template) in separate views in Illustrator. 
2. Obtain your desired SVG icon from one of the retired symbol sets.
3. Open the retired SVG icon in illustrator. Explore the layers in the icon and remove any unnecessary elements of the icon (usually the frame).
4. Go to the Unknown Template Frame in illustrator. Select it, copy it, and paste it into the same view as your retired icon SVG.
5. Select the desired elements of your icon SVG (it may be easier to select from the layers panel instead of selecting directly in the view/artboard). 
6. Re-size the icon SVG so that the edges of the SVG are touching the template frame. You can either re-size the elements directly in the view/artboard, or you may manipulate the W and H values that are docked above the artboard. 
7. When you are done re-sizing the icon SVG, navigate to the layers panel and make the template frame invisible (click on the eye icon so that it goes away). 
8. Save the new SVG using the following naming convention: 
  * C-D-FFFFFF.svg
    * Where C = coding schema letter, D = dimension code letter, FFFFFF = function code string (using dashes as needed to make six characters).
  * Examples:
    * S-F-UCI---.svg 
    * S-P-V-----.svg
  * Where there are four versions of the same icon for frame shape purposes, use the following:
    * C-D-FFFFFF_0.svg = Unknown/Pending frame shape 
    * C-D-FFFFFF_1.svg = Friend/Assumed Friend frame shape 
    * C-D-FFFFFF_2.svg = Neutral frame shape 
    * C-D-FFFFFF_3.svg = Hostile/Suspect frame shape
9. Repeat steps 4-8 for the Friendly, Neutral, and Hostile template frame. Remember, when you are creating Full-Frame icons, the size of the icons will be different depending on the frame. 
  * By default, Adobe Illustrator will change the SVG XML to inserts style/css directives that may not be useable by other SVG viewers/converters (the original DISA SVGs did not have these css styles) – to disable this option, when you save the file, select “Save As” and change the CSS Properties to “Style Attributes." This is under advanced options when saving as an SVG.
  * You have to be very careful Illustrator doesn’t use unsupported fonts or you don’t inadvertently re-use such fonts from the retired SVGs. Also Illustrator substitutes different fonts from what is selected (example Illustrator substitutes ArialMT for Arial). 
  * We need to figure out how DISA managed to change illustrator settings so that the fonts were restricted to Sans-Serif when dealing with these SVGs. 




