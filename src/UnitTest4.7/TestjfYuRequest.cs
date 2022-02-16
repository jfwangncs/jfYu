using Autofac;
using jfYu.Core.Common.Configurations;
using jfYu.Core.EMail;
using jfYu.Core.Excel;
using jfYu.Core.jfYuRequest;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace UnitTest4._7.Request
{

    [TestClass]
    public class TestjfYuRequest
    {     


        [TestMethod]
        public void SendHtml()
        {

            jfYuRequest jfYu = new jfYuRequest("https://b2b.10086.cn/b2b/main/listVendorNotice.html?noticeType=2")
            {
                Method = jfYuRequestMethod.Get
            };
            jfYu.RequestHeader.Host = "https://b2b.10086.cn";
            var x = jfYu.GetHtml();
            var y = jfYu.GetHtmlAsync().Result;
            Assert.IsTrue(x.Length > 100);
            Assert.AreEqual(x.Substring(x.Length - 100, 99), y.Substring(y.Length - 100, 99));
        }

        [TestMethod]
        public void SendFile()
        {

            jfYuRequest jfYu = new jfYuRequest("https://img.nga.178.com/attachments/mon_201904/11/-7da9Q5-dgq4ZgT3cSzk-qo.jpg");

            jfYu.GetFile("d:/2.jpg",(q,w,e)=> { });
            jfYu.GetFile("d:/3.jpg");
            Assert.IsTrue(File.Exists("d:/2.jpg"));
            Assert.IsTrue(File.Exists("d:/3.jpg"));
            Assert.AreEqual(File.ReadAllText("d:/2.jpg"), File.ReadAllText("d:/3.jpg"));          
            File.Delete("d:/2.jpg");
            File.Delete("d:/3.jpg");
            
           

        }
    }



}
