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
    using ConferenceTrackManagement.Common;

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
            var activities = _activitySource.GetActivities()?.ToList();
            if (comparer == null)
                activities = activities.OrderByDescending(x => (int)x.Duration.Unit * x.Duration.Value).ToList();
            else
                activities.Sort(comparer);


            foreach (var schedulePlan in schedulesPlans)
            {
                //Clear Activities 
                schedulePlan.ClearActivies();

                //Arrange Morning
                ArrangeConferencePhase(schedulePlan.Morning, activities);

                //Arramge Afternoon
                ArrangeConferencePhase(schedulePlan.Afternoon, activities);

                //Validate Schedule
                if (!schedulePlan.IsValid())
                    throw new LackOfActivitiesException("Please ensure GetActivities() of IActivitySource returns enough activities to fill schedules");
            }
        }

        public void Print(IEnumerable<ConferenceSchedule> schedulesPlans)
        {
            if (schedulesPlans == null || !schedulesPlans.Any())
                throw new ArgumentNullException(nameof(schedulesPlans));

            _schedulePrinter.PrintSchedule(schedulesPlans.ToArray());
        }

        private void ArrangeConferencePhase(ConferencePhase phase, List<Activity> activities)
        {
            if (phase == null)
                return;
                
            if (activities == null || !activities.Any())
                return;

            while (phase.RemainedMinutes > 0)
            {
                var activity = activities.FirstOrDefault();
                if (!phase.IsEnoughToAddActivity(activity))
                {
                    activity = activities.FirstOrDefault(x => x.GetDuration() < phase.RemainedMinutes);
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
