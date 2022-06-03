using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Querying_Module
{
    public partial class Introduction : Form
    {
        public Introduction()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.enableContextMenu = false;
            axWindowsMediaPlayer1.windowlessVideo = false;

            axWindowsMediaPlayer1.URL = Directory.GetCurrentDirectory() + "\\Flare_1.avi";
            axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            timer1.Dispose();
            this.Hide();
            f.ShowDialog();
        }
    }
}
