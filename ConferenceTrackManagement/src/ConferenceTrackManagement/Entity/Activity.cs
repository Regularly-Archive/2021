using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace ConferenceTrackManagement.Entity
{
    public class Activity
    {
        public string Subject { get; private set; }

        public ActivityDuration Duration { get; private set; }

        public Activity(string subject, ActivityDuration duration)
        {
            if (subject.IndexOfAny("0123456789".ToCharArray()) > 0)
                throw new ArgumentException(nameof(subject));

            Subject = subject;
            Duration = duration;
        }

        public decimal GetDuration() => Duration.Value * (int)Duration.Unit;
    }
}