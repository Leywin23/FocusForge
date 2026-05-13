using System;
using System.Collections.Generic;
using System.Text;

namespace DomainStudyService.Exceptions
{
    public class SessionAlreadyCompletedException : Exception
    {
        public SessionAlreadyCompletedException(string message) : base(message)
        {
        }
    }
}
