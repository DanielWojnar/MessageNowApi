using MessageNowApi.Data;
using MessageNowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageNowApi.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityContext _context;
        public UserService(IdentityContext context) {
            _context = context;
        }

        public async Task<MessageNowUser?> GetUserByUsername(string username)
        {
            return await (from u in _context.Users
                          where u.UserName == username
                          select u).FirstOrDefaultAsync();
        }
    }
}
