namespace ApplicationStudyService.StudySessions.Commands.FinishStudySession
{
    public class FinishStudySessionCommand
    {
        public Guid UserId { get; }
        public Guid SessionId { get; }
        
        public FinishStudySessionCommand(Guid userId, Guid sessionId)
        {
            UserId = userId;
            SessionId = sessionId;
        }
    }
}