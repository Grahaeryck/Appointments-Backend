using Serilog;
using System.Runtime.CompilerServices;
using System;
using ILogger = Serilog.ILogger;

namespace Appointment_Backend.Worker
{
    public static class LoggerExtensions
    {
        public static ILogger Log(this ILogger logger,
            [CallerMemberName] string memberName = "")
        {
            return logger
                .ForContext("MemberName", memberName);
        }
    }
}
