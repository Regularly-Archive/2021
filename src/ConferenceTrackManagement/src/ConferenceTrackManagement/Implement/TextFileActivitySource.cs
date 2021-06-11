using System;
using System.IO;
using System.Collections.Generic;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Abstract;
    using ConferenceTrackManagement.Common;

    public class TextFileActivitySource : IActivitySource
    {
        private readonly string _filePath;
        private readonly ActivityParser _parser = new ActivityParser();

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

                yield return _parser.Parse(trimed);
            }
        }
    }
}