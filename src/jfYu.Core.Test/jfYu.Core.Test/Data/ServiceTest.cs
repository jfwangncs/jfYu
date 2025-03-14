using jfYu.Core.Data.Model;
using jfYu.Core.Test.Models;
using jfYu.Core.Test.Models.Entity;
using jfYu.Core.Test.Models.Service;
using Microsoft.EntityFrameworkCore;

namespace jfYu.Core.Test.Data
{

    [Collection("Data")]
    public class ServiceTest
    {
        private readonly IUserService _userService;

        public ServiceTest(IUserService userService)
        {
            _userService = userService;
        }

        [Fact]
        public async void GetByNickNameAsync_Correctly()
        {
            var user = new User
            {
                UserName = "Test",
                NickName = "Test"
            };
            var result = await _userService.AddAsync(user);
            Assert.Equal(1, result);
            var data = await _userService.GetByNickNameAsync(user.NickName);
            Assert.NotNull(data);
        }

        [Fact]
        public async void AddAsync_Correctly()
        {
            var user = new User
            {
                UserName = "Test",
                NickName = "Test"
            };
            var result = await _userService.AddAsync(user);
            Assert.Equal(1, result);
            var data = await _userService.GetOneAsync(q => q.Id.Equals(user.Id));
            Assert.NotNull(data);
        }

        [Fact]
        public async void AddAsync_Range_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            var users = new EFUserFaker().Generate(8);
            var result = await _userService.AddAsync(users);
            Assert.Equal(8, result);
            var data = await _userService.GetListAsync();
            Assert.Equal(8, data.Count);
        }
        [Fact]
        public async void UpdateAsync_Id_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(1));
            var user = await _userService.GetOneAsync();
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await _userService.UpdateAsync(user);
            Assert.Equal(1, result);
            var data = await _userService.GetOneAsync(q => q.Id.Equals(user.Id));
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async void UpdateAsync_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(1));
            var user = await _userService.GetOneAsync();
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await _userService.UpdateAsync(user);
            Assert.Equal(1, result);
            var data = await _userService.GetOneAsync(q => q.Id.Equals(user.Id));
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async void UpdateAsync_Again_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(1));
            var user = await _userService.GetOneAsync();
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await _userService.UpdateAsync(user);
            Assert.Equal(1, result);
            var data = await _userService.GetOneAsync(q => q.Id.Equals(user.Id));
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);


            user.State = (int)StateEnum.Enabled;
            user.DepartmentId = 567;
            user.UserName = "Test213";
            result = await _userService.UpdateAsync(user);
            Assert.Equal(1, result);
            data = await _userService.GetOneAsync(q => q.Id.Equals(user.Id));
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async void UpdateAsync_Range_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(12));
            var users = await _userService.GetListAsync();
            users = users.Take(7).OrderBy(q => q.Id).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                user!.State = (int)StateEnum.Disable;
                user.DepartmentId = i + 1;
                user.UserName = $"Test{i}";
            }

            var result = await _userService.UpdateAsync(users.ToList());
            Assert.Equal(7, result);
            var data = await _userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.NotNull(data);
            data = data.OrderBy(q => q.Id).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal(data[i].State, users[i].State);
                Assert.Equal(data[i].DepartmentId, users[i].DepartmentId);
                Assert.Equal(data[i].UserName, users[i].UserName);
                Assert.Equal(data[i].NickName, users[i].NickName);
            }
        }
        [Fact]
        public async void UpdateAsync_Predicate_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(12));

            var users = await _userService.GetListAsync();
            users = users.Take(6).OrderBy(q => q.Id).ToList();

            var result = await _userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            });
            Assert.Equal(6, result);
            var data = await _userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.NotNull(data);
            data = data.OrderBy(q => q.Id).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal(data[i].State, users[i].State);
                Assert.Equal(data[i].DepartmentId, users[i].DepartmentId);
                Assert.Equal(data[i].UserName, users[i].UserName);
                Assert.Equal(data[i].NickName, users[i].NickName);
            }
        }

        [Fact]
        public async void UpdateAsync_PredicateReturnEmpty_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(12));

            var users = new List<User>();

            var result = await _userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            });
            Assert.Equal(0, result);
        }

        [Fact]
        public async void UpdateAsync_PredicateOrScalarIsNull_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(12));

            var users = new List<User>();

            var result = await _userService.UpdateAsync(null!, (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            });
            Assert.Equal(0, result);

            result = await _userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), null!);
            Assert.Equal(0, result);
        }

        [Fact]
        public async void RemoveAsync_PredicateReturnEmptyOrNull_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var users = new List<User>();

            var result = await _userService.RemoveAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.Equal(0, result);

            result = await _userService.RemoveAsync(null!);
            Assert.Equal(0, result);
        }


        [Fact]
        public async void RemoveAsync_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var users = await _userService.GetListAsync();
            users = users.Take(6).OrderBy(q => q.Id).ToList();

            var result = await _userService.RemoveAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.Equal(6, result);

            var data = await _userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.NotNull(data);
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal((int)StateEnum.Disable, users[i].State);
            }
        }
        [Fact]
        public async void HardRemoveAsync_PredicateReturnEmptyOrNull_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var users = new List<User>() { new() { Id = 1, UserName = "ex" } };

            var result = await _userService.HardRemoveAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.Equal(0, result);

            result = await _userService.HardRemoveAsync(null!);
            Assert.Equal(0, result);
        }

        [Fact]
        public async void HardRemoveAsync_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var users = await _userService.GetListAsync();
            users = users.Take(6).OrderBy(q => q.Id).ToList();

            var result = await _userService.HardRemoveAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.Equal(6, result);

            var data = await _userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id));
            Assert.Empty(data);
        }

        [Fact]
        public async void GetListAsync_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetListAsync(q => new TestSubModel()
            {
                Id = q.Id,
                CardNum = q.UserName,
                ExpiresIn = q.CreatedTime
            });

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await _userService.GetOneAsync(q => q.Id.Equals(result[i].Id));
                Assert.Equal(result[i].CardNum, user!.UserName);
                Assert.Equal(result[i].ExpiresIn, user.UpdatedTime);
            }
        }

        [Fact]
        public async void GetListAsync_WithPredicate_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetListAsync(q => new TestSubModel() { Id = q.Id, CardNum = q.UserName, ExpiresIn = q.CreatedTime }, q => true);

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await _userService.GetOneAsync(q => q.Id.Equals(result[i].Id));
                Assert.Equal(result[i].CardNum, user!.UserName);
                Assert.Equal(result[i].ExpiresIn, user.CreatedTime);
            }
        }

        [Fact]
        public async void GetListAsync_ScalarIsNull_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetListAsync<TestSubModel>(null!);

            Assert.Empty(result);
        }


        [Fact]
        public async void GetSelectListAsyncAsync_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetSelectListAsync(q => new TestSubModel()
            {
                Id = q.Id,
                CardNum = q.UserName,
                ExpiresIn = q.CreatedTime
            });

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await _userService.GetOneAsync(q => q.Id.Equals(result[i].Id));
                Assert.Equal(result[i].CardNum, user!.UserName);
            }
        }

        [Fact]
        public async void GetSelectListAsync_WithPredicate_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetSelectListAsync(q => new TestSubModel() { Id = q.Id, CardNum = q.UserName }, q => true);

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await _userService.GetOneAsync(q => q.Id.Equals(result[i].Id));
                Assert.Equal(result[i].CardNum, user!.UserName);
            }
        }

        [Fact]
        public async void GetSelectListAsync_ScalarIsNull_Correctly()
        {
            _userService.Context.Users.ExecuteDelete();
            await _userService.AddAsync(new EFUserFaker().Generate(10));

            var result = await _userService.GetSelectListAsync<TestSubModel>(null!);

            Assert.Empty(result);
        }
    }
}
