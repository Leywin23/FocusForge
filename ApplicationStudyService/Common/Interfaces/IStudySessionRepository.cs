using DomainStudyService.Entities;

namespace ApplicationStudyService.Common.Interfaces
{
    public interface IStudySessionRepository
    {
        Task AddAsync(StudySession session, CancellationToken cancellationToken);

        Task<StudySession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<StudySession?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}