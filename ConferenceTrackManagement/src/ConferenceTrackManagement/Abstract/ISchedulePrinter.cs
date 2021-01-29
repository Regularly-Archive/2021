namespace ConferenceTrackManagement.Abstract
{
    using ConferenceTrackManagement.Entity;

    public interface ISchedulePrinter
    {
        void PrintSchedule(params ConferenceSchedule[] schedules);
    }
}