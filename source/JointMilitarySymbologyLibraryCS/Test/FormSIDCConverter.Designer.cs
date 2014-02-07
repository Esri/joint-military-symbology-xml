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
            this.text2525C = new System.Windows.Forms.TextBox();
            this.label2525C = new System.Windows.Forms.Label();
            this.text2525D_1 = new System.Windows.Forms.TextBox();
            this.label2525D = new System.Windows.Forms.Label();
            this.text2525D_2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // text2525C
            // 
            this.text2525C.Enabled = false;
            this.text2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525C.Location = new System.Drawing.Point(102, 47);
            this.text2525C.MaxLength = 15;
            this.text2525C.Name = "text2525C";
            this.text2525C.Size = new System.Drawing.Size(184, 30);
            this.text2525C.TabIndex = 0;
            // 
            // label2525C
            // 
            this.label2525C.AutoSize = true;
            this.label2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525C.Location = new System.Drawing.Point(19, 50);
            this.label2525C.Name = "label2525C";
            this.label2525C.Size = new System.Drawing.Size(77, 25);
            this.label2525C.TabIndex = 1;
            this.label2525C.Text = "2525C:";
            // 
            // text2525D_1
            // 
            this.text2525D_1.Enabled = false;
            this.text2525D_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_1.Location = new System.Drawing.Point(395, 26);
            this.text2525D_1.MaxLength = 10;
            this.text2525D_1.Name = "text2525D_1";
            this.text2525D_1.Size = new System.Drawing.Size(262, 30);
            this.text2525D_1.TabIndex = 2;
            // 
            // label2525D
            // 
            this.label2525D.AutoSize = true;
            this.label2525D.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525D.Location = new System.Drawing.Point(313, 50);
            this.label2525D.Name = "label2525D";
            this.label2525D.Size = new System.Drawing.Size(76, 25);
            this.label2525D.TabIndex = 3;
            this.label2525D.Text = "2525D:";
            // 
            // text2525D_2
            // 
            this.text2525D_2.Enabled = false;
            this.text2525D_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_2.Location = new System.Drawing.Point(395, 72);
            this.text2525D_2.Name = "text2525D_2";
            this.text2525D_2.Size = new System.Drawing.Size(262, 30);
            this.text2525D_2.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 108);
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
            this.listBox1.Location = new System.Drawing.Point(24, 162);
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
            this.listBox2.Location = new System.Drawing.Point(318, 162);
            this.listBox2.Name = "listBox2";
            this.listBox2.ScrollAlwaysVisible = true;
            this.listBox2.Size = new System.Drawing.Size(339, 94);
            this.listBox2.TabIndex = 9;
            this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 142);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Choose a 2525C Symbol...";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(315, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(191, 17);
            this.label3.TabIndex = 11;
            this.label3.Text = "...or choose a 2525D Symbol";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 281);
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
            // FormSIDCConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 303);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.text2525D_2);
            this.Controls.Add(this.label2525D);
            this.Controls.Add(this.text2525D_1);
            this.Controls.Add(this.label2525C);
            this.Controls.Add(this.text2525C);
            this.Name = "FormSIDCConverter";
            this.Text = "SIDC Converter";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text2525C;
        private System.Windows.Forms.Label label2525C;
        private System.Windows.Forms.TextBox text2525D_1;
        private System.Windows.Forms.Label label2525D;
        private System.Windows.Forms.TextBox text2525D_2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

