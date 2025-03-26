using System;

namespace jfYu.Core.jfYuRequest.Enum
{
    [Flags]
    public enum JfYuLoggingFields
    {
        None = 0,
        RequestPath = 1,
        RequestMethod = 2,
        RequestHeaders = 4,
        RequestData = 8,       
        ResponseStatus = 16,
        Response = 32,
        All = RequestPath | RequestMethod | RequestHeaders | RequestData | Response | ResponseStatus
    }
}
