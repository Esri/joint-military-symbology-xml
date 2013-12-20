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
            this.text2525D = new System.Windows.Forms.TextBox();
            this.label2525D = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.buttonCtoD = new System.Windows.Forms.Button();
            this.buttonDtoC = new System.Windows.Forms.Button();
            this.pictureBoxSymbol = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSymbol)).BeginInit();
            this.SuspendLayout();
            // 
            // text2525C
            // 
            this.text2525C.Location = new System.Drawing.Point(80, 183);
            this.text2525C.MaxLength = 15;
            this.text2525C.Name = "text2525C";
            this.text2525C.Size = new System.Drawing.Size(116, 22);
            this.text2525C.TabIndex = 0;
            // 
            // label2525C
            // 
            this.label2525C.AutoSize = true;
            this.label2525C.Location = new System.Drawing.Point(28, 186);
            this.label2525C.Name = "label2525C";
            this.label2525C.Size = new System.Drawing.Size(49, 17);
            this.label2525C.TabIndex = 1;
            this.label2525C.Text = "2525C";
            // 
            // text2525D
            // 
            this.text2525D.Location = new System.Drawing.Point(341, 181);
            this.text2525D.MaxLength = 10;
            this.text2525D.Name = "text2525D";
            this.text2525D.Size = new System.Drawing.Size(118, 22);
            this.text2525D.TabIndex = 2;
            // 
            // label2525D
            // 
            this.label2525D.AutoSize = true;
            this.label2525D.Location = new System.Drawing.Point(285, 186);
            this.label2525D.Name = "label2525D";
            this.label2525D.Size = new System.Drawing.Size(50, 17);
            this.label2525D.TabIndex = 3;
            this.label2525D.Text = "2525D";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(341, 209);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(118, 22);
            this.textBox1.TabIndex = 4;
            // 
            // buttonCtoD
            // 
            this.buttonCtoD.Image = ((System.Drawing.Image)(resources.GetObject("buttonCtoD.Image")));
            this.buttonCtoD.Location = new System.Drawing.Point(247, 176);
            this.buttonCtoD.Name = "buttonCtoD";
            this.buttonCtoD.Size = new System.Drawing.Size(32, 32);
            this.buttonCtoD.TabIndex = 5;
            this.buttonCtoD.UseVisualStyleBackColor = true;
            this.buttonCtoD.Click += new System.EventHandler(this.buttonCtoD_Click);
            // 
            // buttonDtoC
            // 
            this.buttonDtoC.Image = ((System.Drawing.Image)(resources.GetObject("buttonDtoC.Image")));
            this.buttonDtoC.Location = new System.Drawing.Point(209, 176);
            this.buttonDtoC.Name = "buttonDtoC";
            this.buttonDtoC.Size = new System.Drawing.Size(32, 32);
            this.buttonDtoC.TabIndex = 6;
            this.buttonDtoC.UseVisualStyleBackColor = true;
            // 
            // pictureBoxSymbol
            // 
            this.pictureBoxSymbol.Location = new System.Drawing.Point(142, 12);
            this.pictureBoxSymbol.Name = "pictureBoxSymbol";
            this.pictureBoxSymbol.Size = new System.Drawing.Size(203, 158);
            this.pictureBoxSymbol.TabIndex = 7;
            this.pictureBoxSymbol.TabStop = false;
            // 
            // FormSIDCConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 246);
            this.Controls.Add(this.pictureBoxSymbol);
            this.Controls.Add(this.buttonDtoC);
            this.Controls.Add(this.buttonCtoD);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2525D);
            this.Controls.Add(this.text2525D);
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
        private System.Windows.Forms.TextBox text2525D;
        private System.Windows.Forms.Label label2525D;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button buttonCtoD;
        private System.Windows.Forms.Button buttonDtoC;
        private System.Windows.Forms.PictureBox pictureBoxSymbol;
    }
}

