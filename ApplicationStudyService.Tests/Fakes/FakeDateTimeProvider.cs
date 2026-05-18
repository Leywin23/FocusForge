using ApplicationStudyService.Common.Interfaces;

namespace ApplicationStudyService.Tests.Fakes
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; set; } = DateTime.UtcNow;
    }
}