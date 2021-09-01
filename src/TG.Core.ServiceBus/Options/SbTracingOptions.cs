using System;

namespace TG.Core.ServiceBus.Options
{
    public class SbTracingOptions
    {
        public Func<string?>? GetTraceId { get; set; }
        
        public Action<string>? SetTraceId { get; set; }
    }
}