using MessageNowApi.Models;

namespace MessageNowApi.Services
{
    public interface IUserService
    {
        public Task<MessageNowUser?> GetUserByUsername(string username);
    }
}
