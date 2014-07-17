/* 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Svg;

namespace Test
{
    // SVG Class and sample usage provide from:
    // https://github.com/csmoore/MyMiscellanea/blob/master/CSharp/Testing2525D/MilSymbolPicker/SVGSymbol.cs
    public class SvgSymbol
    {
        public static Size ImageSize { get; set; }

        public static Bitmap GetBitmap(List<string> graphicLayers)
        {
            if (graphicLayers.Count == 0)
                return null;

            Bitmap bitmap = new Bitmap(ImageSize.Width, ImageSize.Height);

            foreach (string graphicLayer in graphicLayers)
            {
                if (!System.IO.File.Exists(graphicLayer))
                {
                    System.Diagnostics.Trace.WriteLine("Could not find SVG layer: "
                        + graphicLayer);
                    continue;
                }

                System.Diagnostics.Trace.WriteLine("Drawing SVG layer: "
                        + graphicLayer);

                SvgDocument document = GetSvgDoc(graphicLayer);

                document.Draw(bitmap);
            }

            return bitmap;
        }

        public static SvgDocument GetSvgDoc(string svgFile)
        {
            SvgDocument document = SvgDocument.Open(svgFile);

            return Resize(document);
        }

        private static SvgDocument Resize(SvgDocument document)
        {
            if (document.Height > ImageSize.Height)
            {
                document.Width = (int)(((double)document.Width / (double)document.Height) * (double)ImageSize.Height);
                document.Height = ImageSize.Height;
            }
            return document;
        }
    }
}
