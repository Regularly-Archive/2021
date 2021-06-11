using System;

namespace ConferenceTrackManagement.Exceptions
{
    public class LackOfActivitiesException : Exception
    {
        public LackOfActivitiesException(string message) : base(message)
        {

        }
    }
}