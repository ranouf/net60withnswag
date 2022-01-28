using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SK.Extensions;
using SK.Queues.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SK.Queues
{
    public abstract class QueueService<T> : IQueueService<T> where T : class
    {
        public static readonly string DEFAULT_POISON_QUEUE_NAME_SUFFIX = "-poison";
        private readonly QueueSettings _queueSettings;
        private readonly ILogger _logger;

        private string _queueName;
        public string QueueName
        {
            get { return _queueName.ToLower(); }
            set { _queueName = value; }
        }

        public QueueService(
            [NotNull] string queueName,
            [NotNull] IOptions<QueueSettings> queueSettings,
            [NotNull] ILogger logger
        )
        {
            _queueSettings = queueSettings.Value;
            _logger = logger;
            QueueName = queueName;
        }

        public QueueService(
            IQueueNameResolver queueNameResolver,
            IOptions<QueueSettings> connectionStringsSettings,
            ILogger logger
        ) : this(queueNameResolver.ResolveQueueName(), connectionStringsSettings, logger)
        {
        }

        public async Task EnsureQueuesExistsAsync()
        {
            await CreateIfNotExistsAsync(QueueName);
            await CreateIfNotExistsAsync(QueueName + DEFAULT_POISON_QUEUE_NAME_SUFFIX);

            async Task CreateIfNotExistsAsync(string queueName)
            {
                var queueClient = GetQueueClient(queueName);
                await queueClient.CreateIfNotExistsAsync();
            }
        }

        public async Task<T> QueueMessageAsync([NotNull] T message, TimeSpan? visibilityTimeout = null)
        {
            var queueClient = GetQueueClient(QueueName);
            await queueClient.SendMessageAsync(
                message.ToJson(),
                visibilityTimeout
            );
            _logger.LogInformation($"A new message has been queued.", message);
            return message;
        }

        #region private
        private QueueClient GetQueueClient(string queueName)
        {
            return new QueueClient(
                _queueSettings.ConnectionString,
                queueName,
                new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                }
            );
        }
        #endregion
    }
}
