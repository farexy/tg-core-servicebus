using System;
using System.Collections.Generic;

namespace TG.Core.ServiceBus.Messages
{
    public class PushNotificationMessage
    {
        public Guid EventId { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid? DeviceId { get; set; }
        
        public Dictionary<string, object>? Payload { get; set; }
    }
}