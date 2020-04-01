using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace EzanAlarmi
{
    public partial class Alarm : Form
    {
        SoundPlayer sp;
        public Alarm(string vakit_ismi, string saat, bool sesli_mi)
        {
            InitializeComponent();
            if(sesli_mi == true) {sp = new SoundPlayer("alarm.wav"); sp.Play(); }
            label2.Text = vakit_ismi;
            label3.Text = saat;
        }
        protected override void OnLoad(EventArgs e)
        {
            Screen ekran = Screen.FromPoint(Location);
            Location = new Point(ekran.WorkingArea.Right - Width, ekran.WorkingArea.Bottom - Height);
            base.OnLoad(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Alarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sp != null) { sp.Stop(); sp.Dispose(); }
        }
    }
}
