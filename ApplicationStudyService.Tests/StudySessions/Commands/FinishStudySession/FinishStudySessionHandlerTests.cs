using ApplicationStudyService.StudySessions.Commands.FinishStudySession;
using ApplicationStudyService.StudySessions.Exceptions;
using ApplicationStudyService.Tests.Fakes;
using DomainStudyService.Entities;
using DomainStudyService.Statuses;

namespace ApplicationStudyService.Tests.StudySessions.Commands.FinishStudySession
{
    public class FinishStudySessionHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenSessionExists_FinishesSession()
        {
            var userId = Guid.NewGuid();
            var topic = "Math";
            var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var finishTime = new DateTime(2026, 1, 1, 10, 30, 0, DateTimeKind.Utc);

            int totalStudySeconds = 1800;
            int totalPausedSeconds = 0;

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = finishTime };

            var session = StudySession.Start(userId, topic, startTime);
            repository.AddExistingSession(session);

            var handler = new FinishStudySessionHandler(repository, dateTimeProvider);
            var command = new FinishStudySessionCommand(userId, session.Id);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.Equal(session.Id, result.SessionId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(finishTime, result.FinishedAtUtc);
            Assert.Equal(totalStudySeconds - totalPausedSeconds, result.TotalStudySeconds);
            Assert.Equal("Completed", result.Status);
            Assert.IsType<CompletedStatus>(session.Status);
            Assert.True(repository.SaveChangesCalled);
        }

        [Fact]
        public async Task HandleAsync_WhenSessionDoesNotExist_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = DateTime.UtcNow };

            var handler = new FinishStudySessionHandler(repository, dateTimeProvider);
            var command = new FinishStudySessionCommand(userId, sessionId);

            await Assert.ThrowsAsync<StudySessionNotFoundException>(
                () => handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WhenStudySessionDoesntBelongToUser_ThrowsException()
        {
            var userId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();
            var topic = "Math";
            var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            var session = StudySession.Start(anotherUserId, topic, startTime);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = DateTime.UtcNow };

            var handler = new FinishStudySessionHandler(repository, dateTimeProvider);
            var command = new FinishStudySessionCommand(userId, session.Id);

            repository.AddExistingSession(session);

            await Assert.ThrowsAsync<StudySessionAccessDeniedException>(
                () => handler.HandleAsync(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleAsync_WhenSessionIsPaused_FinishesAndClosesPause()
        {
            var userId = Guid.NewGuid();
            var topic = "Math";
            var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var pauseTime = new DateTime(2026, 1, 1, 10, 10, 0, DateTimeKind.Utc);
            var finishTime = new DateTime(2026, 1, 1, 10, 20, 0, DateTimeKind.Utc);

            var repository = new FakeStudySessionRepository();
            var dateTimeProvider = new FakeDateTimeProvider { UtcNow = finishTime };

            var session = StudySession.Start(userId, topic, startTime);
            session.Pause(pauseTime);
            repository.AddExistingSession(session);

            var handler = new FinishStudySessionHandler(repository, dateTimeProvider);
            var command = new FinishStudySessionCommand(userId, session.Id);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            var pause = Assert.Single(session.Pauses);

            Assert.Equal(session.Id, result.SessionId);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(finishTime, result.FinishedAtUtc);
            Assert.Equal(600, result.TotalPausedSeconds);
            Assert.Equal(600, result.TotalStudySeconds);
            Assert.Equal("Completed", result.Status);
            Assert.IsType<CompletedStatus>(session.Status);
            Assert.Equal(finishTime, pause.EndedAtUtc);
            Assert.True(repository.SaveChangesCalled);
        }
    }
}