using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;
using JointMilitarySymbologyLibrary;

namespace Test
{
    public partial class FormSIDCConverter : Form
    {
        private librarian _librarian;
        private symbol _symbol;

        public FormSIDCConverter()
        {
            InitializeComponent();

            _librarian = new librarian();
            _symbol = _librarian.makeSymbol(new sidc());

            updateControls();
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

        private void buttonCtoD_Click(object sender, EventArgs e)
        {
            _symbol.legacySIDC = text2525C.Text;

            updateD();
            updateImage();
        }

        private void buttonDtoC_Click(object sender, EventArgs e)
        {
            sidc sidc = _symbol.sidc;

            sidc.partAString = text2525D_1.Text;
            sidc.partBString = text2525D_2.Text;

            _symbol.sidc = sidc;

            updateC();
            updateImage();
        }

        // Update controls on the form

        private void updateC()
        {
            text2525C.Text = _symbol.legacySIDC;
        }

        private void updateD()
        {
            text2525D_1.Text = _symbol.sidc.partAString;
            text2525D_2.Text = _symbol.sidc.partBString;
        }

        private void updateControls()
        {
            updateC();
            updateD();
            updateImage();
        }

        private void updateImage()
        {
            if (_symbol.image != null)
            {
                pictureBoxSymbol.Image = _symbol.image;

                _symbol.saveImage("C:\\Users\\andy750\\Documents\\jmsml\\save.png");
            }
        }
    }
}
