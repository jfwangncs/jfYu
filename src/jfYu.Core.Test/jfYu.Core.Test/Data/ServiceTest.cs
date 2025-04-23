using jfYu.Core.Data.Model;
using jfYu.Core.Test.Models;
using jfYu.Core.Test.Models.Entity;
using jfYu.Core.Test.Models.Service;
using Microsoft.EntityFrameworkCore;

namespace jfYu.Core.Test.Data
{
    [Collection("Data")]
    public class ServiceTest(IUserService userService)
    {
        [Fact]
        public async Task GetByNickNameAsync_Correctly()
        {
            var user = new User
            {
                UserName = "Test",
                NickName = "Test"
            };
            var result = await userService.AddAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            var data = await userService.GetByNickNameAsync(user.NickName).ConfigureAwait(true);
            Assert.NotNull(data);
        }

        [Fact]
        public async Task AddAsync_Correctly()
        {
            var user = new User
            {
                UserName = "Test",
                NickName = "Test"
            };
            var result = await userService.AddAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            var data = await userService.GetOneAsync(q => q.Id.Equals(user.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
        }

        [Fact]
        public async Task AddAsync_Range_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            var users = new EFUserFaker().Generate(8);
            var result = await userService.AddAsync(users).ConfigureAwait(true);
            Assert.Equal(8, result);
            var data = await userService.GetListAsync().ConfigureAwait(true);
            Assert.Equal(8, data.Count);
        }

        [Fact]
        public async Task UpdateAsync_Id_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(1)).ConfigureAwait(true);
            var user = await userService.GetOneAsync().ConfigureAwait(true);
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await userService.UpdateAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            var data = await userService.GetOneAsync(q => q.Id.Equals(user.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async Task UpdateAsync_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(1)).ConfigureAwait(true);
            var user = await userService.GetOneAsync().ConfigureAwait(true);
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await userService.UpdateAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            var data = await userService.GetOneAsync(q => q.Id.Equals(user.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async Task UpdateAsync_Again_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(1)).ConfigureAwait(true);
            var user = await userService.GetOneAsync().ConfigureAwait(true);
            user!.State = (int)StateEnum.Disable;
            user.DepartmentId = 213;
            user.UserName = "Test";
            var result = await userService.UpdateAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            var data = await userService.GetOneAsync(q => q.Id.Equals(user.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);

            user.State = (int)StateEnum.Enabled;
            user.DepartmentId = 567;
            user.UserName = "Test213";
            result = await userService.UpdateAsync(user).ConfigureAwait(true);
            Assert.Equal(1, result);
            data = await userService.GetOneAsync(q => q.Id.Equals(user.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            Assert.Equal(data.State, user.State);
            Assert.Equal(data.DepartmentId, user.DepartmentId);
            Assert.Equal(data.UserName, user.UserName);
            Assert.Equal(data.NickName, user.NickName);
        }

        [Fact]
        public async Task UpdateAsync_Range_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(12)).ConfigureAwait(true);
            var users = await userService.GetListAsync().ConfigureAwait(true);
            users = [.. users.Take(7).OrderBy(q => q.Id)];
            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                user!.State = (int)StateEnum.Disable;
                user.DepartmentId = i + 1;
                user.UserName = $"Test{i}";
            }

            var result = await userService.UpdateAsync([.. users]).ConfigureAwait(true);
            Assert.Equal(7, result);
            var data = await userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            data = [.. data.OrderBy(q => q.Id)];
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal(data[i].State, users[i].State);
                Assert.Equal(data[i].DepartmentId, users[i].DepartmentId);
                Assert.Equal(data[i].UserName, users[i].UserName);
                Assert.Equal(data[i].NickName, users[i].NickName);
            }
        }

        [Fact]
        public async Task UpdateAsync_Predicate_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(12)).ConfigureAwait(true);

            var users = await userService.GetListAsync().ConfigureAwait(true);
            users = [.. users.Take(6).OrderBy(q => q.Id)];

            var result = await userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            }).ConfigureAwait(true);
            Assert.Equal(6, result);
            var data = await userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            data = [.. data.OrderBy(q => q.Id)];
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal(data[i].State, users[i].State);
                Assert.Equal(data[i].DepartmentId, users[i].DepartmentId);
                Assert.Equal(data[i].UserName, users[i].UserName);
                Assert.Equal(data[i].NickName, users[i].NickName);
            }
        }

        [Fact]
        public async Task UpdateAsync_PredicateReturnEmpty_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(12)).ConfigureAwait(true);

            var users = new List<User>();

            var result = await userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            }).ConfigureAwait(true);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task UpdateAsync_PredicateOrScalarIsNull_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(12)).ConfigureAwait(true);

            var users = new List<User>();

            var result = await userService.UpdateAsync(null!, (i, q) =>
            {
                q.State = (int)StateEnum.Disable;
                q.DepartmentId = i + 1;
                q.UserName = $"Test{i}";
            }).ConfigureAwait(true);
            Assert.Equal(0, result);

            result = await userService.UpdateAsync(q => users.Select(q => q.Id).Contains(q.Id), null!).ConfigureAwait(true);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RemoveAsync_PredicateReturnEmptyOrNull_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var users = new List<User>();

            var result = await userService.RemoveAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.Equal(0, result);

            result = await userService.RemoveAsync(null!).ConfigureAwait(true);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RemoveAsync_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var users = await userService.GetListAsync().ConfigureAwait(true);
            users = [.. users.Take(6).OrderBy(q => q.Id)];

            var result = await userService.RemoveAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.Equal(6, result);

            var data = await userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.NotNull(data);
            for (int i = 0; i < data.Count; i++)
            {
                Assert.Equal((int)StateEnum.Disable, users[i].State);
            }
        }

        [Fact]
        public async Task HardRemoveAsync_PredicateReturnEmptyOrNull_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var users = new List<User>() { new() { Id = 1, UserName = "ex" } };

            var result = await userService.HardRemoveAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.Equal(0, result);

            result = await userService.HardRemoveAsync(null!).ConfigureAwait(true);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task HardRemoveAsync_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var users = await userService.GetListAsync().ConfigureAwait(true);
            users = [.. users.Take(6).OrderBy(q => q.Id)];

            var result = await userService.HardRemoveAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.Equal(6, result);

            var data = await userService.GetListAsync(q => users.Select(q => q.Id).Contains(q.Id)).ConfigureAwait(true);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetListAsync_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var result = (await userService.GetListAsync().ConfigureAwait(true)).Select(q => new TestSubModel()
            {
                Id = q.Id,
                CardNum = q.UserName,
                ExpiresIn = q.CreatedTime
            }).ToList();

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await userService.GetOneAsync(q => q.Id.Equals(result[i].Id)).ConfigureAwait(true);
                Assert.Equal(result[i].CardNum, user!.UserName);
                Assert.Equal(result[i].ExpiresIn, user.UpdatedTime);
            }
        }

        [Fact]
        public async Task GetListAsync_WithPredicate_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var result = (await userService.GetListAsync(q => true).ConfigureAwait(true)).Select(q => new TestSubModel() { Id = q.Id, CardNum = q.UserName, ExpiresIn = q.CreatedTime }).ToList();

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await userService.GetOneAsync(q => q.Id.Equals(result[i].Id)).ConfigureAwait(true);
                Assert.Equal(result[i].CardNum, user!.UserName);
                Assert.Equal(result[i].ExpiresIn, user.CreatedTime);
            }
        }



        [Fact]
        public async Task GetSelectListAsyncAsync_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var result = await userService.GetSelectListAsync(q => new TestSubModel()
            {
                Id = q.Id,
                CardNum = q.UserName,
                ExpiresIn = q.CreatedTime
            }).ConfigureAwait(true);

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await userService.GetOneAsync(q => q.Id.Equals(result[i].Id)).ConfigureAwait(true);
                Assert.Equal(result[i].CardNum, user!.UserName);
            }
        }

        [Fact]
        public async Task GetSelectListAsync_WithPredicate_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var result = await userService.GetSelectListAsync(q => new TestSubModel() { Id = q.Id, CardNum = q.UserName }, q => true).ConfigureAwait(true);

            Assert.Equal(10, result.Count);

            for (int i = 0; i < result.Count; i++)
            {
                var user = await userService.GetOneAsync(q => q.Id.Equals(result[i].Id)).ConfigureAwait(true);
                Assert.Equal(result[i].CardNum, user!.UserName);
            }
        }

        [Fact]
        public async Task GetSelectListAsync_ScalarIsNull_Correctly()
        {
            userService.Context.Users.ExecuteDelete();
            await userService.AddAsync(new EFUserFaker().Generate(10)).ConfigureAwait(true);

            var result = await userService.GetSelectListAsync<TestSubModel>(null!).ConfigureAwait(true);

            Assert.Empty(result);
        }
    }
}