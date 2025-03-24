using System;

namespace jfYu.Core.jfYuRequest
{
    public class LogFilter
    {
        public Func<string, string> RequestFunc { get; set; } = q => { return q; };
        public Func<string, string> ResponseFunc { get; set; } = q => { return q; };
    }
}
