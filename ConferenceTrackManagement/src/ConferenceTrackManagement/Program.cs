using System;
using System.IO;
using ConferenceTrackManagement.Entity;
using ConferenceTrackManagement.Implement;

namespace ConferenceTrackManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFile = Path.Combine(Environment.CurrentDirectory, "tracks.txt");
            var outputFile = Path.Combine(Environment.CurrentDirectory, "output.txt");

            /* 
                ConferenceManager is a high-level component to arrange and print schedules.You can load activities 
                from any type impments IActivitySource. TextFileActivitySource is default implemention what I provided
                in this app. To make different outputs with any type inherits from SchedulePrinterBase. TextFileSchedulePrinter 
                and TerminalSchedulePrinter are available in this app.
            */
            var conferenceManager = new ConferenceManager(
                new TextFileActivitySource(inputFile),
                new TextFileSchedulePrinter(outputFile)
            );
            
            //Build a 2 days schedule plan and then to arrange activities from IActivitySource.
            var schedules = ConferenceSchedule.Days(2);
            conferenceManager.Arrange(schedules);
            conferenceManager.Print(schedules);
        }
    }
}
