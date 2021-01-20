using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Abstract;

    public class TextFileActivitySource : IActivitySource
    {
        private readonly string _filePath;

        public TextFileActivitySource(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            _filePath = filePath;
        }

        public IEnumerable<Activity> GetActivities()
        {
            foreach (var line in File.ReadAllLines(_filePath))
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                var trimed = line.Trim();

                if (string.IsNullOrEmpty(trimed))
                    continue; 

                var activity = ParseActivity(trimed);
                yield return activity;
            }
        }

        private TimeUnit ParseUnitOfSubject(string line)
        {
            foreach (var unit in Enum.GetValues(typeof(TimeUnit)))
            {
                if (line.IndexOf(unit.ToString(), StringComparison.OrdinalIgnoreCase) > -1)
                    return (TimeUnit)Enum.Parse(typeof(TimeUnit), unit.ToString());
            }

            return TimeUnit.Min;
        }

        private Activity ParseActivity(string text)
        {
            var subject = text;
            var duration = 1M;

            var timeUnit = ParseUnitOfSubject(text);
            
            var numberIndex = -1;
            var match = Regex.Match(text, @"(\d+){2}");
            if (match.Success)
                numberIndex = match.Index;

            if (numberIndex != -1)
            {
                subject = text.Substring(0, numberIndex).Trim();
                var lastIndex = text.LastIndexOf(timeUnit.ToString(), StringComparison.OrdinalIgnoreCase);
                duration = decimal.Parse(text.Substring(numberIndex, lastIndex - numberIndex));
            }

            return new Activity(subject, new ActivityDuration(timeUnit, duration));
        }
    }
}