namespace ApplicationStudyService.StudySessions.Commands.ResumeStudySession
{
    public sealed record ResumeStudySessionResult(
        Guid SessionId,
        Guid UserId,
        DateTime ResumedAtUtc,
        int TotalPausedSeconds,
        string Status
        );
}
