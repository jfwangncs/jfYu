using jfYu.Core.jfYuRequest;
using jfYu.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;

namespace jfYu.Core.Test.JfYuRequest
{
    [Collection("Data")]
    public class jfYuHttpRequestTests
    {
        IJfYuRequest client;

        public jfYuHttpRequestTests()
        {
            var services = new ServiceCollection();
            services.AddJfYuHttpRequestService();

            var serviceProvider = services.BuildServiceProvider();
            client = serviceProvider.GetRequiredService<IJfYuRequest>();
        }

        //[Fact]
        //public async Task Test_Get_Success()
        //{
        //    client.Url = "https://httpbin.org/get";
        //    client.Method = HttpMethod.Get;
        //    client.Params.Add("test", "value");

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("test=value", jsonResponse!["url"].ToString());
        //}

        //[Fact]
        //public async Task Test_Get_MultipleParams_Success()
        //{
        //    client.Url = "https://httpbin.org/get?x=y";
        //    client.Method = HttpMethod.Get;
        //    client.Params.Add("test", "value");

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("test=value", jsonResponse!["url"].ToString());
        //    Assert.Contains("x=y", jsonResponse!["url"].ToString());
        //}

        //[Fact]
        //public async Task Test_Post_Success()
        //{
        //    client.Url = "https://httpbin.org/post";
        //    client.Method = HttpMethod.Post;
        //    client.Params.Add("test", "value");
        //    client.ContentType = "application/x-www-form-urlencoded";

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("test", jsonResponse!["form"].ToString());
        //}

        //[Fact]
        //public async Task Test_Put_Success()
        //{
        //    client.Url = "https://httpbin.org/put";
        //    client.Method = HttpMethod.Put;
        //    client.Params.Add("test", "value");

        //    var response = await client.SendAsync();
        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //}

        //[Fact]
        //public async Task Test_Delete_Success()
        //{
        //    client.Url = "https://httpbin.org/delete";
        //    client.Method = HttpMethod.Delete;

        //    var response = await client.SendAsync();
        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //}

        //[Fact]
        //public async Task Test_Status_404()
        //{
        //    client.Url = "https://httpbin.org/status/404";

        //    var response = await client.SendAsync();
        //    Assert.Equal(HttpStatusCode.NotFound, client.StatusCode);
        //}

        //[Fact]
        //public async Task Test_Status_500()
        //{
        //    client.Url = "https://httpbin.org/status/500";

        //    var response = await client.SendAsync();
        //    Assert.Equal(HttpStatusCode.InternalServerError, client.StatusCode);
        //}

        //[Fact]
        //public async Task Test_CustomHeaders()
        //{
        //    client.Url = "https://httpbin.org/headers";
        //    client.RequestCustomHeaders.Add("X-Custom-Header", "test-value");

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("X-Custom-Header", jsonResponse!["headers"].ToString());
        //}

        //[Fact]
        //public async Task Test_Authorization()
        //{
        //    client.Url = "https://httpbin.org/headers";
        //    client.Authorization = "Bearer test-token";

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("Bearer test-token", jsonResponse!["headers"].ToString());
        //}

        //[Fact]
        //public async Task Test_Timeout()
        //{
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时

        //    await Assert.ThrowsAsync<WebException>(() => client.SendAsync());
        //}
        //[Fact]
        //public async Task Test_Send_NoneILogger()
        //{

        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService(q => { return "1"; }, q => { return "1"; });
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return null!; });
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/get";
        //    client.Method = HttpMethod.Get;
        //    client.Params.Add("test", "value");

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("test=value", jsonResponse!["url"].ToString());
        //}
         

        

        //[Fact]
        //public async Task Test_Send_Exception_NoneILogger1()
        //{

        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService();
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return null!; });
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/status/500";


        //    var html = await client.SendAsync();
        //    Assert.True(html=="");
        //}

      
        //[Fact]
        //public async Task Test_Send_WebException_HaveILogger1()
        //{

        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService();
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton(mockLogger.Object);
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/status/500";


        //    var html = await client.SendAsync(); 
        //    mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);

        //}
         
        //[Fact]
        //public async Task Test_Send_Exception_HaveILogger1()
        //{

        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService();
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton(mockLogger.Object);
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时


        //    await Assert.ThrowsAsync<WebException>(() => client.SendAsync());
        //    mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        //    mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);

        //}

        //[Fact]
        //public async Task Test_CustomInitFunc()
        //{
        //    client.Url = "https://httpbin.org/headers";
        //    client.CustomInitFunc = (obj) =>
        //    {
        //        var httpClient = (HttpWebRequest)obj;
        //        httpClient.Headers.Add("X-Custom-Init", "test-value");
        //    };

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("X-Custom-Init", jsonResponse!["headers"].ToString());
        //}

        //[Fact]
        //public async Task Test_CustomInitFunc_ThrowException()
        //{
        //    client.Url = "https://httpbin.org/headers";
        //    client.CustomInitFunc = (obj) =>
        //    {
        //        var i = 0;
        //        var x = 1 / i;
        //        var httpClient = (HttpClient)obj;
        //        httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
        //    };

        //    await Assert.ThrowsAsync<DivideByZeroException>(() => client.SendAsync());
        //}

        //[Fact]
        //public async Task Test_CustomInitFunc_ThrowException_NoneILogger()
        //{
        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService(q => { return "1"; }, q => { return "1"; });
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return null!; });

        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();

        //    client.Url = "https://httpbin.org/headers";
        //    client.CustomInitFunc = (obj) =>
        //    {
        //        var i = 0;
        //        var x = 1 / i;
        //        var httpClient = (HttpClient)obj;
        //        httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
        //    };

        //    await Assert.ThrowsAsync<DivideByZeroException>(() => client.SendAsync());
        //}

        //[Fact]
        //public async Task Test_CustomInitFunc_ThrowException_HaveILogger()
        //{
        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService(q => { return "1"; }, q => { return "1"; });
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton(mockLogger.Object);

        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();

        //    client.Url = "https://httpbin.org/headers";
        //    client.CustomInitFunc = (obj) =>
        //    {
        //        var i = 0;
        //        var x = 1 / i;
        //        var httpClient = (HttpClient)obj;
        //        httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
        //    };

        //    await Assert.ThrowsAsync<DivideByZeroException>(() => client.SendAsync());
        //    mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        //}


        //[Fact]
        //public async Task Test_Header()
        //{
        //    client.Url = "https://httpbin.org/headers";
        //    client.RequestHeader.UserAgent = "JfYuHttpRequest/1.0";
        //    client.RequestHeader.Host = "httpbin.org";
        //    client.RequestHeader.Referer = "http://httpbin.org";
        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains("JfYuHttpRequest/1.0", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["User-Agent"]!.ToString());
        //    Assert.Contains("http://httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Referer"]!.ToString());
        //    Assert.Contains("httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Host"]!.ToString());
        //}
        //public class EncodeExpectdata : TheoryData<string>
        //{
        //    public EncodeExpectdata()
        //    {
        //        Add("gzip");
        //        Add("deflate");
        //        Add("brotli");
        //    }
        //}
        //[Theory]
        //[ClassData(typeof(EncodeExpectdata))]
        //public async Task Test_Gzip(string code)
        //{
        //    client.Url = $"https://httpbin.org/{code}";
        //    client.RequestHeader.AcceptEncoding = code;

        //    var response = await client.SendAsync();
        //    var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        //    Assert.Equal(HttpStatusCode.OK, client.StatusCode);
        //    Assert.Contains(code, JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept-Encoding"]!.ToString());
        //}
        //[Fact]
        //public async Task Test_DownloadFile_ContentLengthNull()
        //{
        //    client.Url = "http://httpbin.org/status/204";

        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    using var stream = await client.DownloadFileAsync((p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.NotNull(stream);
        //    Assert.True(0 <= stream.Length);
        //}

        //[Fact]
        //public async Task Test_DownloadFile()
        //{
        //    client.Url = "https://httpbin.org/bytes/1024";

        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    using var stream = await client.DownloadFileAsync((p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.NotNull(stream);
        //    Assert.Equal(1024, stream.Length);
        //}


        //[Fact]
        //public async Task Test_DownloadFileWithParams()
        //{
        //    client.Url = "https://httpbin.org/bytes/1024?x=y";
        //    client.Params = new Dictionary<string, string>() { { "test", "value" } };
        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    using var stream = await client.DownloadFileAsync((p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.NotNull(stream);
        //    Assert.Equal(1024, stream.Length);
        //}





        //[Fact]
        //public async Task Test_DownloadFile_GetException()
        //{
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时

        //    await Assert.ThrowsAsync<WebException>(() => client.DownloadFileAsync());
        //}

        //[Fact]
        //public async Task Test_DownloadFile_GetException_HaveILogger()
        //{
        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService();
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton(mockLogger.Object);
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时

        //    await Assert.ThrowsAsync<WebException>(() => client.DownloadFileAsync());
        //    mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);

        //}
        //[Fact]
        //public async Task Test_DownloadFileWithPath_ContentLengthNull()
        //{
        //    var path = $"{nameof(Test_DownloadFileWithPath_ContentLengthNull)}.txt";
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    client.Url = "http://httpbin.org/status/204";
        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    var flag = await client.DownloadFileAsync(path, (p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.True(flag);
        //    Assert.True(File.Exists(path));
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}

        //[Fact]
        //public async Task Test_DownloadFileWithPathAndParams()
        //{
        //    var path = $"{nameof(Test_DownloadFileWithPathAndParams)}.txt";
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    client.Url = "https://httpbin.org/bytes/1024?x=y";
        //    client.Params.Add("test", "value");
        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    var flag = await client.DownloadFileAsync(path, (p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.True(flag);
        //    Assert.True(File.Exists(path));
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}

        //[Fact]
        //public async Task Test_DownloadFileWithPathEmpty()
        //{
        //    var path = "";
        //    client.Url = "https://httpbin.org/bytes/1024";

        //    await Assert.ThrowsAsync<ArgumentNullException>(() => client.DownloadFileAsync(path));
        //}

        //[Fact]
        //public async Task Test_DownloadFileWithPath()
        //{
        //    var path = $"{nameof(Test_DownloadFileWithPath)}.txt";
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    client.Url = "https://httpbin.org/bytes/1024";

        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    var flag = await client.DownloadFileAsync(path, (p, s, r) =>
        //   {
        //       Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //   });

        //    Assert.True(flag);
        //    Assert.True(File.Exists(path));
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}
        //[Fact]
        //public async Task Test_DownloadFileWithDicPath()
        //{
        //    var path = $"download/{nameof(Test_DownloadFileWithDicPath)}.txt";
        //    if (File.Exists(path))
        //        File.Delete(path);
        //    client.Url = "https://httpbin.org/bytes/1024";

        //    var progress = new Progress<(decimal percentage, decimal speed, decimal remain)>();
        //    var flag = await client.DownloadFileAsync(path, (p, s, r) =>
        //    {
        //        Console.WriteLine($"Progress: {p}%, Speed: {s}KB/s, Remain: {r}s");
        //    });

        //    Assert.True(flag);
        //    Assert.True(File.Exists(path));
        //    if (File.Exists(path))
        //        File.Delete(path);
        //}


        //[Fact]
        //public async Task Test_DownloadFileWithPath_GetException()
        //{
        //    var path = nameof(Test_DownloadFileWithPath_GetException);
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时

        //    await Assert.ThrowsAsync<WebException>(() => client.DownloadFileAsync(path));
        //}



        //[Fact]
        //public async Task Test_DownloadFileWithPath_GetException_HaveILogger()
        //{
        //    var path = nameof(Test_DownloadFileWithPath_GetException_HaveILogger);
        //    var services = new ServiceCollection();
        //    services.AddJfYuHttpRequestService();
        //    var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();

        //    services.AddSingleton(mockLogger.Object);
        //    var serviceProvider = services.BuildServiceProvider();
        //    var client = serviceProvider.GetRequiredService<IJfYuRequest>();
        //    client.Url = "https://httpbin.org/delay/5";
        //    client.Timeout = 1; // 1秒超时

        //    await Assert.ThrowsAsync<WebException>(() => client.DownloadFileAsync(path));
        //    mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
        //}
    }
}

