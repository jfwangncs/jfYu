using Autofac;
using AutoMapper;
using jfYu.Core.Common.AutoMapper;
using Xunit;

namespace xUnitTestCore.AutoMapper
{
    public class TestAutoMapper
    {
        [Fact]
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
            Assert.NotNull(am);
            var u = new User() { UserName = "x", NickName = "y" };
            var vu = am.Map<UserProfileViewModel>(u);
            Assert.Equal("yx", vu.AllName);

        }




        public class User
        {

            public string UserName { get; set; }


            public string NickName { get; set; }

        }

        public class UserProfileViewModel
        {
            public string AllName { get; set; }

        }
    }
}
