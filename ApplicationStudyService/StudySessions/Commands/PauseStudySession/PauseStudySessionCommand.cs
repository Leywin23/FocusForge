namespace ApplicationStudyService.StudySessions.Commands.PauseStudySession
{
    public class PauseStudySessionCommand
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }
        public PauseStudySessionCommand(Guid userId, Guid sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
        }
    }
}
