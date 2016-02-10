using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JointMilitarySymbologyLibrary
{
    public class LegacyHQTFFDExport : HQTFFDExport, IHQTFFDExport
    {
        // Class designed to export HQTFFD amplifiers as legacy to 2525D lookups

        private string _standard;

        public LegacyHQTFFDExport(ConfigHelper configHelper, string standard)
        {
            _configHelper = configHelper;
            _standard = standard;
        }

        string IHQTFFDExport.Headers
        {
            get { return "Name,Key" + _standard + ",MainIcon,Modifier1,Modifier2,ExtraIcon,FullFrame,GeometryType,Status,Notes"; }
        }

        string IHQTFFDExport.Line(LibraryHQTFDummy hqTFFD, LibraryHQTFDummyGraphic graphic)
        {
            string result = "";

            LibraryStandardIdentityGroup siGroup = _configHelper.Librarian.StandardIdentityGroup(graphic.StandardIdentityGroup);
            LibraryDimension dimension = _configHelper.Librarian.Dimension(graphic.DimensionID);

            result = BuildHQTFFDItemName(siGroup, dimension, hqTFFD);
            result = result + "," + BuildSIDCKey(siGroup, dimension, hqTFFD);
            result = result + "," + BuildHQTFFDCode(siGroup, dimension, hqTFFD);
            result = result + ",";
            result = result + ",";
            result = result + ",";
            result = result + ",";
            result = result + "," + "Point";
            result = result + ",";
            result = result + ",";

            return result;
        }
    }
}
