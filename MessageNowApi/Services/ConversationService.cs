using MessageNowApi.Data;
using MessageNowApi.Dtos;
using MessageNowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageNowApi.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IdentityContext _context;
        public ConversationService(IdentityContext cotext) {
            _context = cotext;
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsForUser(string username)
        {
            return await (from ucc in _context.UserConvConnectors
                          join c in _context.Conversations on ucc.Conversation equals c
                          where ucc.User.UserName == username
                          select new ConversationDto
                          {
                              Id = c.Id,
                              Messages = null,
                              LastMessage = c.LastMessage,
                              Name = c.UserConvConnectors.First(x => !x.User.UserName.Equals(username)).User.UserName
                          }).ToListAsync();
        }

        public async Task<FullConversationDto?> GetConversationById(int conversationId, string username)
        {
            return await (from c in _context.Conversations
                          where c.Id == conversationId
                          select new FullConversationDto
                          {
                              Id = c.Id,
                              Messages = c.Messages,
                              Username = c.UserConvConnectors.First(x => !x.User.UserName.Equals(username)).User.UserName
                          }).FirstOrDefaultAsync();
        }

        public async Task<Conversation?> GetConversationModel(int conversationId)
        {
            return await (from c in _context.Conversations
                          where c.Id == conversationId
                          select c).FirstOrDefaultAsync();
        }

        public async Task AddConversation(Conversation conversation)
        {
            await _context.Conversations.AddAsync(conversation);
            await _context.UserConvConnectors.AddRangeAsync(conversation.UserConvConnectors);
            await _context.SaveChangesAsync();
        }
    }
}
