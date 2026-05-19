using DomainStudyService.Entities;
using DomainStudyService.Statuses;
using DomainStudyService.Exceptions;

namespace DomainStudyService.Tests
{
    public class StudySessionTests
    {
        [Fact]
        public void Start_ShouldCreateActiveSession()
        {
            var userId = Guid.NewGuid();
            var topic = "ASP.NET";
            var nowUtc = DateTime.UtcNow;

            var session = StudySession.Start(userId, topic, nowUtc);

            Assert.NotEqual(Guid.Empty, session.Id);
            Assert.Equal(userId, session.UserId);
            Assert.Equal(topic, session.Title);
            Assert.Equal(nowUtc, session.StartTime);
            Assert.Null(session.EndTime);
            Assert.Equal(0, session.TotalPausedSeconds);
            Assert.IsType<ActiveStatus>(session.Status);
            Assert.Empty(session.Pauses);
        }


        [Fact]
        public void Pause_ShouldChangeStatusToPaused()
        {
            var session = CreateActiveSession();

            session.Pause(DateTime.UtcNow);

            Assert.IsType<PausedStatus>(session.Status);
        }

        [Fact]
        public void Pause_ShouldAddOpenPause()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;

            session.Pause(pauseTime);

            var pause = Assert.Single(session.Pauses);

            Assert.Equal(pauseTime, pause.StartedAtUtc);
            Assert.Null(pause.EndedAtUtc);
        }

        [Fact]
        public void Resume_ShouldChangeStatusToActive()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;
            var resumeTime = pauseTime.AddMinutes(5);

            session.Pause(pauseTime);

            session.Resume(resumeTime);

            Assert.IsType<ActiveStatus>(session.Status);
        }

        [Fact]
        public void Resume_ShouldCloseCurrentPause()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;
            var resumeTime = pauseTime.AddMinutes(5);

            session.Pause(pauseTime);

            session.Resume(resumeTime);

            var pause = Assert.Single(session.Pauses);
            Assert.Equal(resumeTime, pause.EndedAtUtc);
        }

        [Fact]
        public void Resume_ShouldIncreaseTotalPausedSeconds()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;
            var resumeTime = pauseTime.AddSeconds(90);

            session.Pause(pauseTime);

            session.Resume(resumeTime);

            Assert.Equal(90, session.TotalPausedSeconds);
        }

        [Fact]
        public void Finish_ShouldChangeStatusToCompleted()
        {
            var session = CreateActiveSession();
            var finishTime = DateTime.UtcNow.AddMinutes(30);

            session.Finish(finishTime);

            Assert.IsType<CompletedStatus>(session.Status);
        }

        [Fact]
        public void Finish_ShouldSetEndTime()
        {
            var session = CreateActiveSession();
            var finishTime = DateTime.UtcNow.AddMinutes(30);

            session.Finish(finishTime);

            Assert.Equal(finishTime, session.EndTime);
        }

        [Fact]
        public void Finish_ShouldResumePausedSessionBeforeCompleting()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;
            var finishTime = pauseTime.AddSeconds(120);

            session.Pause(pauseTime);

            session.Finish(finishTime);

            var pause = Assert.Single(session.Pauses);

            Assert.IsType<CompletedStatus>(session.Status);
            Assert.Equal(finishTime, session.EndTime);
            Assert.Equal(finishTime, pause.EndedAtUtc);
            Assert.Equal(120, session.TotalPausedSeconds);
        }

        [Fact]
        public void Pause_ShouldThrow_WhenSessionIsNotActive()
        {
            var session = CreatePausedSession();

            Assert.Throws<InvalidSessionStateException>(() => session.Pause(DateTime.UtcNow));
        }

        [Fact]
        public void Resume_ShouldThrow_WhenSessionIsNotPaused()
        {
            var session = CreateActiveSession();

            Assert.Throws<InvalidSessionStateException>(() => session.Resume(DateTime.UtcNow));
        }

        [Fact]
        public void Finish_ShouldThrow_WhenSessionAlreadyCompleted()
        {
            var session = CreateActiveSession();
            var finishTime = DateTime.UtcNow;

            session.Finish(finishTime);

            Assert.Throws<SessionAlreadyCompletedException>(() => session.Finish(finishTime.AddMinutes(1)));
        }

        [Fact]
        public void GetStudyDurationSeconds_ShouldReturnDurationWithoutPauses()
        {
            var startTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            var pauseTime = startTime.AddMinutes(5);
            var resumeTime = pauseTime.AddMinutes(5);
            var endTime = resumeTime.AddMinutes(5);

            int totalTime = (int)(endTime - startTime).TotalSeconds;

            var userId = Guid.NewGuid();
            var topic = "ASP.NET";


            var session = StudySession.Start(userId, topic, startTime);
            session.Pause(pauseTime);
            session.Resume(resumeTime);
            session.Finish(endTime);


            Assert.Equal(totalTime - session.TotalPausedSeconds, session.GetStudyDurationSeconds());
        }

        private StudySession CreateActiveSession()
        {
            var userId = Guid.NewGuid();
            var topic = "ASP.NET";
            var nowUtc = DateTime.UtcNow;

            return StudySession.Start(userId, topic, nowUtc);
        }

        private StudySession CreatePausedSession()
        {
            var session = CreateActiveSession();
            var pauseTime = DateTime.UtcNow;
            session.Pause(pauseTime);

            return session;
        }
    }
}
