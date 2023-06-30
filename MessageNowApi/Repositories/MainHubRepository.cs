namespace MessageNowApi.Repositories
{
    public class MainHubRepository : IMainHubRepository
    {
        private Dictionary<string, string> _users; //<Username, ConnectionId>
        public MainHubRepository() {
            _users = new Dictionary<string, string>();
        }

        public void AddUser(string username, string connectionId)
        {
            _users.Add(username, connectionId);
        }

        public string? GetUser(string username)
        {
            try
            {
                return _users[username];
            } 
            catch
            {
                return null;
            }
        }

        public void RemoveUser(string username)
        {
            _users.Remove(username);
        }
    }
}
