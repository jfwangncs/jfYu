

```
Install-Package jfYu.Core.jfYuRequest 

```


```
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

//injection
services.AddJfYuHttpRequestService();
services.AddJfYuHttpClientService(new System.Net.Http.HttpClientHandler() { UseCookies = false });

//send
_client.Url = "";
_client.Method = RequestMethod.Get;
_client.Timeout = 60;
var html1 = await _client.SendAsync(); 

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
