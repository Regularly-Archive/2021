using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;
    using ConferenceTrackManagement.Abstract;

    public class SchedulePrinterBase : ISchedulePrinter
    {
        public void PrintSchedule(params ConferenceSchedule[] schedules)
        {
            OnRenderBegin();

            for (var i = 0; i < schedules.Length; i++)
            {
                //Header
                RenderTrackHeader(schedules[i], (i + 1));

                //Morning
                RenderConferencePhase(schedules[i].Morning, 9 * 60, "AM");

                //Lunch
                RenderLunchSlot(new LunchSlot(), new TimeSpan(12, 0, 0), "PM");

                //Afternoon
                RenderConferencePhase(schedules[i].Afternoon, 1 * 60, "PM");

                //Network Event
                RenderNetworkEventSlot(new NerworkEventSlot(), new TimeSpan(17, 0, 0), "PM");
            }

            OnRenderComplete();
        }

        protected void RenderConferencePhase(ConferencePhase phase, int minutes, string timeSuffix)
        {
            foreach (var slot in phase.Slots)
            {
                var hour = minutes / 60;
                var minute = minutes % 60;
                var timeSpan = new TimeSpan(hour, minute, 0);
                RenderSlot(slot, timeSpan, timeSuffix);
                minutes += (int)slot.Duration;
            }
        }

        protected virtual void RenderSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {

        }

        protected virtual void RenderTrackHeader(ConferenceSchedule schedule, int numOfDay)
        {

        }

        protected virtual void RenderLunchSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {

        }

        protected virtual void RenderNetworkEventSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {

        }

        protected virtual void OnRenderComplete()
        {

        }
        protected virtual void OnRenderBegin()
        {

        }
    }
}