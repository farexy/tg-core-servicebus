using System;

namespace TG.Core.ServiceBus.Messages
{
    public class UserDeletedMessage
    {
        public Guid Id { get; set; }

        public string Login { get; set; } = default!;
        
        public int[] Roles { get; set; } = default!;
    }
}