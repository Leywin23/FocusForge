using DomainStudyService.Exceptions;
using DomainStudyService.Statuses;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace DomainStudyService.Entities
{
    public class StudySession
    {
        private readonly List<PausePeriod> _pauses = new();

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int TotalPausedSeconds { get; private set; }
        public SessionStatus Status { get; private set; } = null!;

        public IReadOnlyCollection<PausePeriod> Pauses => _pauses.AsReadOnly();

        private StudySession() { }

        public static StudySession Start(Guid userId, string topic, DateTime nowUtc)
        {
            return new StudySession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = topic,
                StartTime = nowUtc,
                Status = new ActiveStatus()
            };
        }
        
        public void Pause(DateTime nowUtc)
        {
            EnsureIsActive();
            _pauses.Add(PausePeriod.Start(nowUtc));
            Status = new PausedStatus();
        }

        public void Resume(DateTime nowUtc)
        {
            EnsureIsPaused();

            var pause = GetCurrentPause();
            pause.End(nowUtc);
            TotalPausedSeconds += pause.GetDurationSeconds();
            Status = new ActiveStatus();
        }

        public void Finish(DateTime nowUtc)
        {
            EnsureIsNotCompleted();

            if (Status is PausedStatus)
                Resume(nowUtc);

            EndTime = nowUtc;
            Status = new CompletedStatus();
        }

        private void EnsureIsActive()
        {
            if (Status is not ActiveStatus)
                throw new InvalidSessionStateException("Only active session can be paused.");
        }

        private void EnsureIsPaused()
        {
            if (Status is not PausedStatus)
                throw new InvalidSessionStateException("Only paused session can be resumed.");
        }

        private void EnsureIsNotCompleted()
        {
            if (Status is CompletedStatus)
                throw new SessionAlreadyCompletedException("Session is already completed.");
        }

        private PausePeriod GetCurrentPause()
        {
            return _pauses.LastOrDefault(x => x.EndedAtUtc is null)
                   ?? throw new OpenPauseNotFoundException("Active pause was not found.");
        }
    }
}
