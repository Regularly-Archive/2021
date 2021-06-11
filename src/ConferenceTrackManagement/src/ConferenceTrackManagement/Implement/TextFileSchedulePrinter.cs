using System;
using System.IO;
using System.Text;

namespace ConferenceTrackManagement.Implement
{
    using ConferenceTrackManagement.Entity;

    public class TextFileSchedulePrinter : SchedulePrinterBase
    {
        private readonly string _filePath;
        private readonly StringBuilder _builder  = new StringBuilder();
        public TextFileSchedulePrinter(string filePath)
        {
            _filePath = filePath;
        }
        protected override void RenderTrackHeader(ConferenceSchedule schedule, int numOfDay)
        {
            _builder.Append($"\r\nTrack {numOfDay}:\r\n");
        }

        protected override void RenderNetworkEventSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            RenderSlot(slot, timeSpan, timeSuffix);
        }

        protected override void RenderLunchSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            RenderSlot(slot, timeSpan, timeSuffix);
        }

        protected override void RenderSlot(ConferenceSlot slot, TimeSpan timeSpan, string timeSuffix)
        {
            var hour = timeSpan.Hours.ToString("00");
            var minute = timeSpan.Minutes.ToString("00");
            if (slot.ShowDuration)
            {
                _builder.Append($"{hour}:{minute}{timeSuffix} {slot.Title} {slot.Duration}min\r\n");
            }
            else
            {
                _builder.Append($"{hour}:{minute}{timeSuffix} {slot.Title}\r\n");
            }
        }

        protected override void OnRenderComplete()
        {
            using(var streamWriter = new StreamWriter(_filePath, false))
                streamWriter.WriteLine(_builder.ToString());
        }
    }
}