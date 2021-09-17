using System;

namespace TG.Core.ServiceBus.Messages
{
    public class NewUserAuthorizationMessage
    {
        public Guid Id { get; set; }

        public string Login { get; set; } = default!;

        public string? Email { get; set; }

        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }

        public int[] Roles { get; set; } = default!;
    }
}