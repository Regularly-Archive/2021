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

        public bool IsValid()
        {
            var assignedMinutes = 0;
            if (Morning != null)
                assignedMinutes += (int)Morning.AssignedMinutes;
            if (Afternoon != null)
                assignedMinutes += (int)Afternoon.AssignedMinutes;
            return assignedMinutes > 0;
        }

        public void ClearActivies()
        {
            if (Morning != null)
                Morning.ClearActivies();
            if (Afternoon != null)
                Afternoon.ClearActivies();
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