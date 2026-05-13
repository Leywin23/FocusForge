using System;
using System.Collections.Generic;
using System.Text;

namespace DomainStudyService.Exceptions
{
    public class OpenPauseNotFoundException : Exception
    {
        public OpenPauseNotFoundException(string message) : base(message)
        {
            
        }
    }
}
