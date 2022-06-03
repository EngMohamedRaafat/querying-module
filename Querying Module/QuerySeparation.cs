using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Querying_Module
{
    class QuerySeparation
    {
        public string Query;

        public string[] SelectClause;
        public string[] FromClause;
        public string WhereClause;
        public string[] GroupByClause;
        public string[] HavingClause;
        public string OrderByClause;

        public bool Distinct;

        public QuerySeparation()
        {
            Query = null;
            SelectClause = null;
            FromClause = null;
            WhereClause = null;
            GroupByClause = null;
            HavingClause = null;
            OrderByClause = null;
            Distinct = false;
        }
        public void Separate(string query)
        {
            Query = query;
            SelectClause = null;
            FromClause = null;
            WhereClause = null;
            GroupByClause = null;
            HavingClause = null;
            OrderByClause = null;
            Distinct = false;

            string[] fields = null;

            query = query.Trim();
            string[] temp = query.Split(new char [] {' '} , StringSplitOptions.RemoveEmptyEntries);

            if (temp[0].ToLower() == "select")
            {
                //Distinct = false;
                if (temp[1].ToLower() == "distinct")
                {
                    Distinct = true;
                }
                string noSpace_RTxtBox = query.Trim();
                noSpace_RTxtBox = noSpace_RTxtBox.Replace(" ", "").ToLower(); // results a text with no space

                string[] separators = { "select", "from", "where", "groupby", "having", "orderby" };
                fields = noSpace_RTxtBox.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < fields.Length; i++)
                {
                    if (SelectClause == null)
                    {
                        string[] x = fields[i].Split(',');
                        if (x[0].ToLower().Trim().Contains("distinct") & !ContainsFunction(x[0]))
                        {
                            x[0] = x[0].Replace("distinct", "");
                            SelectClause = x;
                        }
                        else
                        {
                            SelectClause = fields[i].Split(',');
                        }
                    }
                    else if (FromClause == null)
                        FromClause = fields[i].Split(',');
                    else if (WhereClause == null && noSpace_RTxtBox.ToLower().Contains("where"))
                    {
                        string[] WhereStatment = { separators[2], separators[3], separators[4], separators[5],"order by" };
                        WhereClause = query.ToLower().Split(WhereStatment, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                    else if (OrderByClause == null && noSpace_RTxtBox.ToLower().Contains("orderby"))
                    {
                        if (fields[i].Contains("asc"))
                            OrderByClause = fields[i].Remove(fields[i].Length - "asc".Length);
                        else if (fields[i].Contains("desc"))
                            OrderByClause = fields[i].Insert(fields[i].Length - "desc".Length, " ");
                        else
                            OrderByClause = fields[i];
                    }
                    else if (GroupByClause == null)
                        GroupByClause = fields[i].Split(',');
                    else if (HavingClause == null)
                        HavingClause = fields[i].Split(',');
                }
            }
        }

        public bool ContainsFunction(string Statment)
        {
            if (Statment.ToLower().Replace(" ", "").Contains("sum") || Statment.ToLower().Contains("avg") || Statment.ToLower().Contains("min") || Statment.ToLower().Contains("max") || Statment.ToLower().Replace(" ", "").Contains("count("))
                return true;
            return false;
        }

        public void Apply_Functions(DataTable table)
        {
            string[] separators = {"and", "or"};
            string[] conditions = WhereClause.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string function in conditions)
            {
                if (ContainsFunction(function))
                {
                    string col = null;
                    if (function.ToLower().Contains("sum"))
                    {
                        string[] sep = {"sum("};
                        col = function.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        //col = function.Replace("sum(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();
                        double[] x = Selected_Column_Values(table, col);
                        WhereClause = WhereClause.Replace("sum(" + col + ")", x.Sum().ToString());
                    }
                    else if (function.ToLower().Contains("min"))
                    {
                        string[] sep = { "min(" };
                        col = function.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        //col = function.Replace("min(", "");
                        col = col .Replace(")", "");
                        col = col.Trim();
                        double[] x = Selected_Column_Values(table, col);
                        WhereClause = WhereClause.Replace("min(" + col + ")", x.Min().ToString());
                    }
                    else if (function.ToLower().Contains("max"))
                    {
                        string[] sep = { "max(" };
                        col = function.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        //col = function.Replace("max(", "");
                        col = col .Replace(")", "");
                        col = col.Trim();
                        double[] x = Selected_Column_Values(table, col);
                        WhereClause = WhereClause.Replace("max(" + col + ")", x.Max().ToString());
                    }
                    else if (function.ToLower().Contains("count"))
                    {
                        string[] sep = { "count(" };
                        col = function.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        //col = function.Replace("count(", "");
                        col = col .Replace(")", "");
                        col = col.Trim();
                        double[] x = Selected_Column_Values(table, col);
                        WhereClause = WhereClause.Replace("count(" + col + ")", x.Count().ToString());
                    }
                    else if (function.ToLower().Contains("avg"))
                    {
                        string[] sep = { "avg(" };
                        col = function.Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                        //col = function.Replace("avg(", "");
                        col = col .Replace(")", "");
                        col = col.Trim();
                        double[] x = Selected_Column_Values(table, col);
                        WhereClause = WhereClause.Replace("avg(" + col + ")", (x.Sum() / x.Count()).ToString());
                    }
                }
            }
        }

        public bool SELECT_hasfunction()
        {
            foreach (string item in SelectClause)
            {
                if (item.Contains("sum") || item.Contains("min") || item.Contains("max") || item.Contains("count") || item.Contains("avg"))
                    return true;
            }
            return false;   
        }

        public DataTable TableOfFunctionResult(DataTable table)
        {
            DataTable ResultTable = new DataTable();

            int ColNum = 0;
            foreach (string item in SelectClause)
            {
                if (ContainsFunction(item))
                    ColNum++;
            }

            object[] row = new object[ColNum];
            int i = 0;

            foreach (string function in SelectClause)
            {
                if (ContainsFunction(function))
                {
                    string col = null;

                    if (function.ToLower().Contains("sum"))
                    {
                        col = function.Replace("sum(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();
                        ResultTable.Columns.Add(function, typeof(double));
                        row[i++] = Selected_Column_Values(table, col).Sum();
                    }
                    else if (function.ToLower().Contains("min"))
                    {
                        col = function.Replace("min(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();
                        ResultTable.Columns.Add(function, typeof(double));
                        row[i++] = Selected_Column_Values(table, col).Min();
                    }
                    else if (function.ToLower().Contains("max"))
                    {
                        col = function.Replace("max(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();
                        ResultTable.Columns.Add(function, typeof(double));
                        row[i++] = Selected_Column_Values(table, col).Max();
                    }
                    else if (function.ToLower().Contains("count"))
                    {
                        col = function.Replace("count(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();

                        if (col.Contains("distinct"))
                        {
                            col = col.Replace("distinct", "").Trim();
                            string[] c = {col};

                            ResultTable.Columns.Add(function.Replace("distinct", "DISTINCT "), typeof(double));
                            row[i++] = double.Parse(table.DefaultView.ToTable(table.TableName, true, c).Rows.Count.ToString());
                            continue;
                        }
                        ResultTable.Columns.Add(function.Replace("distinct", "DISTINCT "), typeof(double));
                        row[i++] = double.Parse(table.Rows.Count.ToString());
                    }
                    else if (function.ToLower().Contains("avg"))
                    {
                        col = function.Replace("avg(", "");
                        col = col.Replace(")", "");
                        col = col.Trim();
                        ResultTable.Columns.Add(function, typeof(double));
                        double[] x = Selected_Column_Values(table, col);
                        row[i++] = x.Sum() / x.Count();
                    }
                }
            }
            ResultTable.Rows.Add(row);

            return ResultTable;
        }

        public double[] Selected_Column_Values(DataTable dt, string column)
        {
            double[] x = new double[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                x[i] = double.Parse(dt.Rows[i][column].ToString());
            }

            return x;
        }


    }
}
