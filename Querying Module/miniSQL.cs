using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using XCoolFormTest;
using Querying_Module.Properties;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Threading;

namespace Querying_Module
{
    public partial class miniSQL : XCoolForm.XCoolForm
    {
        public static List<string> ErrorList = new List<string>();

        private XmlThemeLoader xtl = new XmlThemeLoader();

        QuerySeparation query = new QuerySeparation();
        SQLquery operation = new SQLquery();
        QueryStyle style = new QueryStyle();
        DataTable SpecifiedTable = new DataTable();

        /////////////////////
        
        string Clause = "(select|from|where|in|distinct|group by|having|order by)";
        string tokens = "(and|or)";
        string AggregateFunction = "(sum|min|max|avg|count)";

        
        ////////////////////////////
        SpeechSynthesizer sSynth = new SpeechSynthesizer();
        PromptBuilder pBuilder = new PromptBuilder();
        SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        
        public miniSQL()
            : base()
        {
            InitializeComponent();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int x = 0;

                richTextBox1.SelectionColor = Color.Black;

                int StartCursorPosition = richTextBox1.SelectionStart;

                richTextBox1.Undo();
                x = richTextBox1.Text.Length;
                richTextBox1.Redo();

                if (StartCursorPosition < x)
                {
                    string token = "(" + richTextBox1.Text.Replace(' ', '|') + ")";
                    if (token.Contains("|*"))
                        token.Replace("|*", "");
                    else
                        richTextBox1 = style.Get_Style(richTextBox1, token, Color.Black);
                }

                richTextBox1 = style.Get_Style(richTextBox1, Clause, Color.Blue);
                richTextBox1 = style.Get_Style(richTextBox1, tokens, Color.Gray);
                richTextBox1 = style.Get_Style(richTextBox1, AggregateFunction, Color.Fuchsia);

                string[] txt = richTextBox1.Text.Split('\'');
                if (txt.Length > 1)
                {
                    string name = "(";
                    for (int i = 0; i < txt.Length; i++)
                    {
                        if (i % 2 == 1)
                        {
                            if (name == "(")
                                name = name + txt[i];
                            else
                                name = name + "|" + txt[i];
                        }
                    }
                    name = name + ")";
                    richTextBox1 = style.Get_Style(richTextBox1, name, Color.DarkRed);
                }
            }
            catch (Exception) { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.DataSource = null;
                listBox2.Items.Clear();
                ErrorList.Clear();

                query.Separate(richTextBox1.Text);

                dataGridView1.DataSource = null;

                SpecifiedTable = null;

                if (query.FromClause != null)
                {
                    SpecifiedTable = operation.GetTables(query.FromClause)[0];
                }
                else
                {
                    ErrorList.Add("Invalid structure : Syntax Error near 'from' (enter table name)");
                    //throw new SyntaxErrorException(@"Syntax Error : Invalid structure 
                    //
                    //SQL SELECT Statement:
                    //
                    //SELECT column_list FROM table-name
                    //[WHERE Clause]
                    //[GROUP BY Clause]
                    //[HAVING Clause]
                    //[ORDER BY Clause]");
                }

                //edit here 
                if (query.SelectClause != null)
                {
                    if (richTextBox1.Text.ToLower().Replace(" ", "").Contains("orderby"))
                    {
                        if (query.OrderByClause != null)
                        {
                            SpecifiedTable.DefaultView.Sort = query.OrderByClause;
                        }
                        else
                            ErrorList.Add("Invalid structure : Syntax Error near 'order by'");
                    }
                    
                    if (query.WhereClause != null && !query.ContainsFunction(query.WhereClause))
                    {
                        SpecifiedTable.DefaultView.RowFilter = query.WhereClause;
                        SpecifiedTable = SpecifiedTable.DefaultView.ToTable(SpecifiedTable.TableName);
                    }
                    else if (query.WhereClause != null && query.ContainsFunction(query.WhereClause))
                    {
                        query.Apply_Functions(SpecifiedTable);
                        SpecifiedTable.DefaultView.RowFilter = query.WhereClause;
                        SpecifiedTable = SpecifiedTable.DefaultView.ToTable(SpecifiedTable.TableName);
                    }
                    else
                    {
                        if (richTextBox1.Text.ToLower().Contains("where"))
                            ErrorList.Add("Invalid structure : Syntax Error near 'where' (enter a valid condition)");
                            //throw new SyntaxErrorException("Invalid structure : Syntax Error near 'where' (enter a valid condition)");
                    }

                    string[] temp = richTextBox1.Text.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (temp[1] == "*" || temp[2] == "*")
                    {
                        if (query.Distinct)
                            SpecifiedTable = SpecifiedTable.DefaultView.ToTable(SpecifiedTable.TableName);
                    }
                    else if (query.SELECT_hasfunction())
                    {
                        SpecifiedTable = query.TableOfFunctionResult(SpecifiedTable);
                    }
                    else
                    {
                        SpecifiedTable = SpecifiedTable.DefaultView.ToTable(SpecifiedTable.TableName, query.Distinct, query.SelectClause);
                    }
                }
                else
                {
                    ErrorList.Add("Invalid structure : Syntax Error near 'select' ( you must enter 'column(/s)' name(/s) )");
                    //throw new SyntaxErrorException(@"Syntax Error : you must enter 'column(/s)' name(/s) 
                    //
                    //SQL SELECT Statement:
                    //
                    //SELECT column_list FROM table-name
                    //[WHERE Clause]
                    //[GROUP BY Clause]
                    //[HAVING Clause]
                    //[ORDER BY Clause]");
                }

                if (ErrorList.Count == 0)
                {
                    tabControl1.Visible = false;
                    dataGridView1.DataSource = SpecifiedTable;
                }
                else
                {
                    listBox2.DataSource = ErrorList;
                    tabControl1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ErrorList.Add(ex.Message);
                listBox2.DataSource = ErrorList;
                tabControl1.Visible = true;
                //MessageBox.Show(ex.Message);
            }
        }

        public DataTable ExecuteNonQuery(string Query ,object sender, EventArgs e)
        {
            richTextBox1.Text = Query;

            button1_Click(sender, e);

            return SpecifiedTable;
        }

        private void button2_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void miniSQL_Load(object sender, EventArgs e)
        {
            this.MenuIcon = Querying_Module.Properties.Resources.Untitled_1.GetThumbnailImage(50, 24, null, IntPtr.Zero);

            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(60));
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(200, "miniSQL"));
            this.StatusBar.BarItems.Add(new XCoolForm.XStatusBar.XBarItem(80, "Done"));
            this.StatusBar.EllipticalGlow = false;

            xtl.ThemeForm = this;
            xtl.ApplyTheme(Path.Combine(Environment.CurrentDirectory, @"..\..\Themes\StandardWindowsTheme.xml"));
            this.TitleBar.TitleBarCaption = "Querying Module";
            this.TitleBar.TitleBarFill = XCoolForm.XTitleBar.XTitleBarFill.AdvancedRendering;
            this.TitleBar.TitleBarType = XCoolForm.XTitleBar.XTitleBarType.Rounded;
        }

        bool flag = true;
        private void button3_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                button3.BackgroundImage = Querying_Module.Properties.Resources.Microphone_Normal_Red_icon;
                flag = false;

                Choices sList = new Choices();
                sList.Add(new string[] { "tektek", "tak", "tek", "taktak", "execute", "select", "all", "select all", "distinct", "from", "where", "back", "redo", "order by", "Patients", "Employees", "Products", "id", "name", "close", "all", "price", "sum", "min", "avg", "max", "count", "comma", "clean", "age", "gender", "bloodgroup", "department", "country", "salary", "greater than", "less than", "equals", "in", "not", "like", "and", "or", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
                Grammar gr = new Grammar(new GrammarBuilder(sList));
                try
                {
                    sRecognize.RequestRecognizerUpdate();
                    sRecognize.LoadGrammar(gr);
                    sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                    sRecognize.SetInputToDefaultAudioDevice();
                    sRecognize.RecognizeAsync(RecognizeMode.Multiple);
                    sRecognize.Recognize();
                }

                catch
                {
                    return;
                }
            }
            else
            {
                flag = true;
                button3.BackgroundImage = Resources.Microphone_Hot_icon__1_;

                sRecognize.RecognizeAsyncStop();
            }
        }
        private void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "close" || e.Result.Text == "taktak" || e.Result.Text == "tektek" || e.Result.Text == "tak" || e.Result.Text == "tek")
            {
                Application.Exit();
            }
            else if (e.Result.Text == "select all")
            {
                richTextBox1.Text += "select * ";
            }
            else if (e.Result.Text == "all")
            {
                richTextBox1.Text += " * ";
            }
            else if (e.Result.Text == "comma")
            {
                richTextBox1.Text += ",";
            }
            else if (e.Result.Text == "execute")
            {
                button1_Click(sender, e);
            }
            else if (e.Result.Text == "clean")
            {
                richTextBox1.Text = "";
            }
            else if (e.Result.Text == "greater than")
            {
                richTextBox1.Text += ">";
            }
            else if (e.Result.Text == "less than")
            {
                richTextBox1.Text += "<";
            }
            else if (e.Result.Text == "equals")
            {
                richTextBox1.Text += "=";
            }
            else if (e.Result.Text == "back")
            {
                richTextBox1.Undo();
            }
            else if (e.Result.Text == "redo")
            {
                richTextBox1.Redo();
            }
            else
            {
                richTextBox1.Text += " " + e.Result.Text.ToString();
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
    }
}
