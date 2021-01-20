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
            if (Regex.IsMatch(subject, "[0-9]+$"))
                throw new ArgumentException(nameof(subject));

            Subject = subject;
            Duration = duration;
        }

        public override string ToString()
        {
            return $"{Subject}({Duration.Value * (int)Duration.Unit}Min)";
        }

        public decimal GetDuration() => Duration.Value * (int)Duration.Unit;
    }
}