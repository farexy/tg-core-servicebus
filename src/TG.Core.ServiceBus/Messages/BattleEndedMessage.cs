using System;

namespace TG.Core.ServiceBus.Messages
{
    public enum BattleEndReason
    {
        None = 0,
        Finished = 1,
        Crash = 2
    }

    public class BattleEndedMessage
    {
        public Guid BattleId { get; set; }
        
        public BattleEndReason Reason { get; set; }
    }
}