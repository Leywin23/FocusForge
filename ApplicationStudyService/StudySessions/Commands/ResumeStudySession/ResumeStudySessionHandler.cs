using ApplicationStudyService.Common.Interfaces;
using ApplicationStudyService.StudySessions.Exceptions;

namespace ApplicationStudyService.StudySessions.Commands.ResumeStudySession
{
    public class ResumeStudySessionHandler
    {
        private readonly IStudySessionRepository _studySessionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ResumeStudySessionHandler(IStudySessionRepository studySessionRepository, IDateTimeProvider dateTimeProvider)
        {
            _studySessionRepository = studySessionRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ResumeStudySessionResult> HandleAsync(ResumeStudySessionCommand command, CancellationToken cancellationToken)
        {
            var session = await _studySessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
            if (session == null)
                throw new StudySessionNotFoundException(command.SessionId);

            if (session.UserId != command.UserId)
                throw new StudySessionAccessDeniedException(command.SessionId, command.UserId);

            var resumedAtUtc = _dateTimeProvider.UtcNow;
            session.Resume(resumedAtUtc);

            await _studySessionRepository.SaveChangesAsync(cancellationToken);

            return new ResumeStudySessionResult(
                session.Id,
                session.UserId,
                resumedAtUtc,
                session.TotalPausedSeconds,
                session.Status.DisplayStatus
                );
        }
    }
}
