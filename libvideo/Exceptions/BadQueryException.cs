using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothVidLib.Exceptions
{
    internal class BadQueryException : Exception
    {
        public BadQueryException()
            : base()
        { }

        public BadQueryException(string message)
            : base(message)
        { }

        public BadQueryException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
