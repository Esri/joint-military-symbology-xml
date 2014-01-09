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
            this.text2525C = new System.Windows.Forms.TextBox();
            this.label2525C = new System.Windows.Forms.Label();
            this.text2525D_1 = new System.Windows.Forms.TextBox();
            this.label2525D = new System.Windows.Forms.Label();
            this.text2525D_2 = new System.Windows.Forms.TextBox();
            this.buttonCtoD = new System.Windows.Forms.Button();
            this.buttonDtoC = new System.Windows.Forms.Button();
            this.pictureBoxSymbol = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSymbol)).BeginInit();
            this.SuspendLayout();
            // 
            // text2525C
            // 
            this.text2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525C.Location = new System.Drawing.Point(96, 202);
            this.text2525C.MaxLength = 15;
            this.text2525C.Name = "text2525C";
            this.text2525C.Size = new System.Drawing.Size(165, 30);
            this.text2525C.TabIndex = 0;
            // 
            // label2525C
            // 
            this.label2525C.AutoSize = true;
            this.label2525C.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525C.Location = new System.Drawing.Point(19, 205);
            this.label2525C.Name = "label2525C";
            this.label2525C.Size = new System.Drawing.Size(77, 25);
            this.label2525C.TabIndex = 1;
            this.label2525C.Text = "2525C:";
            // 
            // text2525D_1
            // 
            this.text2525D_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_1.Location = new System.Drawing.Point(437, 180);
            this.text2525D_1.MaxLength = 10;
            this.text2525D_1.Name = "text2525D_1";
            this.text2525D_1.Size = new System.Drawing.Size(136, 30);
            this.text2525D_1.TabIndex = 2;
            // 
            // label2525D
            // 
            this.label2525D.AutoSize = true;
            this.label2525D.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2525D.Location = new System.Drawing.Point(365, 205);
            this.label2525D.Name = "label2525D";
            this.label2525D.Size = new System.Drawing.Size(76, 25);
            this.label2525D.TabIndex = 3;
            this.label2525D.Text = "2525D:";
            // 
            // text2525D_2
            // 
            this.text2525D_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.text2525D_2.Location = new System.Drawing.Point(437, 226);
            this.text2525D_2.Name = "text2525D_2";
            this.text2525D_2.Size = new System.Drawing.Size(136, 30);
            this.text2525D_2.TabIndex = 4;
            // 
            // buttonCtoD
            // 
            this.buttonCtoD.Image = ((System.Drawing.Image)(resources.GetObject("buttonCtoD.Image")));
            this.buttonCtoD.Location = new System.Drawing.Point(316, 196);
            this.buttonCtoD.Name = "buttonCtoD";
            this.buttonCtoD.Size = new System.Drawing.Size(45, 45);
            this.buttonCtoD.TabIndex = 5;
            this.buttonCtoD.UseVisualStyleBackColor = true;
            this.buttonCtoD.Click += new System.EventHandler(this.buttonCtoD_Click);
            // 
            // buttonDtoC
            // 
            this.buttonDtoC.Image = ((System.Drawing.Image)(resources.GetObject("buttonDtoC.Image")));
            this.buttonDtoC.Location = new System.Drawing.Point(273, 196);
            this.buttonDtoC.Name = "buttonDtoC";
            this.buttonDtoC.Size = new System.Drawing.Size(45, 45);
            this.buttonDtoC.TabIndex = 6;
            this.buttonDtoC.UseVisualStyleBackColor = true;
            this.buttonDtoC.Click += new System.EventHandler(this.buttonDtoC_Click);
            // 
            // pictureBoxSymbol
            // 
            this.pictureBoxSymbol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxSymbol.Location = new System.Drawing.Point(197, 12);
            this.pictureBoxSymbol.Name = "pictureBoxSymbol";
            this.pictureBoxSymbol.Size = new System.Drawing.Size(165, 165);
            this.pictureBoxSymbol.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxSymbol.TabIndex = 7;
            this.pictureBoxSymbol.TabStop = false;
            // 
            // FormSIDCConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 424);
            this.Controls.Add(this.pictureBoxSymbol);
            this.Controls.Add(this.buttonDtoC);
            this.Controls.Add(this.buttonCtoD);
            this.Controls.Add(this.text2525D_2);
            this.Controls.Add(this.label2525D);
            this.Controls.Add(this.text2525D_1);
            this.Controls.Add(this.label2525C);
            this.Controls.Add(this.text2525C);
            this.Name = "FormSIDCConverter";
            this.Text = "SIDC Converter";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSymbol)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text2525C;
        private System.Windows.Forms.Label label2525C;
        private System.Windows.Forms.TextBox text2525D_1;
        private System.Windows.Forms.Label label2525D;
        private System.Windows.Forms.TextBox text2525D_2;
        private System.Windows.Forms.Button buttonCtoD;
        private System.Windows.Forms.Button buttonDtoC;
        private System.Windows.Forms.PictureBox pictureBoxSymbol;
    }
}

