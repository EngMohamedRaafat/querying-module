using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Querying_Module
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            miniSQL form = new miniSQL();
            this.Hide();
            form.ShowDialog();
            this.Show();
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
