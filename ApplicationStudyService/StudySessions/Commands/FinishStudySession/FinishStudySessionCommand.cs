using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Commands.FinishStudySession
{
    public class FinishStudySessionCommand
    {
        public Guid SessionId { get; }
        public Guid UserId { get; }

        public FinishStudySessionCommand(Guid sessionId, Guid userId)
        {
            SessionId = sessionId;
            UserId = userId;
        }
    }
}