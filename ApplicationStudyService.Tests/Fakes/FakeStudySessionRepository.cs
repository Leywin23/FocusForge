using ApplicationStudyService.Common.Interfaces;
using DomainStudyService.Entities;
using DomainStudyService.Statuses;

namespace ApplicationStudyService.Tests.Fakes
{
    public class FakeStudySessionRepository : IStudySessionRepository
    {
        public StudySession? AddedSession { get; private set; }
        public bool SaveChangesCalled { get; private set; }
        private readonly List<StudySession> _sessions = new();

        public Task AddAsync(StudySession session, CancellationToken cancellationToken)
        {
            _sessions.Add(session);
            AddedSession = session;

            return Task.CompletedTask;
        }

        public Task<StudySession?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(session);
        }

        public Task<StudySession?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var session = _sessions.FirstOrDefault(s => s.UserId == userId && s.Status is ActiveStatus);
            return Task.FromResult(session);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalled = true;

            return Task.CompletedTask;
        }

        public void AddExistingSession(StudySession session)
        {
            _sessions.Add(session);
        }
    }
}