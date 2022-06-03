using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Querying_Module
{
    class WhereException : Exception
    {
        public DataTable Table;

        public WhereException() { Table = null; }
        public WhereException(DataTable dt)
        {
            Table = dt;
        }
    }
}
