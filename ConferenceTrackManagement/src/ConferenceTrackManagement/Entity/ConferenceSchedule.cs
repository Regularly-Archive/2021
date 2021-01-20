using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;

namespace ConferenceTrackManagement.Entity
{
    public class ConferenceSchedule
    {
        public ConferencePhase Morning { get; private set; }

        public ConferencePhase Afternoon { get; private set; }

        public ConferenceSchedule(ConferencePhase morning, ConferencePhase afternoon)
        {
            Morning = morning;
            Afternoon = afternoon;
        }

        public static ConferenceSchedule Daily =>
            new ConferenceSchedule(
                new ConferencePhase(180),
                new ConferencePhase(240)
            );

        public static ConferenceSchedule[] Days(int days) =>
            Enumerable.Range(0, days).Select(x => ConferenceSchedule.Daily).ToArray();

    }
}