namespace jfYu.Core.RabbitMQ
{
    public class RabbitMQEndPoint
    {
        /// <summary>
        /// MQ主机地址
        /// </summary>
        public string HostName
        {
            get; set;
        }
        /// <summary>
        /// MQ主机端口
        /// </summary>
        public int Port
        {
            get; set;
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get; set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get; set;
        }
        /// <summary>
        /// 请求心跳
        /// </summary>
        public ushort HeartBeat
        {
            get; set;
        }

        /// <summary>
        /// 虚拟消息服务器
        /// </summary>
        public string VirtualHost
        {
            get; set;
        }
    }
}
