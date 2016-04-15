

using System;
using System.Diagnostics;
using Microsoft.Azure.WebJobs.Host;

namespace OrderProcessingJob
{
    public class ColorConsoleTraceWriter : TraceWriter
    {
        public ColorConsoleTraceWriter(TraceLevel level)
            : base(level)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
            var holdColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(traceEvent.Message);

            Console.ForegroundColor = holdColor;
        }
    }
}
