using ApplicationStudyService.StudySessions.Commands.StartStudySession;
using ApplicationStudyService.StudySessions.Exceptions;
using ApplicationStudyService.Tests.Fakes;
using DomainStudyService.Entities;

namespace ApplicationStudyService.Tests.StudySessions.Commands.StartStudySession
{
    public class StartStudySessionHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenUserHasNoActiveSession_CreatesStudySession()
        {
            var userId = Guid.NewGuid();
            var title = "Math";
            var nowUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = nowUtc };

            var handler = new StartStudySessionHandler(repository, dateTimeProvider);
            var command = new StartStudySessionCommand(userId, title);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(userId, result.UserId);
            Assert.Equal(title, result.Title);
            Assert.Equal(nowUtc, result.StartTime);
            Assert.Equal("Active", result.Status);

            Assert.NotNull(repository.AddedSession);
            Assert.True(repository.SaveChangesCalled);

            var addedSession = repository.AddedSession!;

            Assert.Equal(userId, addedSession.UserId);
            Assert.Equal(title, addedSession.Title);
            Assert.Equal(nowUtc, addedSession.StartTime);
        }

        [Fact]
        public async Task HandleAsync_WhenUserAlreadyHasActiveSession_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var title = "Math";
            var nowUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = nowUtc };

            var existingSession = StudySession.Start(userId, "Physics", nowUtc.AddMinutes(-30));
            repository.AddExistingSession(existingSession);

            var handler = new StartStudySessionHandler(repository, dateTimeProvider);
            var command = new StartStudySessionCommand(userId, title);

            await Assert.ThrowsAsync<ActiveStudySessionAlreadyExistsException>(() => handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WhenUserHasPausedSession_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var title = "Math";
            var nowUtc = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var pausedAtUtc = nowUtc.AddMinutes(30);
            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = nowUtc };

            var session = StudySession.Start(userId, title, nowUtc);
            session.Pause(pausedAtUtc);
            repository.AddExistingSession(session);

            var handler = new StartStudySessionHandler(repository, dateTimeProvider);
            var command = new StartStudySessionCommand(userId, title);

            await Assert.ThrowsAsync<ActiveStudySessionAlreadyExistsException>(() => handler.HandleAsync(command, CancellationToken.None));
        }
    }
}