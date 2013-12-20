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
        public FormSIDCConverter()
        {
            InitializeComponent();

            librarian myLibrarian = new librarian();

            library lib = myLibrarian.library;

            var x = lib.uses[1].useCode;
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

        }
    }
}
