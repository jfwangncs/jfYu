using System;

namespace jfYu.Core.jfYuRequest
{
    public class LogFilter
    {
        public Func<string, string>? RequestFunc { get; set; }
        public Func<string, string>? ResponseFunc { get; set; }
    }
}
