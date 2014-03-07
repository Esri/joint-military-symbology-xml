/* Copyright 2014 Esri
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace Test
{
    partial class FormSIDCConverter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSIDCConverter));
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TagsLabel = new System.Windows.Forms.TextBox();
            this.GeoLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.text2525D_2 = new System.Windows.Forms.TextBox();
            this.label2525D = new System.Windows.Forms.Label();
            this.text2525D_1 = new System.Windows.Forms.TextBox();
            this.label2525C = new System.Windows.Forms.Label();
            this.text2525C = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLabel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRemarks = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colX = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLength = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label8 = new System.Windows.Forms.Label();
            this.listView2 = new System.Windows.Forms.ListView();
            this.colID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnchorPoints = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOrientation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSizeShape = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 476);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Try the following...";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Items.AddRange(new object[] {
            "YXHTXXX--------\tInvalid Symbol",
            "SFPPV----------\tCrewed Space Vehicle",
            "SFPPS----------\tSpace Satellite",
            "SHPAT----------\tSpace Station",
            "SNAPMFKB-------\tBoom-Only Tanker",
            "SUAPMFCL-------\tLight Cargo Airlift",
            "SPSPCLLLSU-----\tLittoral Combat, SUW",
            "SASACUS--------\tASW USV",
            "SFAPMFQRZ------\tRPV ES",
            "SFAP-----------\tAir"});
            this.listBox1.Location = new System.Drawing.Point(24, 518);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(256, 94);
            this.listBox1.TabIndex = 8;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // listBox2
            // 
            this.listBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 15;
            this.listBox2.Items.AddRange(new object[] {
            "1000600000,1010101010\tAn Invalid Symbol",
            "1000980000,1000000000\tInvalid Data Symbol",
            "1000980000,1100000000\tRetired Data Symbol",
            "1006050000,1208000000\tMiniaturized Civilian Satellite",
            "1004050000,1119000000\tSpace Launch Vehicle",
            "1014010000,1101040000\tFighter/Bomber (Ex Neutral)",
            "1005010000,1102000701\tHeavy Utility Helicopter",
            "1001011000,1204000000\tCivilian, Lighter than Air",
            "1003301000,1204020000\tMinesweeper",
            "1002300000,1401040000\tCivilian Merchant, RORO"});
            this.listBox2.Location = new System.Drawing.Point(318, 518);
            this.listBox2.Name = "listBox2";
            this.listBox2.ScrollAlwaysVisible = true;
            this.listBox2.Size = new System.Drawing.Size(339, 94);
            this.listBox2.TabIndex = 9;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 498);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Choose a 2525C Symbol...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 498);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(191, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "...or choose a 2525D Symbol";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 697);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(682, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(24, 624);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(633, 54);
            this.label4.TabIndex = 13;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.listView2);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.TagsLabel);
            this.groupBox1.Controls.Add(this.GeoLabel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.text2525D_2);
            this.groupBox1.Controls.Add(this.label2525D);
            this.groupBox1.Controls.Add(this.text2525D_1);
            this.groupBox1.Controls.Add(this.label2525C);
            this.groupBox1.Controls.Add(this.text2525C);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(24, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(633, 451);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            // 
            // TagsLabel
            // 
            this.TagsLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TagsLabel.Location = new System.Drawing.Point(92, 137);
            this.TagsLabel.Multiline = true;
            this.TagsLabel.Name = "TagsLabel";
            this.TagsLabel.ReadOnly = true;
            this.TagsLabel.Size = new System.Drawing.Size(525, 50);
            this.TagsLabel.TabIndex = 13;
            // 
            // GeoLabel
            // 
            this.GeoLabel.AutoSize = true;
            this.GeoLabel.Location = new System.Drawing.Point(89, 108);
            this.GeoLabel.Name = "GeoLabel";
            this.GeoLabel.Size = new System.Drawing.Size(0, 17);
            this.GeoLabel.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Tags:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Geometry:";
            // 
            // text2525D_2
            // 
            this.text2525D_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_2.Location = new System.Drawing.Point(373, 75);
            this.text2525D_2.Name = "text2525D_2";
            this.text2525D_2.Size = new System.Drawing.Size(244, 30);
            this.text2525D_2.TabIndex = 9;
            this.text2525D_2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text2525D_2_KeyPress);
            // 
            // label2525D
            // 
            this.label2525D.AutoSize = true;
            this.label2525D.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525D.Location = new System.Drawing.Point(291, 53);
            this.label2525D.Name = "label2525D";
            this.label2525D.Size = new System.Drawing.Size(76, 25);
            this.label2525D.TabIndex = 8;
            this.label2525D.Text = "2525D:";
            // 
            // text2525D_1
            // 
            this.text2525D_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_1.Location = new System.Drawing.Point(373, 29);
            this.text2525D_1.MaxLength = 10;
            this.text2525D_1.Name = "text2525D_1";
            this.text2525D_1.Size = new System.Drawing.Size(244, 30);
            this.text2525D_1.TabIndex = 7;
            this.text2525D_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text2525D_1_KeyPress);
            // 
            // label2525C
            // 
            this.label2525C.AutoSize = true;
            this.label2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525C.Location = new System.Drawing.Point(6, 50);
            this.label2525C.Name = "label2525C";
            this.label2525C.Size = new System.Drawing.Size(77, 25);
            this.label2525C.TabIndex = 6;
            this.label2525C.Text = "2525C:";
            // 
            // text2525C
            // 
            this.text2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525C.Location = new System.Drawing.Point(84, 48);
            this.text2525C.MaxLength = 15;
            this.text2525C.Name = "text2525C";
            this.text2525C.Size = new System.Drawing.Size(184, 30);
            this.text2525C.TabIndex = 5;
            this.text2525C.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.text2525C_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 190);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 17);
            this.label7.TabIndex = 18;
            this.label7.Text = "Amplifiers/Labels:";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colLabel,
            this.colDescription,
            this.colRemarks,
            this.colX,
            this.colY,
            this.colType,
            this.colLength});
            this.listView1.Location = new System.Drawing.Point(11, 210);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(606, 105);
            this.listView1.TabIndex = 17;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 106;
            // 
            // colLabel
            // 
            this.colLabel.Text = "Label";
            this.colLabel.Width = 100;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            // 
            // colRemarks
            // 
            this.colRemarks.Text = "Remarks";
            // 
            // colX
            // 
            this.colX.Text = "X";
            // 
            // colY
            // 
            this.colY.Text = "Y";
            // 
            // colType
            // 
            this.colType.Text = "Type";
            // 
            // colLength
            // 
            this.colLength.Text = "Length";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 330);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 17);
            this.label8.TabIndex = 19;
            this.label8.Text = "Drawing Rule:";
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colID,
            this.colAnchorPoints,
            this.colSizeShape,
            this.colOrientation});
            this.listView2.Location = new System.Drawing.Point(11, 350);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(606, 69);
            this.listView2.TabIndex = 20;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // colID
            // 
            this.colID.Text = "Name";
            // 
            // colAnchorPoints
            // 
            this.colAnchorPoints.Text = "Anchor Points";
            this.colAnchorPoints.Width = 130;
            // 
            // colOrientation
            // 
            this.colOrientation.Text = "Orientation";
            this.colOrientation.Width = 245;
            // 
            // colSizeShape
            // 
            this.colSizeShape.Text = "Size/Shape";
            this.colSizeShape.Width = 154;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 431);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(97, 17);
            this.label9.TabIndex = 21;
            this.label9.Text = "Drawing Note:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(108, 431);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 17);
            this.label10.TabIndex = 22;
            // 
            // FormSIDCConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 719);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Name = "FormSIDCConverter";
            this.Text = "Joint Military Symbology XML Demo";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label GeoLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox text2525D_2;
        private System.Windows.Forms.Label label2525D;
        private System.Windows.Forms.TextBox text2525D_1;
        private System.Windows.Forms.Label label2525C;
        private System.Windows.Forms.TextBox text2525C;
        private System.Windows.Forms.TextBox TagsLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colLabel;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colRemarks;
        private System.Windows.Forms.ColumnHeader colX;
        private System.Windows.Forms.ColumnHeader colY;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colLength;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader colID;
        private System.Windows.Forms.ColumnHeader colAnchorPoints;
        private System.Windows.Forms.ColumnHeader colOrientation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ColumnHeader colSizeShape;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
    }
}

