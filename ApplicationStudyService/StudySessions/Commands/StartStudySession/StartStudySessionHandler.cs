using ApplicationStudyService.Common.Interfaces;
using ApplicationStudyService.StudySessions.Exceptions;
using DomainStudyService.Entities;

namespace ApplicationStudyService.StudySessions.Commands.StartStudySession
{
    public sealed class StartStudySessionHandler
    {
        private readonly IStudySessionRepository _studySessionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public StartStudySessionHandler(IStudySessionRepository studySessionRepository, IDateTimeProvider dateTimeProvider)
        {
            _studySessionRepository = studySessionRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<StartStudySessionResult> HandleAsync(StartStudySessionCommand command, CancellationToken cancellationToken)
        {
            var currentSession = await _studySessionRepository.GetCurrentByUserIdAsync(command.UserId, cancellationToken);
            if (currentSession != null)
                throw new ActiveStudySessionAlreadyExistsException(command.UserId);
            
            var newSession = StudySession.Start(command.UserId, command.Title, _dateTimeProvider.UtcNow);
            await _studySessionRepository.AddAsync(newSession, cancellationToken);
            await _studySessionRepository.SaveChangesAsync(cancellationToken);

            return new StartStudySessionResult(
                newSession.Id,
                newSession.UserId,
                newSession.Title,
                newSession.StartTime,
                newSession.Status.DisplayStatus);

        }
    }
      
}