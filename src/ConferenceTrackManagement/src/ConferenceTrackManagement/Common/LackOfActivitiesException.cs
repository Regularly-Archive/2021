using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace ConferenceTrackManagement.Common
{
    using ConferenceTrackManagement.Entity;

    public class LackOfActivitiesException : Exception
    {
        public LackOfActivitiesException(string message) : base(message)
        {

        }
    }
}