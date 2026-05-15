using DomainStudyService.Entities;

namespace DomainStudyService.Tests
{
    public class PausePeriodTests
    {
        [Fact]
        public void Start_ShouldCreateOpenPause()
        {
            var startTime = DateTime.UtcNow;
            var pause = PausePeriod.Start(startTime);

            Assert.Equal(startTime, pause.StartedAtUtc);
            Assert.Null(pause.EndedAtUtc);
        }

        [Fact]
        public void End_ShouldSetEndedAtUtc()
        {
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddMinutes(5);
            var pause = PausePeriod.Start(startTime);

            pause.End(endTime);

            Assert.Equal(endTime, pause.EndedAtUtc);
        }

        [Fact]
        public void End_ShouldThrow_WhenPauseAlreadyEnded()
        {
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddMinutes(5);
            var pause = PausePeriod.Start(startTime);

            pause.End(endTime);

            Assert.Throws<InvalidOperationException>(() => pause.End(endTime.AddSeconds(1)));
        }

        [Fact]
        public void End_ShouldThrow_WhenEndTimeIsBeforeStartTime()
        {
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddMinutes(-5);
            var pause = PausePeriod.Start(startTime);

            Assert.Throws<InvalidOperationException>(() => pause.End(endTime));
        }

        [Fact]
        public void GetDurationSeconds_ShouldReturnDuration()
        {
            var startTime = DateTime.UtcNow;
            var endTime = startTime.AddSeconds(75);

            var pause = PausePeriod.Start(startTime);
            pause.End(endTime);

            Assert.Equal(75, pause.GetDurationSeconds());
        }
    }
}
