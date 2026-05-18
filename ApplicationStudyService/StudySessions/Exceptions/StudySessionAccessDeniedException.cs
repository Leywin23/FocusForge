using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Exceptions
{
    public class StudySessionAccessDeniedException : Exception
    {
        public StudySessionAccessDeniedException(Guid sessionId, Guid userId) 
            : base($"Session {sessionId} doesn't belong to user {userId}")
        {
        }
    }
}
