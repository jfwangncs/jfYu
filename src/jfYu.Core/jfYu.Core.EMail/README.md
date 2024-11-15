
###  Email工具

```
Install-Package jfYu.Core.Email
```
配置email server
```
{
 "Email": {
    "MailServer": "smtp.aliyun.com",
    "MailServerUsername": "xxx@aliyun.com",
    "MailServerPassword": "xxx",
    "SenderEmail": "xxx@aliyun.com",
    "SenderName": "xxx",
    "AdminSenderEmail": "xxx",
    "Port": 465
  },
}

var builder = new ConfigurationBuilder().AddConfigurationFile("Email.json");
 builder.Build();
var cb = new ContainerBuilder();
cb.AddEmail();           
cb.AddEmail((new EmailConfiguration() { MailServer = "smtp.aliyun.com", MailServerPassword = "", MailServerUsername = "", Port = 465, SenderName = "", SenderEmail = "" )); //自定义
IEmail mail = cb.Build().Resolve<IEmail>();
await mail.SendMailAsync("475760135@qq.com", "x1", "xx1");
mail.SendMail("475760135@qq.com", "617110271@qq.com;wjf@sunbirddata.com.cn", "单TO多CC", "测试邮件");
```
