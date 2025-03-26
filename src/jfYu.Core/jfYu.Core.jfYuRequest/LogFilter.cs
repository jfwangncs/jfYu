using jfYu.Core.jfYuRequest.Enum;
using System;

namespace jfYu.Core.jfYuRequest
{
    public class LogFilter
    {
        public JfYuLoggingFields LoggingFields { get; set; } = JfYuLoggingFields.All;

        public Func<string, string> RequestFunc { get; set; } = q => q;
        public Func<string, string> ResponseFunc { get; set; } = q => q;
    }
}
