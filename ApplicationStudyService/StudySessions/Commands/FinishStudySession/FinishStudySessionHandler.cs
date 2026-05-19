using ApplicationStudyService.Common.Interfaces;
using ApplicationStudyService.StudySessions.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.StudySessions.Commands.FinishStudySession
{
    public class FinishStudySessionHandler
    {
        private readonly IStudySessionRepository _studySessionRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FinishStudySessionHandler(IStudySessionRepository studySessionRepository, IDateTimeProvider dateTimeProvider)
        {
            _studySessionRepository = studySessionRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FinishStudySessionResult> HandleAsync(FinishStudySessionCommand command, CancellationToken cancellationToken)
        {
            var session = await _studySessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
            if (session == null)
                throw new StudySessionNotFoundException(command.SessionId);

            if (session.UserId != command.UserId)
                throw new StudySessionAccessDeniedException(command.SessionId, command.UserId);

            var finishedAtUtc = _dateTimeProvider.UtcNow;
            session.Finish(finishedAtUtc);

            await _studySessionRepository.SaveChangesAsync(cancellationToken);

            return new FinishStudySessionResult(
                session.Id,
                session.UserId,
                finishedAtUtc,
                session.GetStudyDurationSeconds(),
                session.TotalPausedSeconds,
                session.Status.DisplayStatus
                );
        }
    }
}
