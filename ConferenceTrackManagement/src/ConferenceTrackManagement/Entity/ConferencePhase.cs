using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace ConferenceTrackManagement.Entity
{
    public class ConferencePhase
    {
        public decimal TotalMinutes { get; private set; }
        public decimal UsedMinutes => Slots.Sum(x => x.Duration);
        public decimal RemainMinutes => TotalMinutes - UsedMinutes;

        public List<ConferenceSlot> Slots { get; private set; }

        public ConferencePhase(decimal totalMinutes)
        {
            TotalMinutes = totalMinutes;
            Slots = new List<ConferenceSlot>();
        }

        public void AddActivity(Activity activity)
        {
            Slots.Add(new ConferenceSlot(
                activity.Subject, 
                activity.GetDuration(), 
                activity.Duration.Unit == TimeUnit.Min)
            );
        }

        public bool IsEnoughToAddActivity(Activity activity)
        {
            if (activity == null) return false;
            return RemainMinutes >= activity.GetDuration();
        }
    }
}