using System;

namespace jfYu.Core.RabbitMQ
{
    public class MessageRetryPolicy
    {

        /// <summary>
        /// Indicates whether the dead letter queue is enabled.
        /// </summary>
        public bool EnableDeadQueue { get; set; } = false;

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
        /// Delay time before retrying message delivery.
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 5000;
    }
}
