using System;

namespace TG.Core.ServiceBus.Messages
{
    public class UserCancellationMessage
    {
        public Guid Id { get; set; }

        public string Login { get; set; } = default!;
        
        public int[] Roles { get; set; } = default!;
        
        public UserCancellationType Type { get; set; }
    }

    public enum UserCancellationType
    {
        Deleted = 0,
        Banned = 1,
    }
}