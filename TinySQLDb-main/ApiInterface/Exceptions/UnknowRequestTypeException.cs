using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Exceptions
{
    public class UnknownSQLSentenceException : Exception
    {
        public UnknownSQLSentenceException() : base("SQL sentence could not be parsed due to invalid syntax.") { }
    }
}
