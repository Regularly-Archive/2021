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
        public decimal DesignedMinutes { get; private set; }
        public decimal AssignedMinutes => Slots.Sum(x => x.Duration);
        public decimal RemainedMinutes => DesignedMinutes - AssignedMinutes;

        public List<ConferenceSlot> Slots { get; private set; }

        public ConferencePhase(decimal designedMinutes)
        {
            DesignedMinutes = designedMinutes;
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

        public void ClearActivies()
        {
            Slots.Clear();
        }

        public bool IsEnoughToAddActivity(Activity activity)
        {
            if (activity == null) return false;
            return RemainedMinutes >= activity.GetDuration();
        }
    }
}