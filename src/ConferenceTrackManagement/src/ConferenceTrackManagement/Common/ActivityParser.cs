using System;

namespace ConferenceTrackManagement.Common
{
    using ConferenceTrackManagement.Exceptions;
    using ConferenceTrackManagement.Entity;

    public class ActivityParser
    {
        const string NUMBER_STRING = "0123456789";

        public Activity Parse(string text)
        {
            var subject = text;
            var duration = 1M;

            var timeUnit = ParseTimeUnitOfSubject(text);
            
            var indexOfNum = text.IndexOfAny(NUMBER_STRING.ToCharArray());
            if (indexOfNum != -1)
            {
                subject = text.Substring(0, indexOfNum).Trim();
                var indexOfUnit = text.LastIndexOf(timeUnit.ToString(), StringComparison.OrdinalIgnoreCase);
                duration = decimal.Parse(text.Substring(indexOfNum, indexOfUnit - indexOfNum));
            }

            return new Activity(subject, new ActivityDuration(timeUnit, duration));
        }

        private TimeUnit ParseTimeUnitOfSubject(string text)
        {
            foreach (var unit in Enum.GetValues(typeof(TimeUnit)))
            {
                if (text.IndexOf(unit.ToString(), StringComparison.OrdinalIgnoreCase) > -1)
                    return (TimeUnit)Enum.Parse(typeof(TimeUnit), unit.ToString());
            }

            throw new AcitivityParseException($"Unsupported text pattern: {text}");
        }
    }
}