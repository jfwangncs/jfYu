namespace jfYu.Core.RabbitMQ
{
    public class RabbitMQConfig
    {
        /// <summary>
        /// Host
        /// </summary>
        public string HostName { get; set; } = "";

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = 5673;

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; } = "";

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// HeartBeat default:60s
        /// </summary>
        public ushort HeartBeat { get; set; } = 60;

        /// <summary>
        /// VirtualHost
        /// </summary>
        public string VirtualHost { get; set; } = "/";
    }
}
