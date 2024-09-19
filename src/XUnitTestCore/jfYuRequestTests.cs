using jfYu.Core.jfYuRequest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore
{
    public class JfYuRequestTests(IJfYuRequest client)
    {
        private readonly IJfYuRequest _client = client;

        [Fact]
        public void IOC_Should_Return_NotNull_When_Injected()
        {
            var services = new ServiceCollection();
            services.AddJfYuHttpClientService();
            var serviceProvider = services.BuildServiceProvider();
            var _clientService = serviceProvider.GetService<IJfYuRequest>();
            Assert.NotNull(_clientService);

        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task DownloadFileAsync_Should_Return_Error(string path)
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "",
                Method = HttpMethod.Get,
                Timeout = 60
            };
            await Assert.ThrowsAnyAsync<Exception>(async () => await jfYu.DownloadFileAsync(path));

            _client.Url = "";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 60;
            await Assert.ThrowsAnyAsync<Exception>(async () => await _client.DownloadFileAsync(path));
        }

        [Fact]
        public async Task SendAsync_Should_Return_Html()
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://www.baidu.com/",
                Method = HttpMethod.Get,
                Timeout = 60
            };
            var html = await jfYu.SendAsync();
            Assert.True(html.Length > 100);
            Assert.Equal(System.Net.HttpStatusCode.OK, jfYu.StatusCode);


            _client.Url = "https://www.baidu.com/";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 60;

            var html1 = await _client.SendAsync();
            Assert.True(html1.Length > 100);
            Assert.Equal(System.Net.HttpStatusCode.OK, _client.StatusCode);
        }

        [Fact]
        public async Task SendAsync_Should_Return_403_When_Request_403()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            _client.Url = "https://bbs.nga.cn/read.php?tid=39381647";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 60;
            _client.RequestEncoding = Encoding.GetEncoding("GB18030");

            var html1 = await _client.SendAsync();
            Assert.True(html1.Length > 100);
            Assert.NotEqual(System.Net.HttpStatusCode.OK, _client.StatusCode);

            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://bbs.nga.cn/read.php?tid=39381647",
                Method = HttpMethod.Get,
                Timeout = 60,
                RequestEncoding = Encoding.GetEncoding("GB18030"),
            };
            var html = await jfYu.SendAsync();
            Assert.True(html.Length > 100);
            Assert.NotEqual(System.Net.HttpStatusCode.OK, jfYu.StatusCode);
        }

        [Fact]
        public async Task DownloadFileAsync_Should_Return_True()
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg"
            };

            var flag = await jfYu.DownloadFileAsync("requestest/2.jpg");
            Assert.True(flag);
            var stream = await jfYu.DownloadFileAsync();
            FileStream fs = File.Create("requestest/3.jpg");
            await fs.WriteAsync(stream?.ToArray());
            fs.Flush();
            fs.Close();
            Assert.True(File.Exists("requestest/2.jpg"));
            Assert.True(File.Exists("requestest/3.jpg"));
            Assert.Equal(File.ReadAllText("requestest/2.jpg"), File.ReadAllText("requestest/3.jpg"));


            _client.Url = "https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg";


            flag = await jfYu.DownloadFileAsync("2.jpg");
            Assert.True(flag);
            var stream1 = await jfYu.DownloadFileAsync();
            FileStream fs1 = File.Create("3.jpg");
            await fs1.WriteAsync(stream1?.ToArray());
            fs1.Flush();
            fs1.Close();
            Assert.True(File.Exists("2.jpg"));
            Assert.True(File.Exists("3.jpg"));
            Assert.Equal(File.ReadAllText("2.jpg"), File.ReadAllText("3.jpg"));
            File.Delete("requestest/2.jpg");
            File.Delete("requestest/3.jpg");
            File.Delete("2.jpg");
            File.Delete("3.jpg");
            Directory.Delete("requestest");
        }

        [Fact]
        public async Task SendAsync_Should_Throw_TimeoutException_When_Request_Times_Out()
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://httpbin.org/delay/30",
                Method = HttpMethod.Get,
                Timeout = 5
            };

            await Assert.ThrowsAnyAsync<Exception>(jfYu.SendAsync);


            _client.Url = "https://httpbin.org/delay/30";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 5;
            await Assert.ThrowsAnyAsync<Exception>(_client.SendAsync);
        }

        [Fact]
        public async Task SendAsync_Should_Return_500_When_Request_500()
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://httpbin.org/status/500",
                Method = HttpMethod.Get,
                Timeout = 20
            };
            await jfYu.SendAsync();
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, jfYu.StatusCode);


            _client.Url = "https://httpbin.org/status/500";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 20;
            await _client.SendAsync();
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, _client.StatusCode);
        }

        [Fact]
        public async Task SendAsync_Should_Return_404_When_Request_404()
        {
            var jfYu = new JfYuHttpRequest()
            {
                Url = "https://httpbin.org/status/404",
                Method = HttpMethod.Get,
                Timeout = 20
            };
            await jfYu.SendAsync();
            Assert.Equal(HttpStatusCode.NotFound, jfYu.StatusCode);


            _client.Url = "https://httpbin.org/status/404";
            _client.Method = HttpMethod.Get;
            _client.Timeout = 20;
            await _client.SendAsync();
            Assert.Equal(HttpStatusCode.NotFound, _client.StatusCode);
        }
    }
}
