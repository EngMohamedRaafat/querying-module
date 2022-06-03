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
    public partial class Form3 : Form
    {
        DataSet DSload;
        DataTable[] tables;
        DataTable SpecifiedTable = new DataTable();
        miniSQL cmd = new miniSQL();

        string Query = null;

        public Form3()
        {
            InitializeComponent();

            checkedListBox1.Items.Clear();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            DSload = new DataSet();
            DSload.ReadXml("Data.xml");

            tables = new DataTable[DSload.Tables.Count];

            for (int i = 0; i < DSload.Tables.Count; i++)
            {
                tables[i] = DSload.Tables[i];
                comboBox1.Items.Add(tables[i].TableName);
                comboBox9.Items.Add(tables[i].TableName);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DSload.Tables.Contains(comboBox1.Text))
            {
                comboBox3.Items.Clear();
                comboBox7.Items.Clear();
                comboBox10.Items.Clear();
                checkedListBox1.Items.Clear();
                checkedListBox1.Items.Add("Select All");
                foreach (var column in DSload.Tables[comboBox1.Text].Columns)
                {
                    checkedListBox1.Items.Add(column.ToString());
                    comboBox3.Items.Add(column.ToString());
                    comboBox7.Items.Add(column.ToString());
                    comboBox10.Items.Add(column.ToString());
                }
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            //comboBox1.Text = "";
            if (comboBox1.Text == "" || !comboBox1.Items.Contains(comboBox1.Text))
                checkedListBox1.Items.Clear();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(0))
            {
                for (int i = 1; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
        }

        private void AppendText(string text)
        {
            richTextBox1.SelectionColor = Color.Gray;
            richTextBox1.AppendText(text);
            richTextBox1.SelectionColor = Color.Black;
        }

        private void CheckBoolExpression()
        {
            String[] oper = richTextBox1.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (oper[oper.Count() - 1].Contains("AND") || oper[oper.Count() - 1].Contains("OR") || oper[oper.Count() - 1] == ")" || oper[oper.Count() - 1] == "(")
            {
                richTextBox1.Undo();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
                MessageBox.Show("Enter your condition First", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                CheckBoolExpression();
                AppendText(" AND ");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
                MessageBox.Show("Enter your condition First", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                CheckBoolExpression();
                AppendText(" OR ");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CheckBoolExpression();
            AppendText(" ( ");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CheckBoolExpression();
            AppendText(" ) ");
        }

        private void Function_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Function_checkBox.Checked)
            {
                Function_GroupBox.Visible = true;
                Boolean_GroupBox.Visible = true;
            }
            else
            {
                Function_GroupBox.Visible = false;
                if (!Comparison_checkBox.Checked)
                    Boolean_GroupBox.Visible = false;
                comboBox7.Text = "";
                comboBox10.Text = "";
                comboBox6.Text = "Operator";
                comboBox8.Text = "Function";
                richTextBox1.Text = "";
            }
        }
        private void Comparison_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Comparison_checkBox.Checked)
            {
                Comparison_GroupBox.Visible = true;
                Boolean_GroupBox.Visible = true;
            }
            else
            {
                Comparison_GroupBox.Visible = false;
                if (!Function_checkBox.Checked)
                    Boolean_GroupBox.Visible = false;
                comboBox3.Text = "";
                comboBox2.Text = "Operator";
                textBox1.Text = "";
                richTextBox1.Text = "";
            }
        }

        private void Option_linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Option_linkLabel.Text == "Show Filtering Options" || linkLabel1.Text == "Show Filtering Options")
            {
                Option_linkLabel.Text = "Hide Filtering Options";
                linkLabel1.Text = "Hide Filtering Options";
                Options_GroupBox.Visible = true;
            }
            else
            {
                Option_linkLabel.Text = "Show Filtering Options";
                linkLabel1.Text = "Show Filtering Options";
                Options_GroupBox.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "" || comboBox2.Text == "" || comboBox2.Text == "Operator" || textBox1.Text == "")
                MessageBox.Show("Please fill all fields", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                richTextBox1.AppendText(comboBox3.Text + " " + comboBox2.Text + " " + textBox1.Text);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox7.Text == "" || comboBox6.Text == "" || comboBox6.Text == "Operator" || comboBox8.Text == "" || comboBox8.Text == "Function" || comboBox10.Text == "")
                MessageBox.Show("Please fill all fields", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                richTextBox1.AppendText(comboBox7.Text + " " + comboBox6.Text + " " + comboBox8.Text + "(" + comboBox10.Text + ")");
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        bool IsString = false;

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var x = double.Parse(DSload.Tables[comboBox1.Text].Rows[0][comboBox3.Text].ToString());
                IsString = false;
            }
            catch (Exception)
            {
                IsString = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !IsString)
                e.Handled = true;
            base.OnKeyPress(e);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var x = double.Parse(DSload.Tables[comboBox1.Text].Rows[0][comboBox7.Text].ToString());
                IsString = false;
            }
            catch (Exception)
            {
                IsString = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text == "")
                MessageBox.Show("Select Table Name", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (checkedListBox1.CheckedItems.Count == 0)
                MessageBox.Show("Select Column(/s)", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Query = "SELECT ";
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                {
                    //remove 
                    Query += checkedListBox1.CheckedItems[i].ToString() + " , ";
                }
                Query = Query.Remove(Query.Length - ", ".Length);
                Query += "FROM " + comboBox1.Text;

                SpecifiedTable = cmd.ExecuteNonQuery(Query, sender, e);
            }

            if (Comparison_checkBox.Checked || Function_checkBox.Checked)
            {
                if (richTextBox1.Text != "")
                {
                    if (richTextBox1.Text.Contains("!="))
                        richTextBox1.Text = richTextBox1.Text.Replace("!=", "<>");
                    Query += " where " + richTextBox1.Text;
                    SpecifiedTable = cmd.ExecuteNonQuery(Query, sender, e);
                }
            }

            dataGridView1.DataSource = SpecifiedTable;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Option_linkLabel_LinkClicked(sender, e);
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DSload.Tables.Contains(comboBox9.Text))
            {
                comboBox3.Items.Clear();
                comboBox7.Items.Clear();
                comboBox10.Items.Clear();
                comboBox4.Items.Clear();
                foreach (var column in DSload.Tables[comboBox9.Text].Columns)
                {
                    comboBox3.Items.Add(column.ToString());
                    comboBox7.Items.Add(column.ToString());
                    comboBox4.Items.Add(column.ToString());
                    comboBox10.Items.Add(column.ToString());
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox9.Text == "" || comboBox4.Text == "" || comboBox5.Text == "Function" || comboBox5.Text == "")
                MessageBox.Show("Please fill all fields", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!DSload.Tables.Contains(comboBox9.Text))
                MessageBox.Show("This Table is NOT Exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (!DSload.Tables[comboBox9.Text].Columns.Contains(comboBox4.Text))
                MessageBox.Show("This Column is NOT Exist", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string item = comboBox5.Text + " ( ";
                if (checkBox1.Checked)
                    item += "Distinct ";
                item += comboBox4.Text + " )";
                listBox1.Items.Add(item);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Query = "SELECT ";
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                Query += listBox1.Items[i].ToString() + " , ";
            }
            Query = Query.Remove(Query.Length - ", ".Length);
            Query += "FROM " + comboBox9.Text;

            SpecifiedTable = cmd.ExecuteNonQuery(Query, sender, e);

            if (Comparison_checkBox.Checked || Function_checkBox.Checked)
            {
                if (richTextBox1.Text != "")
                {
                    if (richTextBox1.Text.Contains("!="))
                        richTextBox1.Text = richTextBox1.Text.Replace("!=", "<>");
                    Query += " where " + richTextBox1.Text;
                    SpecifiedTable = cmd.ExecuteNonQuery(Query, sender, e);
                }
            }

            dataGridView1.DataSource = SpecifiedTable;
        }
    }
}
