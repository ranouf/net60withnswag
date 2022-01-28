using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SK.Queues
{
    public interface IQueueService<T> where T : class
    {
        string QueueName { get; }
        Task EnsureQueuesExistsAsync();
        Task<T> QueueMessageAsync([NotNull] T message, TimeSpan? visibilityTimeout = null);
    }
}
