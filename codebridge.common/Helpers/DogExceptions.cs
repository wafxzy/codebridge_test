using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codebridge.Common.Helpers
{
    public class DogAlreadyExistsException : Exception
    {
        public DogAlreadyExistsException(string message) : base(message)
        {
        }

        public DogAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class DogNotFoundException : Exception
    {
        public DogNotFoundException(string message) : base(message)
        {
        }

        public DogNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
