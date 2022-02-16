using jfYu.Core.jfYuRequest;
using System.IO;
using Xunit;

namespace xUnitTestCore.Request
{
    public class TestjfYuRequestCore
    {

        [Fact]

        public void TestHtml()
        {
            jfYuRequest jfYu = new jfYuRequest("https://b2b.10086.cn/b2b/main/listVendorNotice.html?noticeType=2")
            {
                Method = jfYuRequestMethod.Get
            };
            jfYu.RequestHeader.Host = "https://b2b.10086.cn";
            var x = jfYu.GetHtml();
            var y = jfYu.GetHtmlAsync().Result;
            Assert.True(x.Length > 100);
            Assert.Equal(x.Substring(x.Length - 100, 99), y.Substring(y.Length - 100, 99));


            var jfYu1 = new jfYuHttpClient("https://b2b.10086.cn/b2b/main/listVendorNotice.html?noticeType=2")
            {
                Method = jfYuRequestMethod.Get
            };
            jfYu.RequestHeader.Host = "https://b2b.10086.cn";
            var x1 = jfYu.GetHtml();
            var y1 = jfYu1.GetHtmlAsync().Result;
            Assert.True(x.Length > 100);
            Assert.Equal(x.Substring(x.Length - 100, 99), y.Substring(y.Length - 100, 99));
        }
        [Fact]
        public void TestFile()
        {
            jfYuRequest jfYu = new jfYuRequest("https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg");

            jfYu.GetFile("requestest/2.jpg");
            jfYu.GetFile("requestest/3.jpg");
            Assert.True(File.Exists("requestest/2.jpg"));
            Assert.True(File.Exists("requestest/3.jpg"));
            Assert.Equal(File.ReadAllText("requestest/2.jpg"), File.ReadAllText("requestest/3.jpg"));
            var jfYu1 = new jfYuHttpClient("https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg");
            jfYu1.GetFile("requestest/4.jpg");
            jfYu1.GetFile("requestest/5.jpg");
            Assert.True(File.Exists("requestest/4.jpg"));
            Assert.True(File.Exists("requestest/5.jpg"));
            Assert.Equal(File.ReadAllText("requestest/4.jpg"), File.ReadAllText("requestest/5.jpg"));
            File.Delete("requestest/2.jpg");
            File.Delete("requestest/3.jpg");
            File.Delete("requestest/4.jpg");
            File.Delete("requestest/5.jpg");
            Directory.Delete("requestest");
        }


    }
}
