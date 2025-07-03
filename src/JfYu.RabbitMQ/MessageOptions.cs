namespace JfYu.RabbitMQ
{
    /// <summary>
    /// Rabbit MQ message retry policy.
    /// </summary>
    public class MessageOptions
    {
        /// <summary>
        /// Indicates whether the dead letter queue is enabled.
        /// </summary>
        public bool EnableDeadQueue { get; set; }

        /// <summary>
        /// Maximum number of retry attempts before sending to the dead letter queue.
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;

        /// <summary>
        /// Name of the dead letter exchange.
        /// </summary>
        public string DeadLetterExchange { get; set; } = "dead_letter_exchange";

        /// <summary>
        /// Name of the dead letter queue.
        /// </summary>
        public string DeadLetterQueue { get; set; } = "dead_letter_queue";

        /// <summary>
        /// Routing key for the dead letter queue.
        /// </summary>
        public string DeadLetterRoutingKey { get; set; } = "dead_letter_routing_key";

        /// <summary>
        /// Delay time before retrying message delivery.
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 5000;

        /// <summary>
        /// Maximum number of outstanding confirms allowed.
        /// </summary>
        public int MaxOutstandingConfirms { get; set; } = 1000;

        /// <summary>
        /// Batch size for publishing messages in bulk.
        /// </summary>
        public int BatchSize { get; set; } = 20;

    }
}