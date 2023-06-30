using MessageNowApi.Models;

namespace MessageNowApi.Services
{
    public interface IMessageService
    {
        public Task AddMessage(Message message);
        public Task SetLastAsRead(string username, int conversationId);
    }
}
