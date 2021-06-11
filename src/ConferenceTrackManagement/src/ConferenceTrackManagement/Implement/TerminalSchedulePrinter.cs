using System;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;

    public class TerminalSchedulePrinter : SchedulePrinterBase
    {
        protected override void RenderSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            var hour = timeSpan.Hours.ToString("00");
            var minute = timeSpan.Minutes.ToString("00");
            if (slot.ShowDuration)
            {
                Console.WriteLine($"{hour}:{minute}{timeSuffix} {slot.Title} {slot.Duration}min");
            }
            else
            {
                Console.WriteLine($"{hour}:{minute}{timeSuffix} {slot.Title}");
            }
        }

        protected override void RenderTrackHeader(ConferenceSchedule schedule, int numOfDay)
        {
            Console.WriteLine($"\nTrack {numOfDay}:");
        }

        protected override void RenderLunchSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            RenderSlot(slot, timeSpan, timeSuffix);
        }

        protected override void RenderNetworkEventSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            RenderSlot(slot, timeSpan, timeSuffix);
        }

        protected override void OnRenderComplete()
        {

        }

        protected override void OnRenderBegin()
        {

        }
    }
}