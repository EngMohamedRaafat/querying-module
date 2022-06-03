using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Querying_Module
{
    class SQLquery : QuerySeparation
    {
        public SQLquery() { }

        public DataTable[] GetTables(string[] TablesName)
        {
            DataSet DSload = new DataSet();
            DSload.ReadXml("Data.xml");

            DataTable[] SpecifiedTables = new DataTable[TablesName.Length];

            int index = 0;
            for (int i = 0; i < TablesName.Length; i++)
            {
                if (DSload.Tables.Contains(TablesName[i]))
                {
                    SpecifiedTables[index] = DSload.Tables[DSload.Tables.IndexOf(TablesName[i])];
                    index++;
                }
                else
                    miniSQL.ErrorList.Add("Syntax Error : Invalid table name '" + TablesName[i] + "'");
                    //throw new SyntaxErrorException("Syntax Error : Invalid table name '" + TablesName[i] + "'");
            }

            return SpecifiedTables;
        }

        public DataTable GetColumns(string[] ColumnsName, DataTable TablesName)
        {
            DataTable dt = new DataTable();
            DataRow[] row = new DataRow[TablesName.Rows.Count];

            //int index = 0;

            for (int i = 0; i < ColumnsName.Length; i++)
            {
                if (TablesName.Columns.Contains(ColumnsName[i]))
                {
                    dt.Columns.Add(ColumnsName[i]);
                }
                else
                    throw new SyntaxErrorException("Syntax Error : Invalid column name '" + ColumnsName[i] + "'");
            }

            for (int j = 0; j < TablesName.Rows.Count; j++)
            {
                row[j] = TablesName.Rows[j];
            }

            for (int j = 0; j < TablesName.Rows.Count; j++)
            {
                object[] array = new object[dt.Columns.Count];

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    array[i] = row[j][dt.Columns[i].ColumnName];
                }
                dt.Rows.Add(array);
            }


            return dt;
        }


        public void CreateXMLfile(DataTable table, string condition)
        {
            //DataTable dt1 = new DataTable("Patients");
            //dt1.Columns.Add("ID",typeof(int));
            //dt1.Columns.Add("Name", typeof(string));
            //dt1.Columns.Add("Age", typeof(int));
            //dt1.Columns.Add("Gender", typeof(string));
            //dt1.Columns.Add("BloodGroup", typeof(string));

            //dt1.Rows.Add(1, "ahmed", 22, "male", "A+");
            //dt1.Rows.Add(2, "mona", 20, "female", "B+");
            //dt1.Rows.Add(3, "omer", 28, "male", "A+");
            //dt1.Rows.Add(4, "heba", 25, "male", "AB+");
            //dt1.Rows.Add(5, "mahmoud", 21, "male", "AB+");
            //dt1.Rows.Add(6, "sarah", 23, "female", "B+");
            //dt1.Rows.Add(7, "mohamed", 32, "male", "O+");

            //DataTable dt2 = new DataTable("Employees");
            //dt2.Columns.Add("ID", typeof(int));
            //dt2.Columns.Add("Name", typeof(string));
            //dt2.Columns.Add("Department", typeof(string));
            //dt2.Columns.Add("Gender", typeof(string));
            //dt2.Columns.Add("Country", typeof(string));
            //dt2.Columns.Add("Salary", typeof(double));
            //dt2.Rows.Add(1, "John", "IT", "Male", "UK", 5000);
            //dt2.Rows.Add(2, "Mary", "HR", "Female", "India", 3000);
            //dt2.Rows.Add(3, "Todd", "HR", "Male", "UK", 3500);
            //dt2.Rows.Add(4, "Donia", "IT", "Female", "Cairo", 3500);
            //dt2.Rows.Add(5, "Mohamed", "IT", "Male", "Cairo", 5500);
            //dt2.Rows.Add(6, "Pam", "HR", "Female", "India", 4000);
            //dt2.Rows.Add(7, "Ahmed", "Payroll", "Male", "USA", 2000);
            //dt2.Rows.Add(8, "Sunil", "IT", "Male", "USA", 2400);
            //dt2.Rows.Add(9, "Ahmed", "Payroll", "Male", "USA", 2400);
            //dt2.Rows.Add(10, "Hari", "Marketing", "Male", "UK", 2500);
            //dt2.Rows.Add(11, "Sunitha", "HR", "Female", "India", 4000);
            //dt2.Rows.Add(12, "Mona", "HR", "Female", "Cairo", 4000);
            //dt2.Rows.Add(13, "Sneha", "IT", "Female", "India", 3000);
            //dt2.Rows.Add(14, "Ruby", "Payroll", "Male", "UK", 4600);
            //dt2.Rows.Add(15, "Ahmed", "Marketing", "Male", "UK", 3600);
            //dt2.Rows.Add(16, "Mahmoud", "Sales", "Male", "Cairo", 2600);
            //dt2.Rows.Add(17, "Lara", "Sales", "Female", "Paris", 2600);
            //dt2.Rows.Add(18, "Jamas", "Accounting", "Male", "Paris", 4300);
            //dt2.Rows.Add(19, "Jennifer", "Accounting", "Female", "Paris", 4300);
            //dt2.Rows.Add(20, "Mohamed", "Accounting", "Male", "Paris", 4850);

            //DataTable dt3 = new DataTable("Products");
            //dt3.Columns.Add("ID", typeof(int));
            //dt3.Columns.Add("Name", typeof(string));
            //dt3.Columns.Add("Price", typeof(double));

            //dt3.Rows.Add(14, "phone", 115);
            //dt3.Rows.Add(15, "tv", 220);
            //dt3.Rows.Add(16, "glasses", 55);
            //dt3.Rows.Add(17, "book", 15);

            //DataSet DS = new DataSet("Data");
            //DS.Tables.Add(dt1);
            //DS.Tables.Add(dt2);
            //DS.Tables.Add(dt3);

            //DS.WriteXml("Data.xml");
            //DS.WriteXmlSchema("Data.xsd");
        }
    }
}
