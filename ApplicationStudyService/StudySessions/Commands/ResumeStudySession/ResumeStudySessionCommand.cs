using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Commands.ResumeStudySession
{
    public class ResumeStudySessionCommand
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }

        public ResumeStudySessionCommand(Guid userId, Guid sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
        }
    }
}
