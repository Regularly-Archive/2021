using System.Collections.Generic;

namespace ConferenceTrackManagement.Abstract
{
    using ConferenceTrackManagement.Entity;

    public interface IConferenceManager
    {
        void Arrange(IEnumerable<ConferenceSchedule> schedulesPlans, IComparer<Activity> comparer = null);
        void Print(IEnumerable<ConferenceSchedule> schedulesPlans);
    }
}