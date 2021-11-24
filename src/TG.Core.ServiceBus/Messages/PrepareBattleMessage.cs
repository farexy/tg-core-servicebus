using System;

namespace TG.Core.ServiceBus.Messages
{
    public class PrepareBattleMessage
    {
        public Guid BattleId { get; set; }

        public string BattleType { get; set; } = default!;
    }
}