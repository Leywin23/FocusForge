using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Exceptions
{
    public class StudySessionNotFoundException : Exception
    {
        public StudySessionNotFoundException(Guid sessionId)
            : base($"Study session with ID {sessionId} was not found.")
        {
        }
    }
}
