using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Querying_Module
{
    class QueryStyle
    {
        Regex rex;

        public QueryStyle()
        {
            rex = null;
        }
        //public QueryStyle( ref QueryStyle obj) { }

        public RichTextBox Get_Style(RichTextBox rTxtBox, string tokens, Color color)
        {
            rTxtBox.SelectionColor = Color.Black;

            rex = new Regex(tokens);

            MatchCollection mc = rex.Matches(rTxtBox.Text.ToLower());
            int StartCursorPosition = rTxtBox.SelectionStart;
            foreach (Match m in mc)
            {
                int startIndex = m.Index;
                int StopIndex = m.Length;

                rTxtBox.Select(startIndex, StopIndex);
                rTxtBox.SelectionColor = color;
                rTxtBox.DeselectAll();
                rTxtBox.SelectionStart = StartCursorPosition;
                rTxtBox.SelectionColor = Color.Black;
            }

            return rTxtBox;
        }
    }
}
