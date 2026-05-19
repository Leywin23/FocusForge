using ApplicationStudyService.StudySessions.Commands.PauseStudySession;
using ApplicationStudyService.StudySessions.Exceptions;
using ApplicationStudyService.Tests.Fakes;
using DomainStudyService.Entities;
using DomainStudyService.Statuses;

namespace ApplicationStudyService.Tests.StudySessions.Commands.PauseStudySession
{
    public class PauseStudySessionHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenSessionExists_PausesSession()
        {
            var userId = Guid.NewGuid();
            var startTime = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var pausedAtUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);


            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = pausedAtUtc };

            var session = StudySession.Start(userId, "Math", startTime);
            repository.AddExistingSession(session);

            var handler = new PauseStudySessionHandler(repository, dateTimeProvider);
            var command = new PauseStudySessionCommand(userId, session.Id);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(session.Id, result.SessionId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(pausedAtUtc, result.PausedAtUtc);
            Assert.Equal("Paused", result.Status);
            Assert.IsType<PausedStatus>(session.Status);

            Assert.True(repository.SaveChangesCalled);
        }

        [Fact]
        public async Task HandleAsync_WhenSessionDoesNotExist_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var pausedAtUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = pausedAtUtc };

            var handler = new PauseStudySessionHandler(repository, dateTimeProvider);
            var command = new PauseStudySessionCommand(userId, sessionId);

            await Assert.ThrowsAsync<StudySessionNotFoundException>(
                () => handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WhenStudySessionDoesntBelongToUser_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var startTime = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);
            var pausedAtUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = pausedAtUtc };

            var session = StudySession.Start(anotherUserId, "Math", startTime);
            repository.AddExistingSession(session);

            var handler = new PauseStudySessionHandler(repository, dateTimeProvider);
            var command = new PauseStudySessionCommand(userId, session.Id);

            await Assert.ThrowsAsync<StudySessionAccessDeniedException>(
                () => handler.HandleAsync(command, CancellationToken.None));
        }
    }
}