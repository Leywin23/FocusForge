namespace ApplicationStudyService.StudySessions.Commands.PauseStudySession
{
    public sealed record PauseStudySessionResult(
        Guid SessionId,
        Guid UserId,
        DateTime PausedAtUtc,
        string Status);
}
