using System;
using System.Globalization;
using System.Net;
using System.Windows.Forms;
using Gecko;

namespace EzanAlarmi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Xpcom.Initialize("Firefox");
            comboBox1.SelectedItem = System.IO.File.ReadAllText("sehir.txt");
        }
        bool okundu = false;
        ListViewItem lvi;
        int i = 1;
        bool ses = true;
        private void button1_Click(object sender, EventArgs e)
        {
            stop = false;
            label2.Text = "yükleniyor..";
            geckoWebBrowser1.Navigate("https://www.sabah.com.tr/"+((string)comboBox1.SelectedItem)+"-namaz-vakitleri");        
            System.IO.File.WriteAllText("sehir.txt", ((string)comboBox1.SelectedItem));            
        }
        private void saatleriYukle()
        {
            try
            {
                listView1.Items.Clear();
                var h = geckoWebBrowser1.Document.GetElementsByTagName("tr");
                for (int y = 1; y < h.Length; y++)
                {
                    lvi = new ListViewItem("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("");
                    for (int g = 0; g < h[y].ChildNodes.Length; g++)
                    {
                        if (!string.IsNullOrWhiteSpace(h[y].ChildNodes[g].TextContent))
                        {
                            if (h[y].ChildNodes[g].TextContent.Contains("."))
                            {
                                lvi.Text = h[y].ChildNodes[g].TextContent;
                            }
                            else
                            {
                                try
                                {
                                    lvi.SubItems[i].Text = h[y].ChildNodes[g].TextContent;
                                    i += 1;
                                }
                                catch (Exception) { }
                            }

                        }
                    }
                    i = 1;
                    listView1.Items.Add(lvi);
                }               
            }
            catch (Exception) { }
            listView1.Items[0].Selected = true;
            listView1.Select();
        }
        private void Kontrolieren()
        {
            try
            {
                CultureInfo tr = new CultureInfo("tr-TR");
                var suankiZaman = DateTime.ParseExact(DateTime.Now.ToString("HH:mm"), "HH:mm", tr);
                for (int h = 1; h < listView1.Items[0].SubItems.Count; h++)
                {
                    if ((DateTime.ParseExact(listView1.Items[0].SubItems[h].Text, "HH:mm", tr) - suankiZaman).ToString() == "00:"+textBox1.Text+":00")
                    {
                        if (okundu == false)
                        {
                            okundu = true;
                            new Alarm(listView1.Columns[h].Text, listView1.Items[0].SubItems[h].Text, ses).Show();                            
                        }
                    }
                    if (listView1.Items[0].SubItems[h].Text == DateTime.Now.ToString("HH:mm"))
                    {
                        okundu = false;
                    }
                }
            }
            catch (Exception) { }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Kontrolieren();
        }
        bool stop = false;
        private void geckoWebBrowser1_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            if (stop == false)
            {
                stop = true;
                saatleriYukle();
                label2.Text = "yüklendi.";
                timer1.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            okundu = false;
            if(((int)numericUpDown1.Value < 10)){
                textBox1.Text = "0"+numericUpDown1.Value.ToString();
            }
            else { textBox1.Text = numericUpDown1.Value.ToString(); }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                ses = true;
            }
            else { ses = false; }
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void gösterGizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = !Visible;
        }
        int k = 1;
        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            k = 1;
            try
            {
                string sb = "";
                foreach(ListViewItem.ListViewSubItem lvi_s in listView1.Items[0].SubItems)
                {
                    if (lvi_s.Text.Contains(".") == false)
                    {
                        sb += (listView1.Columns[k].Text.Substring(0,1) + ": "+ lvi_s.Text + "\n");
                        k++;
                    }        
                }
                notifyIcon1.Text = sb;
            }
            catch (Exception ) {}
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Yapımcı: Sagopa K\nKaynak Site: https://www.sabah.com.tr/namaz-vakitleri", "Hakkında",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
