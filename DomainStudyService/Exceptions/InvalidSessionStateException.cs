using System;
using System.Collections.Generic;
using System.Text;

namespace DomainStudyService.Exceptions
{
    public class InvalidSessionStateException : Exception
    {
        public InvalidSessionStateException(string message) : base(message)
        {
        }
    }
}
