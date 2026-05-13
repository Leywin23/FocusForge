using System;
using System.Collections.Generic;
using System.Text;

namespace DomainStudyService.Entities
{
    public class PausePeriod
    {
        public DateTime StartedAtUtc { get; private set; }
        public DateTime? EndedAtUtc { get; private set; }

        public int GetDurationSeconds()
        {
            if (EndedAtUtc is null)
                throw new InvalidOperationException("Pause is not ended yet.");
            return (int)(EndedAtUtc.Value - StartedAtUtc).TotalSeconds;
        }

        public static PausePeriod Start(DateTime nowUtc)
        {
            return new PausePeriod
            {
                StartedAtUtc = nowUtc
            };
        }

        public void End(DateTime nowUtc)
        {
            if (EndedAtUtc is not null)
                throw new InvalidOperationException("Pause is already ended.");

            if (nowUtc < StartedAtUtc)
                throw new InvalidOperationException("Pause cannot end before it starts.");

            EndedAtUtc = nowUtc;
        }
    }
}
