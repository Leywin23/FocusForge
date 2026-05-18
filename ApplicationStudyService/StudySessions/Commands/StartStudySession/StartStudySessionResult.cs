namespace ApplicationStudyService.StudySessions.Commands.StartStudySession
{
    public sealed record StartStudySessionResult(
    Guid Id,
    Guid UserId,
    string Title,
    DateTime StartTime,
    string Status);
}