namespace MessageNowApi.Repositories
{
    public class ConversationHubRepository : IConversationHubRepository
    {
        private Dictionary<string, Tuple<int, string>> _users; //<Username, <ConversationId, ConnectionId>
        public ConversationHubRepository()
        {
            _users = new Dictionary<string, Tuple<int, string>>();
        }

        public void AddUser(string username, int conversationId, string connectionId)
        {
            _users.Add(username, new Tuple<int, string>(conversationId, connectionId));
        }

        public Tuple<int, string>? GetUser(string username)
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
