namespace ApplicationStudyService.StudySessions.Exceptions
{
    public class ActiveStudySessionAlreadyExistsException : Exception
    {
        public ActiveStudySessionAlreadyExistsException(Guid userId)
            : base($"User with ID {userId} already has an active study session.")
        {
        }
    }
}