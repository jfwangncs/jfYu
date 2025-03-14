using jfYu.Core.Data.Context;
using jfYu.Core.Data.Service;
using jfYu.Core.Test.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace jfYu.Core.Test.Models.Service
{
    public class UserService(DataContext context, ReadonlyDBContext<DataContext> readonlyDBContext)
    : Service<User, DataContext>(context, readonlyDBContext), IUserService
    {
        public async Task<User?> GetByNickNameAsync(string nickName)
        {
            return await Context.Set<User>().FirstOrDefaultAsync(u => u.NickName == nickName);
        }
    }
}
