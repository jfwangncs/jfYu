using jfYu.Core.Common.Configurations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
#if NETSTANDARD20
using Microsoft.Extensions.Configuration;
#endif
using System.Threading.Tasks;

namespace jfYu.Core.EMail
{

    public class Email : IEmail
    {
        public readonly EmailConfiguration Config = new EmailConfiguration();
        public Email()
        {
            try
            {
                Config = AppConfig.Configuration.GetSection("Email").Get<EmailConfiguration>();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"错误的邮件配置:{ex.Message} - {ex.StackTrace}");

            }

        }
        public Email(EmailConfiguration _config)
        {
            Config = _config;
        }
        public void SendMail(string to, string sub, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Config.SenderName, Config.SenderEmail));
            foreach (var item in to.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailboxAddress(item));
            }
            message.Subject = sub;
            message.Body = new TextPart("html") { Text = body };
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Config.MailServer, Config.Port, true);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Config.MailServerUsername, Config.MailServerPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public async Task SendMailAsync(string to, string sub, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Config.SenderName, Config.SenderEmail));
            foreach (var item in to.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailboxAddress(item));
            }
            message.Subject = sub;
            message.Body = new TextPart("html") { Text = body };
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Config.MailServer, Config.Port, true);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Config.MailServerUsername, Config.MailServerPassword);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }

        public void SendMail(string to, string cc, string sub, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Config.SenderName, Config.SenderEmail));
            foreach (var item in to.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailboxAddress(item));
            }
            foreach (var item in cc.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.Cc.Add(new MailboxAddress(item));
            }
            message.Subject = sub;
            message.Body = new TextPart("html") { Text = body };
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Config.MailServer, Config.Port, true);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Config.MailServerUsername, Config.MailServerPassword);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public async Task SendMailAsync(string to, string cc, string sub, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(Config.SenderName, Config.SenderEmail));
            foreach (var item in to.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.To.Add(new MailboxAddress(item));
            }
            foreach (var item in cc.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                    message.Cc.Add(new MailboxAddress(item));
            }
            message.Subject = sub;
            message.Body = new TextPart("html") { Text = body };
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(Config.MailServer, Config.Port, true);
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Config.MailServerUsername, Config.MailServerPassword);
                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
    }
}
