using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace ConferenceTrackManagement.Entity
{
    public class ConferenceSlot
    {
        public string Title { get; private set; }

        public decimal Duration { get; private set; }

        public bool ShowDuration { get; private set; }

        public ConferenceSlot(string title, decimal duration, bool showDuration)
        {
            Title = title;
            Duration = duration;
            ShowDuration = showDuration;
        }
    }

    public class LunchSlot : ConferenceSlot
    {
        public LunchSlot() : base("Lunch", 60, false)
        {

        }
    }

    public class NerworkEventSlot : ConferenceSlot
    {
        public NerworkEventSlot() : base("Networking Event", 00, false)
        {

        }
    }
}