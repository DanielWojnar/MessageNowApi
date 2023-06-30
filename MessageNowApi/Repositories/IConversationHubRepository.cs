namespace MessageNowApi.Repositories
{
    public interface IConversationHubRepository
    {
        public void AddUser(string username, int conversationId, string connectionId);
        public Tuple<int, string>? GetUser(string username);
        public void RemoveUser(string username);
    }
}
