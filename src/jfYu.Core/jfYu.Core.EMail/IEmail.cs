using System.Threading.Tasks;

namespace jfYu.Core.EMail
{
    public interface IEmail
    {

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人多人由;分隔</param>
        /// <param name="sub">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        void SendMail(string to, string sub, string body);
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人多人由;分隔</param>
        /// <param name="sub">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        Task SendMailAsync(string to, string sub, string body);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人多人由;分隔</param>
        /// <param name="cc">抄送多人由;分隔</param>
        /// <param name="sub">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        void SendMail(string to, string cc, string sub, string body);

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人多人由;分隔</param>
        /// <param name="cc">抄送多人由;分隔</param>
        /// <param name="sub">标题</param>
        /// <param name="body">内容</param>
        /// <returns></returns>
        Task SendMailAsync(string to, string cc, string sub, string body);
    }
}
