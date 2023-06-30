using MessageNowApi.Dtos;
using MessageNowApi.Models;

namespace MessageNowApi.Services
{
    public interface IConversationService
    {
        public Task<IEnumerable<ConversationDto>> GetConversationsForUser(string username);
        public Task<FullConversationDto?> GetConversationById(int conversationId, string username);
        public Task AddConversation(Conversation conversation);
        public Task<Conversation?> GetConversationModel(int conversationId);
    }
}
