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
        private Librarian _librarian;
        private Symbol _symbol;

        public FormSIDCConverter()
        {
            InitializeComponent();

            _librarian = new Librarian();
            _symbol = _librarian.MakeSymbol(new SIDC());

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
    
        // Update controls on the form

        private void updateC()
        {
            text2525C.Text = _symbol.LegacySIDC;
        }

        private void updateD()
        {
            text2525D_1.Text = _symbol.SIDC.PartAString;
            text2525D_2.Text = _symbol.SIDC.PartBString;
        }

        private void updateControls()
        {
            updateC();
            updateD();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = listBox1.SelectedItem.ToString();

            string[] l = s.Split('\t');
            
            _symbol.LegacySIDC = l[0];

            updateControls();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = listBox2.SelectedItem.ToString();

            string[] l = s.Split('\t');
            string[] ll = l[0].Split(',');

            SIDC sid = _symbol.SIDC;

            sid.PartAString = ll[0];
            sid.PartBString = ll[1];

            _symbol.SIDC = sid;

            updateControls();
        }
    }
}
