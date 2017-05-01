using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Videofy.Tools;
using Videofy.Main;
using Videofy.Settings;


namespace Videofy
{
    public partial class Form1 : Form
    {
        private Wrapper wrapper;

        private Converter man;
        private OptionsManager options;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) { return; }
            man = new Converter(openFileDialog1.FileName, true);
            man.Pack();

            try
            {
               
            }
            catch (Exception q)
            {

                textBox1.Text = q.ToString();
            }
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (man.workDone)
            {
                lblError.Text = man.error.ToString("0.00");
                //   lblIsWorking.Text = "DONE";
            }
            else
            {
                // lblIsWorking.Text = "WORKING...";
            }
            lblIsWorking.Text = man.state.ToString();
            lblDone.Text = man.workMeter.GetDonePercent();           

        }

        private void button2_Click(object sender, EventArgs e)
        {           
            DialogResult dr = openFileDialog2.ShowDialog();
            if (dr != DialogResult.OK) { return; }
            var s = openFileDialog2.FileName;
            man = new Converter(openFileDialog2.FileName, false);
            man.Unpack();
            timer1.Enabled = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
      
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) { return; }
            man = new Converter(saveFileDialog1.FileName, tbURL.Text);
            timer1.Enabled = true;
            //var dl = new Converter(saveFileDialog1.FileName, tbURL.Text);
            man.Unpack();
        }

        private void tbURL_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbURL_MouseClick(object sender, MouseEventArgs e)
        {
            tbURL.SelectAll();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var form2 = new Form2();
            /*
            form2.Left = this.Left;
            form2.Top = this.Top;
            */
            form2.Visible = false;
            form2.Show();
            form2.Location = this.Location;
            form2.Visible = true;
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.patreon.com/filarius");
        }
        private void pictureBox1_Click_2(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.patreon.com/filarius");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.patreon.com/filarius");
        }

        private void button5_Click(object sender, EventArgs e)
        {
           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (options == null) return;
            options.SetResolution(((ComboBox)sender).SelectedIndex);
        }

        private void cbDestiny_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (options == null) return;
            options.SetDensity(((ComboBox)sender).SelectedIndex);
        }

        private void cbInputFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (options == null) return;
            options.SetPxlFmtIn(((ComboBox)sender).SelectedIndex);
        }

        private void cbOutputFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (options == null) return;
            options.SetPxlFmtOut(((ComboBox)sender).SelectedIndex);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Text = Application.ProductName + " "+ Application.ProductVersion;
            if (options == null)
            {
                options = new OptionsManager(cbResolution, cbDensity, cbInputFormat, cbOutputFormat);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) { return; }
            var man = new Chain.ChainManager();
            man.EncodeFile(openFileDialog1.FileName);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) { return; }
            var man = new Chain.ChainManager();
            man.DecodeFile(saveFileDialog1.FileName);

        }
    }
}
