using jfYu.Core.jfYuRequest;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore
{
	public class JfYuRequestTests
	{
		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public async Task ErrorTest(string path)
		{
			var jfYu = new JfYuHttpRequest()
			{
				Url = "",
				Method = RequestMethod.Get,
				Timeout = 60
			};
			await Assert.ThrowsAnyAsync<Exception>(async () => await jfYu.DownloadFileAsync(path));


			var jfYu1 = new JfYuHttpClient()
			{
				Url = "",
				Method = RequestMethod.Get,
				Timeout = 60
			};
			await Assert.ThrowsAnyAsync<Exception>(async () => await jfYu1.DownloadFileAsync(path));
		}

		[Fact]
		public async Task HtmlTest()
		{
			var jfYu = new JfYuHttpRequest()
			{
				Url = "https://www.baidu.com/",
				Method = RequestMethod.Get,
				Timeout = 60
			};
			var html = await jfYu.SendAsync();
			Assert.True(html.Length > 100);
			Assert.Equal(System.Net.HttpStatusCode.OK, jfYu.StatusCode);

			var jfYu1 = new JfYuHttpClient()
			{
				Url = "https://www.baidu.com/",
				Method = RequestMethod.Get,
				Timeout = 60
			};
			var html1 = await jfYu1.SendAsync();
			Assert.True(html1.Length > 100);
			Assert.Equal(System.Net.HttpStatusCode.OK, jfYu1.StatusCode);
		}

		[Fact]
		public async Task Err403Test()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			var jfYu1 = new JfYuHttpClient()
			{
				Url = "https://bbs.nga.cn/read.php?tid=39381647",
				Method = RequestMethod.Get,
				Timeout = 60,
				RequestEncoding = Encoding.GetEncoding("GB18030"),
			};
			var html1 = await jfYu1.SendAsync();
			Assert.True(html1.Length > 100);
			Assert.NotEqual(System.Net.HttpStatusCode.OK, jfYu1.StatusCode);

			var jfYu = new JfYuHttpRequest()
			{
				Url = "https://bbs.nga.cn/read.php?tid=39381647",
				Method = RequestMethod.Get,
				Timeout = 60,
				RequestEncoding = Encoding.GetEncoding("GB18030"),
			};
			var html = await jfYu.SendAsync();
			Assert.True(html.Length > 100);
			Assert.NotEqual(System.Net.HttpStatusCode.OK, jfYu.StatusCode);
		}

		[Fact]
		public async void TestFile()
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

			var jfYu1 = new JfYuHttpClient()
			{
				Url = "https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg"
			};

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


	}
}
