namespace Videofy
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDone = new System.Windows.Forms.Label();
            this.lblIsWorking = new System.Windows.Forms.Label();
            this.tbURL = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.cbResolution = new System.Windows.Forms.ComboBox();
            this.cbDensity = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbCellCount = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbPreset = new System.Windows.Forms.ComboBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.rbBitrate = new System.Windows.Forms.RadioButton();
            this.rbQuality = new System.Windows.Forms.RadioButton();
            this.tbQuality = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, -1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Data >> MP4";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(659, 5);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(642, 181);
            this.textBox1.TabIndex = 1;
            this.textBox1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 26);
            this.button2.TabIndex = 1;
            this.button2.Text = "MP4 >> Data";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(420, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Value shift:";
            this.label1.Visible = false;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(476, 49);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(13, 13);
            this.lblError.TabIndex = 4;
            this.lblError.Text = "0";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Any file | *.*";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.Filter = "MP4 files | *.mp4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(100, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Processed:";
            // 
            // lblDone
            // 
            this.lblDone.AutoSize = true;
            this.lblDone.Location = new System.Drawing.Point(174, 1);
            this.lblDone.Name = "lblDone";
            this.lblDone.Size = new System.Drawing.Size(30, 13);
            this.lblDone.TabIndex = 7;
            this.lblDone.Text = "0.0%";
            // 
            // lblIsWorking
            // 
            this.lblIsWorking.AutoSize = true;
            this.lblIsWorking.Enabled = false;
            this.lblIsWorking.Location = new System.Drawing.Point(420, 62);
            this.lblIsWorking.Name = "lblIsWorking";
            this.lblIsWorking.Size = new System.Drawing.Size(38, 13);
            this.lblIsWorking.TabIndex = 8;
            this.lblIsWorking.Text = "DONE";
            this.lblIsWorking.Visible = false;
            // 
            // tbURL
            // 
            this.tbURL.Location = new System.Drawing.Point(103, 63);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(210, 20);
            this.tbURL.TabIndex = 2;
            this.tbURL.Text = "Insert URL here";
            this.tbURL.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbURL_MouseClick);
            this.tbURL.TextChanged += new System.EventHandler(this.tbURL_TextChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1, 59);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(96, 26);
            this.button3.TabIndex = 3;
            this.button3.Text = "Youtube >> Data";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "Go into folder and press Save";
            this.saveFileDialog1.Filter = "Any folder | fоlder";
            this.saveFileDialog1.OverwritePrompt = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(330, -1);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(19, 22);
            this.button4.TabIndex = 4;
            this.button4.Text = "?";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Image = global::Videofy.Properties.Resources.patreon;
            this.pictureBox2.Location = new System.Drawing.Point(355, 1);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(88, 45);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = global::Videofy.Properties.Resources.patreon;
            this.pictureBox1.Location = new System.Drawing.Point(12, 232);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 256);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // button5
            // 
            this.button5.Enabled = false;
            this.button5.Location = new System.Drawing.Point(398, 185);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 14;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.Location = new System.Drawing.Point(332, 156);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 15;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(332, 269);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 16;
            this.button7.Text = "Init file";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Location = new System.Drawing.Point(341, 309);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 17;
            this.button8.Text = "Close file";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Visible = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // cbResolution
            // 
            this.cbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbResolution.FormattingEnabled = true;
            this.cbResolution.Location = new System.Drawing.Point(15, 115);
            this.cbResolution.Name = "cbResolution";
            this.cbResolution.Size = new System.Drawing.Size(54, 21);
            this.cbResolution.TabIndex = 18;
            this.cbResolution.SelectedIndexChanged += new System.EventHandler(this.cbResolution_SelectedIndexChanged);
            // 
            // cbDensity
            // 
            this.cbDensity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDensity.FormattingEnabled = true;
            this.cbDensity.Location = new System.Drawing.Point(81, 115);
            this.cbDensity.Name = "cbDensity";
            this.cbDensity.Size = new System.Drawing.Size(48, 21);
            this.cbDensity.TabIndex = 19;
            this.cbDensity.SelectedIndexChanged += new System.EventHandler(this.cbDestiny_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Resolution";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(78, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Density";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(135, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Cell Count";
            // 
            // cbCellCount
            // 
            this.cbCellCount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCellCount.FormattingEnabled = true;
            this.cbCellCount.Location = new System.Drawing.Point(135, 115);
            this.cbCellCount.Name = "cbCellCount";
            this.cbCellCount.Size = new System.Drawing.Size(97, 21);
            this.cbCellCount.TabIndex = 23;
            this.cbCellCount.SelectedIndexChanged += new System.EventHandler(this.cbCellCount_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(235, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Encoding Preset";
            // 
            // cbPreset
            // 
            this.cbPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreset.FormattingEnabled = true;
            this.cbPreset.Location = new System.Drawing.Point(238, 115);
            this.cbPreset.Name = "cbPreset";
            this.cbPreset.Size = new System.Drawing.Size(97, 21);
            this.cbPreset.TabIndex = 25;
            this.cbPreset.SelectedIndexChanged += new System.EventHandler(this.cbPreset_SelectedIndexChanged);
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(439, -1);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 26;
            this.button9.Text = "11111";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Visible = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Enabled = false;
            this.button10.Location = new System.Drawing.Point(439, 23);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 23);
            this.button10.TabIndex = 27;
            this.button10.Text = "222222";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Visible = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // rbBitrate
            // 
            this.rbBitrate.AutoSize = true;
            this.rbBitrate.Location = new System.Drawing.Point(341, 82);
            this.rbBitrate.Name = "rbBitrate";
            this.rbBitrate.Size = new System.Drawing.Size(98, 17);
            this.rbBitrate.TabIndex = 28;
            this.rbBitrate.TabStop = true;
            this.rbBitrate.Text = "Average Bitrate";
            this.rbBitrate.UseVisualStyleBackColor = true;
            this.rbBitrate.CheckedChanged += new System.EventHandler(this.rbBitrate_CheckedChanged);
            // 
            // rbQuality
            // 
            this.rbQuality.AutoSize = true;
            this.rbQuality.Location = new System.Drawing.Point(341, 99);
            this.rbQuality.Name = "rbQuality";
            this.rbQuality.Size = new System.Drawing.Size(102, 17);
            this.rbQuality.TabIndex = 29;
            this.rbQuality.TabStop = true;
            this.rbQuality.Text = "Constant Quality";
            this.rbQuality.UseVisualStyleBackColor = true;
            this.rbQuality.CheckedChanged += new System.EventHandler(this.rbQuality_CheckedChanged);
            // 
            // tbQuality
            // 
            this.tbQuality.Location = new System.Drawing.Point(341, 116);
            this.tbQuality.Name = "tbQuality";
            this.tbQuality.Size = new System.Drawing.Size(102, 20);
            this.tbQuality.TabIndex = 30;
            this.tbQuality.Text = "10000000";
            this.tbQuality.TextChanged += new System.EventHandler(this.tbQuality_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 137);
            this.Controls.Add(this.tbQuality);
            this.Controls.Add(this.rbQuality);
            this.Controls.Add(this.rbBitrate);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.cbPreset);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbCellCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbDensity);
            this.Controls.Add(this.cbResolution);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.tbURL);
            this.Controls.Add(this.lblIsWorking);
            this.Controls.Add(this.lblDone);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Videofy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDone;
        private System.Windows.Forms.Label lblIsWorking;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ComboBox cbResolution;
        private System.Windows.Forms.ComboBox cbDensity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbCellCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbPreset;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.RadioButton rbBitrate;
        private System.Windows.Forms.RadioButton rbQuality;
        private System.Windows.Forms.TextBox tbQuality;
    }
}

