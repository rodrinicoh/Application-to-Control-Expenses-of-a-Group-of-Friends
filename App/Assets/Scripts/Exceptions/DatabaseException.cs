using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) 
            : base(message)
        {
        }

        public DatabaseException()
        {
        }

        public DatabaseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
