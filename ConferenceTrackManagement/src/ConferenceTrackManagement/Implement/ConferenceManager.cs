using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Abstract;

    public class ConferenceManager : IConferenceManager
    {
        private readonly IActivitySource _activitySource;

        private readonly ISchedulePrinter _schedulePrinter;

        public ConferenceManager(IActivitySource activitySource, ISchedulePrinter schedulePrinter)
        {
            _activitySource = activitySource ?? throw new ArgumentNullException(nameof(activitySource));
            _schedulePrinter = schedulePrinter ?? throw new ArgumentNullException(nameof(schedulePrinter));
        }

        public void Arrange(IEnumerable<ConferenceSchedule> schedulesPlans, IComparer<Activity> comparer = null)
        {
            var activities = _activitySource.GetActivities().ToList();
            if (comparer == null)
                activities = activities.OrderByDescending(x => (int)x.Duration.Unit * x.Duration.Value).ToList();
            else
                activities.Sort(comparer);

            
            foreach (var schedulePlan in schedulesPlans)
            {
                //Morning
                ArrangeConferencePhase(schedulePlan.Morning, activities);
                //Afternoon
                ArrangeConferencePhase(schedulePlan.Afternoon, activities);
            }
        }

        public void Print(IEnumerable<ConferenceSchedule> schedulesPlans)
        {
            _schedulePrinter.PrintSchedule(schedulesPlans.ToArray());
        }

        private void ArrangeConferencePhase(ConferencePhase phase, List<Activity> activities)
        {
            while (phase.RemainMinutes > 0)
            {
                var activity = activities.FirstOrDefault();
                if (!phase.IsEnoughToAddActivity(activity))
                {
                    activity = activities.FirstOrDefault(x => x.GetDuration() < phase.RemainMinutes);
                }
                if (activity != null)
                {
                    phase.AddActivity(activity);
                    activities.Remove(activity);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
