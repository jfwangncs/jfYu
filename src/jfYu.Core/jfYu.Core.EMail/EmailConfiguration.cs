namespace jfYu.Core.EMail
{
    public class EmailConfiguration
    {
        /// <summary>
        /// 邮件服务器
        /// </summary>
        public string MailServer { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string MailServerUsername { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string MailServerPassword { get; set; }
        /// <summary>
        /// 发件姓名
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 发件地址
        /// </summary>
        public string SenderEmail { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
