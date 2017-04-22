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
            //textBox1.Text = textBox1.Text + wrapper.ReadString();
            //textBox1.Text = textBox1.Text + "!"+man._wrapIn.ReadString();
            //textBox1.Text = textBox1.Text + ""+man._wrapIn.ErrorString();
            //textBox1.Text = textBox1.Text + "" + man._wrapIn.ReadString();
            //  Console.WriteLine("Timer IN");



            /*
            string s = man._wrapIn.ErrorString();
            if (s != "")
            {
                textBox1.AppendText(s + Environment.NewLine);
            }
            */




            //Console.WriteLine("Timer OUT");
            //      textBox1.AppendText(man._wrapIn.ReadString());


            //   textBox1.Text = textBox1.Text + "@";
            //  Console.WriteLine("TIMER TIMER TIMER");




            /*
            byte[] buff = new byte[1024];
            int i;
            i = wrapper.GetErrorStream().Read(buff, 0, 1024);
            if (i > 0)
            {
                byte[] data = new byte[i];
                textBox1.Text = textBox1.Text + data.ToString();
            }
            wrapper.GetReadStream().Seek(0,System.IO.SeekOrigin.Begin);*/
            //i = wrapper.GetReadStream().Read(buff, 0, 1024);
            //  if (i > 0)
            /* {
                 byte[] data = new byte[i];
                 //textBox1.Text = textBox1.Text + "< >" + wrapper.GetReadStream().Position.ToString(); ///data.Length.ToString(); // BitConverter.ToString(data);// System.Text.Encoding.ASCII.GetString(data);
               //  textBox1.Text = textBox1.Text + "< >" + wrapper._proc.StandardOutput.BaseStream.Position.ToString(); ///data.Length.ToString(); // BitConverter.ToString(data);// System.Text.Encoding.ASCII.GetString(data);
               //  if (wrapper._proc.StandardOutput.BaseStream.Position > 0)
             /*  if (wrapper._readStream.Position > 0)
                 {
                     /*
                     byte[] dt = new byte[1024];
                     int j = wrapper._readStream.Read(dt, 0, 1024);
                     byte[] dd = new byte[j];
                     for (int z = 0; z < j; z++)
                     {
                         dd[z] = dt[z];
                     }
                     textBox1.Text = textBox1.Text + "!" + System.Text.Encoding.Unicode.GetString(dd);

                     */
            //  int z = (int)wrapper._proc.StandardOutput.BaseStream.Position;
            /*
              byte[] dt = new byte[1024];
              int j = wrapper._proc.StandardOutput.Read(dt, 0, 1024);
              char[] dd = new char[j];
              for(int z=0; z<j; z++)
              {
                  dd[z] = dt[z];
              }
              textBox1.Text = textBox1.Text + "!" +new string(dd); 
              */
            /* } */




            //wrapper.GetReadStream().Read(buff, 0, 10);
            //wrapper.br  br.ReadBytes(20);
            /*    }*/

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
