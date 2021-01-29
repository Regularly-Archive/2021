using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Moq;

namespace ConferenceTrackManagement.Test
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Common;
    using ConferenceTrackManagement.Exceptions;
    using ConferenceTrackManagement.Abstract;
    using ConferenceTrackManagement.Implement;

    public class ConferenceManagerTest
    {
        [Fact]
        public void TestArrageActivitiesMorningOnly()
        {
            //Arrange 
            var mock = new Mock<IActivitySource>();
            mock.Setup(x => x.GetActivities()).Returns(new List<Activity>
            {
                new Activity("Activity A", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity B", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity C", new ActivityDuration(TimeUnit.Min, 45)),
                new Activity("Activity D", new ActivityDuration(TimeUnit.Min, 30)),
            });
            var activitySource = mock.Object;
            var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");
            var conferenceManager = new ConferenceManager(
                activitySource,
                new TextFileSchedulePrinter(outputFile)
            );

            //Act
            var schedules = new ConferenceSchedule[] {
                new ConferenceSchedule(new ConferencePhase(180), null)
            };
            conferenceManager.Arrange(schedules);
            conferenceManager.Print(schedules);

            //Assert
            Assert.True(schedules.All(x => x.IsValid()));
            Assert.True(schedules[0].Morning.Slots.Count == 3);
            Assert.True(File.Exists(outputFile));
        }

        [Fact]
        public void TestArrageActivitiesAfternoonOnly()
        {
            //Arrange 
            var mock = new Mock<IActivitySource>();
            mock.Setup(x => x.GetActivities()).Returns(new List<Activity>
            {
                new Activity("Activity A", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity B", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity C", new ActivityDuration(TimeUnit.Min, 45)),
                new Activity("Activity D", new ActivityDuration(TimeUnit.Min, 30)),
            });
            var activitySource = mock.Object;
            var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");
            var conferenceManager = new ConferenceManager(
                activitySource,
                new TextFileSchedulePrinter(outputFile)
            );

            //Act
            var schedules = new ConferenceSchedule[] {
                new ConferenceSchedule(null, new ConferencePhase(240))
            };
            conferenceManager.Arrange(schedules);
            conferenceManager.Print(schedules);

            //Assert
            Assert.True(schedules.All(x => x.IsValid()));
            Assert.True(schedules[0].Afternoon.Slots.Count == 4);
            Assert.True(File.Exists(outputFile));
        }

        [Fact]
        public void TestLackOfAcitivities()
        {
            //Arrange 
            var mock = new Mock<IActivitySource>();
            mock.Setup(x => x.GetActivities()).Returns(new List<Activity>());
            var activitySource = mock.Object;
            var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");
            var conferenceManager = new ConferenceManager(
                activitySource,
                new TextFileSchedulePrinter(outputFile)
            );

            //Act & Assert
            var schedules = new ConferenceSchedule[] {
                new ConferenceSchedule(new ConferencePhase(180), new ConferencePhase(240))
            };
            Assert.Throws<LackOfActivitiesException>(() => conferenceManager.Arrange(schedules));
        }

        [Fact]
        public void TestArrageWithCustomComparer()
        {
            //Arrange 
            var mock = new Mock<IActivitySource>();
            mock.Setup(x => x.GetActivities()).Returns(new List<Activity>
            {
                new Activity("Activity A", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity B", new ActivityDuration(TimeUnit.Min, 60)),
                new Activity("Activity C", new ActivityDuration(TimeUnit.Min, 45)),
                new Activity("Activity D", new ActivityDuration(TimeUnit.Min, 30)),
            });
            var activitySource = mock.Object;
            var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");
            var conferenceManager = new ConferenceManager(
                activitySource,
                new TextFileSchedulePrinter(outputFile)
            );

            //Act & Assert
            var schedules = new ConferenceSchedule[] {
                new ConferenceSchedule(new ConferencePhase(180), new ConferencePhase(240))
            };
            conferenceManager.Arrange(schedules, new LongTermFirstComparer());
            Assert.True(schedules[0].Morning.Slots[0].Duration == 60M);

            conferenceManager.Arrange(schedules, new ShortTermFirstComparer());
            Assert.True(schedules[0].Morning.Slots[0].Duration == 30M);

            conferenceManager.Arrange(schedules, new ASCIIFirstComparer());
            Assert.True(schedules[0].Morning.Slots[0].Title == "Activity D");
        }

        public class LongTermFirstComparer : IComparer<Activity>
        {
            public int Compare(Activity x, Activity y)
            {
                return (int)(x.GetDuration() - y.GetDuration()) * -1;
            }
        }

        public class ShortTermFirstComparer : IComparer<Activity>
        {
            public int Compare(Activity x, Activity y)
            {
                return (int)(x.GetDuration() - y.GetDuration());
            }
        }

        public class ASCIIFirstComparer:IComparer<Activity>
        {
            public int Compare(Activity x, Activity y)
            {
                return -1 * x.Subject.CompareTo(y.Subject);
            }
        }
    }
}
