namespace ApplicationStudyService.Commands.StartStudySession
{
    public class StartStudySessionCommand
    {
        public Guid UserId { get; }
        public string Title { get; }

        public StartStudySessionCommand(Guid userId, string title)
        {
            UserId = userId;
            Title = title;
        }
    }
}