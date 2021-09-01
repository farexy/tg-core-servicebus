using System;

namespace TG.Core.ServiceBus.Extensions
{
    internal static class ResourcePathExtensions
    {
        internal static string GetQueueEntityPath(this Type messageType) => messageType.Name + "-queue";
        internal static string GetTopicEntityPath(this Type messageType) => messageType.Name + "-topic";
        internal static string GetSubscriptionEntityPath(this Type messageType, string serviceName) => $"{serviceName}{messageType}-subscription";
    }
}
