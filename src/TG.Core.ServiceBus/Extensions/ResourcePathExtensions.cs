using System;

namespace TG.Core.ServiceBus.Extensions
{
    internal static class ResourcePathExtensions
    {
        internal static string GetQueueEntityPath(this Type messageType) => TrimMessageName(messageType) + "-queue";
        internal static string GetTopicEntityPath(this Type messageType) => TrimMessageName(messageType) + "-topic";
        internal static string GetSubscriptionEntityPath(this Type messageType, string serviceName) => $"{serviceName}{TrimMessageName(messageType)}-subscription";

        private static string TrimMessageName(Type messageType)
        {
            var messageName = messageType.Name;
            return messageName.EndsWith("Message")
                ? messageName.Substring(0, messageName.LastIndexOf('M'))
                : messageName;
        }
    }
}
