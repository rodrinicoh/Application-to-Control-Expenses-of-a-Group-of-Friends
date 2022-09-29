using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exceptions
{
    public class ReunionException : Exception
    {
        public ReunionException(string message) 
            : base(message)
        {
        }

        public ReunionException()
        {
        }

        public ReunionException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
