using jfYu.Core.jfYuRequest;
using jfYu.Core.jfYuRequest.Enum;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace jfYu.Core.Test.JfYuRequest
{
    [Collection("Data")]
    public class JfYuBaseTests
    {
        #region Send
        public class ServicesNoLogger : TheoryData<IJfYuRequest>
        {
            public ServicesNoLogger()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService();
                var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();
                services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return null!; });
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();

                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService();
                var mockLogger1 = new Mock<ILogger<JfYuHttpClient>>();
                services1.AddSingleton<ILogger<JfYuHttpClient>>(q => { return null!; });
                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();
                Add(client);
                Add(request);
            }
        }


        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Get_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get?username=testUser&age=30";
            client.Method = HttpMethod.Get;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["age"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Post_Plain_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/post";
            client.Method = HttpMethod.Post;
            client.RequestData = "username=testUser&age=30";
            client.ContentType = RequestContentType.Plain;
            client.RequestEncoding = Encoding.UTF8;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains(client.RequestData, jsonResponse!["data"].ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Post_XML_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/post";
            client.Method = HttpMethod.Post;
            client.RequestData = "<user><username>testUser</username><age>30</age></user>";
            client.ContentType = RequestContentType.Xml;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains(client.RequestData, jsonResponse!["data"].ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Post_FormUrlEncoded_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/post";
            client.Method = HttpMethod.Post;
            client.RequestData = "username=testUser&age=30";
            client.ContentType = RequestContentType.FormUrlEncoded;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["form"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["form"].ToString()!)!["age"]!.ToString());
        }


        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Post_Json_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/post";
            client.Method = HttpMethod.Post;
            client.RequestData = "{\"username\":\"testUser\",\"age\":30}";
            client.ContentType = RequestContentType.Json;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["json"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["json"].ToString()!)!["age"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Post_FormData_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/post";
            client.Method = HttpMethod.Post;
            client.RequestData = "username=testUser&age=30";
            client.ContentType = RequestContentType.FormData;
            var file = "test.txt";
            var text = "dada dadad大大大阿斗哇打完带娃啊我啊";
            if (File.Exists(file))
                File.Delete(file);

            using var fs = File.Create(file);
            await fs.WriteAsync(Encoding.UTF8.GetBytes(text));
            fs.Dispose();

            client.Files = new Dictionary<string, string>() { { "test.txt", "test.txt" }, { "test1.txt", "test.txt" } };
            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);

            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["form"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["form"].ToString()!)!["age"]!.ToString());
            Assert.Contains(text, JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["files"].ToString()!)![file].ToString());
            Assert.Contains(text, JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["files"].ToString()!)!["test1.txt"].ToString());

            if (File.Exists(file))
                File.Delete(file);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Put_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/put";
            client.Method = HttpMethod.Put;
            client.RequestData = "{\"username\":\"testUser\",\"age\":30}";

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["json"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["json"].ToString()!)!["age"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Delete_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/delete?username=testUser&age=30";
            client.Method = HttpMethod.Delete;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["age"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Status_404(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/status/404";

            await client.SendAsync();
            Assert.Equal(HttpStatusCode.NotFound, client.StatusCode);
        }
        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Status_400(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/status/400";

            await client.SendAsync();
            Assert.Equal(HttpStatusCode.BadRequest, client.StatusCode);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Status_500(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/status/500";

            await client.SendAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, client.StatusCode);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Status_502(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/status/502";

            await client.SendAsync();
            Assert.Equal(HttpStatusCode.BadGateway, client.StatusCode);
        }
        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Timeout(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/delay/5";
            client.Timeout = 1;

            var ex = await Record.ExceptionAsync(client.SendAsync);
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Header(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/headers";
            client.RequestHeader.UserAgent = "JfYuHttpClient/1.0";
            client.RequestHeader.Host = "httpbin.org";
            client.RequestHeader.Referer = "http://httpbin.org";
            client.RequestHeader.Accept = "text/html";
            client.RequestHeader.AcceptLanguage = "zh-en";
            client.RequestHeader.CacheControl = "cache";
            client.RequestHeader.Connection = "keep-alive";
            client.RequestHeader.Pragma = "Pragma";
            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("JfYuHttpClient/1.0", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["User-Agent"]!.ToString());
            Assert.Contains("http://httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Referer"]!.ToString());
            Assert.Contains("httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Host"]!.ToString());
            Assert.Contains("text/html", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept"]!.ToString());
            Assert.Contains("zh-en", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept-Language"]!.ToString());
            Assert.Contains("cache", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Cache-Control"]!.ToString());
            Assert.Contains("Pragma", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Pragma"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Header_Set(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/headers";
            var header = new RequestHeader
            {
                UserAgent = "JfYuHttpClient/1.0",
                Host = "httpbin.org",
                Referer = "http://httpbin.org",
                Accept = "text/html",
                AcceptLanguage = "zh-en",
                CacheControl = "cache",
                Connection = "keep-alive",
                Pragma = "Pragma"
            };
            client.RequestHeader = header;
            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("JfYuHttpClient/1.0", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["User-Agent"]!.ToString());
            Assert.Contains("http://httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Referer"]!.ToString());
            Assert.Contains("httpbin.org", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Host"]!.ToString());
            Assert.Contains("text/html", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept"]!.ToString());
            Assert.Contains("zh-en", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept-Language"]!.ToString());
            Assert.Contains("cache", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Cache-Control"]!.ToString());
            Assert.Contains("Pragma", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Pragma"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_CustomHeaders(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/headers";
            client.RequestCustomHeaders.Add("X-Custom-Header", "test-value");
            client.Authorization = "Bearer test-token";

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);

            Assert.Contains("test-value", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["X-Custom-Header"]!.ToString());
            Assert.Contains("test-token", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Authorization"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_CustomHeaders_Set(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/headers";
            client.RequestCustomHeaders = new Dictionary<string, string>() { { "X-Custom-Header", "test-value" } };
            client.Authorization = "Bearer test-token";

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);

            Assert.Contains("test-value", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["X-Custom-Header"]!.ToString());
            Assert.Contains("test-token", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Authorization"]!.ToString());
        }

        public class EncodeExpectdata : TheoryData<string, IJfYuRequest>
        {
            public EncodeExpectdata()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService();
                var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();
                services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return default!; });
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();

                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService();
                var mockLogger1 = new Mock<ILogger<JfYuHttpRequest>>();
                services1.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return default!; });
                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();

                Add("gzip", client);
                Add("deflate", client);
                Add("brotli", client);

                Add("gzip", request);
                Add("deflate", request);
                Add("brotli", request);
            }
        }
        [Theory]
        [ClassData(typeof(EncodeExpectdata))]
        public async Task Test_Gzip(string code, IJfYuRequest client)
        {
            client.Url = $"https://httpbin.org/{code}";
            client.RequestHeader.AcceptEncoding = code;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains(code, JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["Accept-Encoding"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Cookies(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/cookies/set?c1=v1&c2=v2";
            client.RequestCookies.Add(new Cookie("test", "value", "/", "httpbin.org"));
            client.RequestCookies.Add(new Cookie("test1", "value1", "/", "httpbin.org"));
            await client.SendAsync();

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Equal("value", client.ResponseCookies["test"]!.Value);
            Assert.Equal("value1", client.ResponseCookies["test1"]!.Value);
            Assert.Equal("v1", client.ResponseCookies["c1"]!.Value);
            Assert.Equal("v2", client.ResponseCookies["c2"]!.Value);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_Cookies_Set(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/cookies/set?c1=v1&c2=v2";

            var cookies = new CookieContainer();
            cookies.Add(new Cookie("test", "value", "/", "httpbin.org"));
            cookies.Add(new Cookie("test1", "value1", "/", "httpbin.org"));
            client.RequestCookies = cookies;
            await client.SendAsync();

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);

            Assert.Equal("value", client.ResponseCookies["test"]!.Value);
            Assert.Equal("value1", client.ResponseCookies["test1"]!.Value);
            Assert.Equal("v1", client.ResponseCookies["c1"]!.Value);
            Assert.Equal("v2", client.ResponseCookies["c2"]!.Value);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_CustomInitFunc(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/headers";
            client.CustomInitFunc = (obj) =>
            {
                if (client is JfYuHttpClient)
                {
                    var httpClient = (HttpClient)obj;
                    httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
                }
                if (client is JfYuHttpRequest)
                {
                    var WebRequest = (HttpWebRequest)obj;
                    WebRequest.Headers.Add("X-Custom-Init", "test-value");
                }
            };

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("test-value", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["headers"].ToString()!)!["X-Custom-Init"]!.ToString());
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_CustomInitFunc_ThrowException(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get";
            client.CustomInitFunc = (obj) =>
            {
                var i = 0;
                var x = 1 / i;
                var httpClient = (HttpClient)obj;
                httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
            };

            await Assert.ThrowsAsync<DivideByZeroException>(client.SendAsync);
        }


        [Fact]
        public async Task Test_Porxy()
        {
            static X509Certificate2 GenerateRandomCertificate()
            {
                var subjectName = $"CN=TestCert-{Guid.NewGuid()}";
                using RSA rsa = RSA.Create(2048);
                var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
                DateTimeOffset startTime = DateTimeOffset.UtcNow;
                DateTimeOffset endTime = startTime.AddSeconds(1);
                var certificate = request.CreateSelfSigned(startTime, endTime);
                return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "password123"), "password123");
            }
            var services = new ServiceCollection();
            services.AddJfYuHttpRequestService();
            var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();
            services.AddSingleton<ILogger<JfYuHttpRequest>>(q => { return null!; });
            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<IJfYuRequest>();
            client.Url = "https://httpbin.org/get";
            client.Proxy = new WebProxy("http://example.com");
            client.Certificate = GenerateRandomCertificate();

            await Assert.ThrowsAsync<WebException>(client.SendAsync);
        }
        #endregion

        #region Download
        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadStream(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/bytes/1024";

            using var stream = await client.DownloadFileAsync((p, s, r) =>
            {
                Assert.True(p >= 0);
                Assert.True(s >= 0);
                Assert.True(r >= 0);
            });

            Assert.NotNull(stream);
            Assert.Equal(1024, stream.Length);
        }


        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile(IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile);
            if (File.Exists(path))
                File.Delete(path);
            client.Url = "https://httpbin.org/bytes/1024";

            var flag = await client.DownloadFileAsync(path, (p, s, r) =>
            {
                Assert.True(p >= 0);
                Assert.True(s >= 0);
                Assert.True(r >= 0);
            });

            Assert.True(flag);
            Assert.True(File.Exists(path));
            Assert.Equal(1024, new FileInfo(path).Length);
            if (File.Exists(path))
                File.Delete(path);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile_Dic(IJfYuRequest client)
        {
            var path = $"download/{nameof(Test_DownloadFile_Dic)}";
            if (File.Exists(path))
                File.Delete(path);
            client.Url = "https://httpbin.org/bytes/1024";

            var flag = await client.DownloadFileAsync(path, (p, s, r) =>
            {
                Assert.True(p >= 0);
                Assert.True(s >= 0);
                Assert.True(r >= 0);
            });

            Assert.True(flag);
            Assert.True(File.Exists(path));
            Assert.Equal(1024, new FileInfo(path).Length);
            if (File.Exists(path))
                File.Delete(path);
        }


        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile_ContentLengthNull(IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile_ContentLengthNull);
            if (File.Exists(path))
                File.Delete(path);
            client.Url = "http://httpbin.org/status/204";

            using var stream = await client.DownloadFileAsync();
            var flag = await client.DownloadFileAsync(path);

            Assert.NotNull(stream);
            Assert.True(0 <= stream.Length);

            Assert.True(flag);
            Assert.True(File.Exists(path));
            Assert.Equal(0, new FileInfo(path).Length);

            if (File.Exists(path))
                File.Delete(path);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile_Get500(IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile_ContentLengthNull);

            client.Url = "https://httpbin.org/status/500";
            if (client is JfYuHttpClient)
            {
                var response = await client.DownloadFileAsync();
                var flag = await client.DownloadFileAsync(path);
                Assert.Null(response);
                Assert.False(flag);
                Assert.False(File.Exists(path));
            }
            if (client is JfYuHttpRequest)
            {
                var ex = await Record.ExceptionAsync(() => client.DownloadFileAsync());
                Assert.IsAssignableFrom<Exception>(ex);
                var ex1 = await Record.ExceptionAsync(() => client.DownloadFileAsync(path));
                Assert.IsAssignableFrom<Exception>(ex1);
                Assert.False(File.Exists(path));
            }
        }



        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile_Timeout(IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile_Timeout);
            client.Url = "https://httpbin.org/delay/5";
            client.Timeout = 1;

            var ex = await Record.ExceptionAsync(() => client.DownloadFileAsync());
            Assert.IsAssignableFrom<Exception>(ex);

            var ex1 = await Record.ExceptionAsync(() => client.DownloadFileAsync(path));
            Assert.IsAssignableFrom<Exception>(ex1);
        }

        [Theory]
        [ClassData(typeof(ServicesNoLogger))]
        public async Task Test_DownloadFile_PathIsEmpty(IJfYuRequest client)
        {
            var path = "";
            client.Url = "https://httpbin.org/bytes/1024";

            await Assert.ThrowsAsync<ArgumentNullException>(() => client.DownloadFileAsync(path));
        }
        #endregion

        #region  Logger
        public class ServicesWithLogger : TheoryData<object, IJfYuRequest>
        {
            public ServicesWithLogger()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService(q=>q.LoggingFields= JfYuLoggingFields.RequestData);
                var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();
                services.AddSingleton(mockLogger.Object);
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();

                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService(null,q => q.LoggingFields = JfYuLoggingFields.RequestData);
                var mockLogger1 = new Mock<ILogger<JfYuHttpClient>>();
                services1.AddSingleton(mockLogger1.Object);
                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();
                Add(mockLogger, request);
                Add(mockLogger1, client);
            }
        }

        [Theory]
        [ClassData(typeof(ServicesWithLogger))]
        public async Task Test_Send_Correctly(object logger, IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get?x=y";
            client.Method = HttpMethod.Get;
            var response = await client.SendAsync();
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(2));
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Exactly(2));
        }


        [Theory]
        [ClassData(typeof(ServicesWithLogger))]
        public async Task Test_Send_ThrowException(object logger, IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/delay/5";
            client.Timeout = 1;

            var ex = await Record.ExceptionAsync(client.SendAsync);
            Assert.IsAssignableFrom<Exception>(ex);

            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
        }
        #endregion

        #region Exception With Logger

        public class ServicesWrongFilterWithLogger : TheoryData<object, IJfYuRequest>
        {
            public ServicesWrongFilterWithLogger()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService(q => { q.RequestFunc = z => { var x = 0; return (1 / x).ToString(); }; q.ResponseFunc = z => z; });
                var mockLogger = new Mock<ILogger<JfYuHttpRequest>>();
                services.AddSingleton(mockLogger.Object);
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();

                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService(null, q => { q.RequestFunc = z => { var x = 0; return (1 / x).ToString(); }; q.ResponseFunc = z => z; });
                var mockLogger1 = new Mock<ILogger<JfYuHttpClient>>();
                services1.AddSingleton(mockLogger1.Object);
                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();
                Add(mockLogger, request);
                Add(mockLogger1, client);
            }
        }
        [Theory]
        [ClassData(typeof(ServicesWrongFilterWithLogger))]
        public async Task Test_Filter_ThrowException(object logger, IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get";

            var ex = await Record.ExceptionAsync(client.SendAsync);
            Assert.IsAssignableFrom<Exception>(ex);

            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        }


        [Theory]
        [ClassData(typeof(ServicesWithLogger))]
        public async Task Test_Send_Init_ThrowException(object logger, IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get";
            client.CustomInitFunc = (obj) =>
                {
                    var i = 0;
                    var x = 1 / i;
                    var httpClient = (HttpClient)obj;
                    httpClient.DefaultRequestHeaders.Add("X-Custom-Init", "test-value");
                };

            await Assert.ThrowsAsync<DivideByZeroException>(client.SendAsync);

            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        }

        [Theory]
        [ClassData(typeof(ServicesWithLogger))]
        public async Task Test_DownloadFile_ThrowException(object logger, IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile_ThrowException);
            client.Url = "https://httpbin.org/delay/5";
            client.Timeout = 1;

            var ex = await Record.ExceptionAsync(() => client.DownloadFileAsync(path));
            var ex1 = await Record.ExceptionAsync(() => client.DownloadFileAsync());
            Assert.IsAssignableFrom<Exception>(ex);
            Assert.IsAssignableFrom<Exception>(ex1);

            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        }

        [Theory]
        [ClassData(typeof(ServicesWithLogger))]
        public async Task Test_DownloadFile_ProgressThrowException(object logger, IJfYuRequest client)
        {
            var path = nameof(Test_DownloadFile_ThrowException);
            client.Url = "https://httpbin.org/bytes/1024";

            var ex = await Record.ExceptionAsync(() => client.DownloadFileAsync(path, (q, w, e) => { int.Parse("x"); }));
            var ex1 = await Record.ExceptionAsync(() => client.DownloadFileAsync((q, w, e) => { int.Parse("x"); }));
            Assert.IsAssignableFrom<Exception>(ex);
            Assert.IsAssignableFrom<Exception>(ex1);

            if (logger is Mock<ILogger<JfYuHttpRequest>> logger1)
                logger1.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
            if (logger is Mock<ILogger<JfYuHttpClient>> logger2)
                logger2.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
        }

        #endregion

        public class ServicesFilter : TheoryData<IJfYuRequest>
        {
            public ServicesFilter()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService(q => { q.RequestFunc = z => z; q.ResponseFunc = z => z; });
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();
                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService(null, q => { q.RequestFunc = z => z; q.ResponseFunc = z => z; });

                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();
                Add(request);
                Add(client);
            }
        }

        [Theory]
        [ClassData(typeof(ServicesFilter))]
        public async Task Test_Get_Filter_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get?username=testUser&age=30";
            client.Method = HttpMethod.Get;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["age"]!.ToString());
        }

        public class ServicesNoneFilter : TheoryData<IJfYuRequest>
        {
            public ServicesNoneFilter()
            {
                IJfYuRequest client;
                IJfYuRequest request;
                var services = new ServiceCollection();
                services.AddJfYuHttpRequestService(null);
                var serviceProvider = services.BuildServiceProvider();
                request = serviceProvider.GetRequiredService<IJfYuRequest>();
                var services1 = new ServiceCollection();
                services1.AddJfYuHttpClientService(null, null);

                var serviceProvider1 = services1.BuildServiceProvider();
                client = serviceProvider1.GetRequiredService<IJfYuRequest>();
                Add(request);
                Add(client);
            }
        }

        [Theory]
        [ClassData(typeof(ServicesNoneFilter))]
        public async Task Test_Get_NoneFilter_Success(IJfYuRequest client)
        {
            client.Url = "https://httpbin.org/get?username=testUser&age=30";
            client.Method = HttpMethod.Get;

            var response = await client.SendAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            Assert.Equal(HttpStatusCode.OK, client.StatusCode);
            Assert.Contains("testUser", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["username"]!.ToString());
            Assert.Contains("30", JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse!["args"].ToString()!)!["age"]!.ToString());
        }


    }
}

