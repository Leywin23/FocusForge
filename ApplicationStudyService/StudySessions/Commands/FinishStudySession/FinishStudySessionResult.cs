using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Commands.FinishStudySession
{
    public sealed record FinishStudySessionResult(
        Guid SessionId,
        Guid UserId,
        DateTime FinishedAtUtc,
        int TotalStudySeconds,
        int TotalPausedSeconds,
        string Status);
}
