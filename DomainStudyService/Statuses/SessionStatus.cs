using System;
using System.Collections.Generic;
using System.Text;

namespace DomainStudyService.Statuses
{
    public abstract class SessionStatus
    {
        public abstract string DisplayStatus { get; }
    }

    public class ActiveStatus : SessionStatus
    {
        public override string DisplayStatus => "Active";
    }

    public class PausedStatus : SessionStatus
    {
        public override string DisplayStatus => "Paused";
    }


    public class CompletedStatus : SessionStatus
    {
        public override string DisplayStatus => "Completed";
    }
}
