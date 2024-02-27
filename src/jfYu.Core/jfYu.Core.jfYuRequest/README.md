

```
Install-Package jfYu.Core.jfYuRequest 

```


```
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var jfYu1 = new JfYuHttpClient()
{
	Url = "www.xxx.com",
	Method = RequestMethod.Get,
	Timeout = 60,
	RequestEncoding = Encoding.GetEncoding("GB18030"),
};
var html1 = await jfYu1.SendAsync(); 

var jfYu = new JfYuHttpRequest()
{
	Url = "www.xxx.com",
	Method = RequestMethod.Get,
	Timeout = 60,
	RequestEncoding = Encoding.GetEncoding("GB18030"),
};
var html = await jfYu.SendAsync(); 

var jfYu = new JfYuHttpRequest()
{
	Url = "https://www.xxx.com/1.jpg"
};

var flag = await jfYu.DownloadFileAsync("requestest/2.jpg"); 
var stream = await jfYu.DownloadFileAsync();
FileStream fs = File.Create("requestest/3.jpg");
await fs.WriteAsync(stream?.ToArray());
fs.Flush();
fs.Close();
```
