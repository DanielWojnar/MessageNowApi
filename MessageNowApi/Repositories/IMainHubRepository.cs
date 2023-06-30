namespace MessageNowApi.Repositories
{
    public interface IMainHubRepository
    {
        public void AddUser(string username, string connectionId);
        public string? GetUser(string username);
        public void RemoveUser(string username);

    }
}
