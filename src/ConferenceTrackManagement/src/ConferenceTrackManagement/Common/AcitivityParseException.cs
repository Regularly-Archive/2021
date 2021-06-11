using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace ConferenceTrackManagement.Common
{
    using ConferenceTrackManagement.Entity;

    public class AcitivityParseException : Exception
    {
        public AcitivityParseException(string message) : base(message)
        {

        }
    }
}