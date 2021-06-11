using System.Collections.Generic;

namespace ConferenceTrackManagement.Abstract
{
    using ConferenceTrackManagement.Entity;

    public interface IActivitySource
    {
        IEnumerable<Activity> GetActivities();
    }
}