using ApplicationStudyService.StudySessions.Commands.ResumeStudySession;
using ApplicationStudyService.StudySessions.Exceptions;
using ApplicationStudyService.Tests.Fakes;
using DomainStudyService.Entities;
using DomainStudyService.Statuses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudyService.Tests.StudySessions.Commands.ResumeStudySession
{
    public class ResumeStudySessionHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenSessionExists_ResumesSession()
        {
            var userId = Guid.NewGuid();
            var startTime = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var pauseTime = new DateTime(2026, 1, 1, 9, 30, 0, DateTimeKind.Utc);
            var resumedAtUtc = new DateTime(2026, 1, 1, 9, 35, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = resumedAtUtc };

            var session = StudySession.Start(userId, "Math", startTime);
            session.Pause(pauseTime);
            repository.AddExistingSession(session);

            var handler = new ResumeStudySessionHandler(repository, dateTimeProvider);
            var command = new ResumeStudySessionCommand(userId, session.Id);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(session.Id, result.SessionId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(resumedAtUtc, result.ResumedAtUtc);
            Assert.Equal(300, result.TotalPausedSeconds);
            Assert.Equal("Active", result.Status);
            Assert.IsType<ActiveStatus>(session.Status);
            Assert.True(repository.SaveChangesCalled);
        }

        [Fact]
        public async Task HandleAsync_WhenStudySessionDoesntBelongToUser_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var startTime = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var pauseTime = new DateTime(2026, 1, 1, 9, 30, 0, DateTimeKind.Utc);
            var resumedAtUtc = new DateTime(2026, 1, 1, 9, 35, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = resumedAtUtc };

            var session = StudySession.Start(userId, "Math", startTime);
            session.Pause(pauseTime);
            repository.AddExistingSession(session);

            var handler = new ResumeStudySessionHandler(repository, dateTimeProvider);
            var command = new ResumeStudySessionCommand(userId2, session.Id);

            await Assert.ThrowsAsync<StudySessionAccessDeniedException>(() => handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WhenSessionDoesNotExist_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var resumedAtUtc = new DateTime(2026, 1, 1, 9, 35, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = resumedAtUtc };

            var handler = new ResumeStudySessionHandler(repository, dateTimeProvider);
            var command = new ResumeStudySessionCommand(userId, sessionId);

            await Assert.ThrowsAsync<StudySessionNotFoundException>(
                () => handler.HandleAsync(command, CancellationToken.None));
        }
    }
}
