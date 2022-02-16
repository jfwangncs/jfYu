
### 验证码

```
Install-Package jfYu.Core.Captcha
```
IOC依赖注入获取对象

```
var containerBuilder = new ContainerBuilder();
var builder = new ConfigurationBuilder().AddConfigurationFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Build();
containerBuilder.AddCaptcha(); //使用默认配置
CaptchaConfig captchaConfig = new CaptchaConfig();
captchaConfig.Length = 6;
captchaConfig.Width = 360;
captchaConfig.Height = 120;
containerBuilder.AddCaptcha(new CaptchaConfig()); //自定义配置
```


使用

```
//通过IOC或者直接new出对象
 var Captcha = New Captcha();
 var result=Captcha.GetCaptcha();
//保存图片到本地 或者 到内存进行显示
Stream s = new MemoryStream(result.CaptchaByteData);
byte[] srcBuf = new Byte[s.Length];
s.Read(srcBuf, 0, srcBuf.Length);
s.Seek(0, SeekOrigin.Begin);
FileStream fs = new FileStream($"d:/{captchaCode}.png", FileMode.Create, FileAccess.Write);
fs.Write(srcBuf, 0, srcBuf.Length);
fs.Close();
```
