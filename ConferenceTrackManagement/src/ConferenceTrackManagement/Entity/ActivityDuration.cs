using System;
using System.Collections;

namespace ConferenceTrackManagement.Entity
{
    public class ActivityDuration
    {
        public TimeUnit Unit  { get; private set; }

        public decimal  Value { get; private set; }

        public ActivityDuration(TimeUnit unit, decimal value)
        {
            if (value <= 0) 
                throw new ArgumentException(nameof(value));
            
            Unit = unit;
            Value = value;
        }
    }
}