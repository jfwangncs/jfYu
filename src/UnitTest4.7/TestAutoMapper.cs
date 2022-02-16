using Autofac;
using AutoMapper;
using jfYu.Core.Common.AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest4._7.AutoMapper
{


    public class User
    {

        public string UserName { get; set; }


        public string NickName { get; set; }

    }

    public class UserProfileViewModel
    {
        public string AllName { get; set; }

    }

    [TestClass]
    public class TestAutoMapper
    {      


        [TestMethod]
        public void Test1()
        {

            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<User, UserProfileViewModel>().ForMember(q => q.AllName, opt => opt.MapFrom(q => q.NickName + q.UserName));
            }
            );

            var c = containerBuilder.Build();
            var am = c.Resolve<IMapper>();
            Assert.IsNotNull(am);
            var u = new User() { UserName = "x", NickName = "y" };
            var vu = am.Map<UserProfileViewModel>(u);
            Assert.AreEqual("yx", vu.AllName);
        }       
    }
}
