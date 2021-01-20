using System;
using System.Text.RegularExpressions;
using Xunit;

namespace ConferenceTrackManagement.Test
{
    using ConferenceTrackManagement.Entity;

    public class ActivitySourceTest
    {
        [Fact]
        public void ParseActivityWithMin()
        {
            //Arrange 
            var text = "Common Ruby Errors 45min";

            //Act
            var activity = ParseActivity(text);

            //Assert
            var expected = new Activity("Common Ruby Errors", new ActivityDuration(TimeUnit.Min, 45));
            Assert.Equal(activity.ToString(), expected.ToString());
        }

        [Fact]
        public void ParseActivityWithLightning()
        {
            //Arrange 
            var text = "Rails for Python Developers lightning";

            //Act
            var activity = ParseActivity(text);

            //Assert
            var expected = new Activity("Rails for Python Developers lightning", new ActivityDuration(TimeUnit.Lightning, 1));
            Assert.Equal(activity.ToString(), expected.ToString());
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
