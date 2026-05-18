using ApplicationStudyService.Common.Interfaces;
using ApplicationStudyService.StudySessions.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Commands.PauseStudySession
{
    public class PauseStudySessionHandler
    {
        private readonly IStudySessionRepository _studySessionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public PauseStudySessionHandler(IStudySessionRepository studySessionRepository, IDateTimeProvider dateTimeProvider)
        {
            _studySessionRepository = studySessionRepository;
            _dateTimeProvider = dateTimeProvider;
        }
        public async Task<PauseStudySessionResult> HandleAsync(PauseStudySessionCommand command, CancellationToken cancellationToken)
        {
            var session = await _studySessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
            if (session == null)
                throw new StudySessionNotFoundException(command.SessionId);

            if (session.UserId != command.UserId)
                throw new StudySessionAccessDeniedException(command.SessionId, command.UserId);

            var pausedAtUtc = _dateTimeProvider.UtcNow;
            session.Pause(pausedAtUtc);

            await _studySessionRepository.SaveChangesAsync(cancellationToken);

            return new PauseStudySessionResult(
                session.Id,
                session.UserId,
                pausedAtUtc,
                session.Status.DisplayStatus
                );
        }
    }
}
