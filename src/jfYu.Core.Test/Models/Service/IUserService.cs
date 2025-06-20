#if NET8_0_OR_GREATER
using jfYu.Core.Data.Service;
using jfYu.Core.Test.Models.Entity;

namespace jfYu.Core.Test.Models.Service
{
    public interface IUserService : IService<User, DataContext>
    {
        Task<User?> GetByNickNameAsync(string nickName);
    }
}
#endif