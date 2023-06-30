namespace MessageNowApi.Services
{
    public interface ITokenService
    {
        public Task<string> CreateToken(string username);
        public Task<string> CreateRefreshToken(string username);
        public Guid? GetUserIdByToken(string token);
        public Task<string?> CreateTokenByRefreshToken(string refreshToken);
        public Task DeactiveRefreshToken(string token);
    }
}
