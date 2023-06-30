using MessageNowApi.Data;
using MessageNowApi.Dtos;
using MessageNowApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageNowApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly IdentityContext _context;
        public MessageService(IdentityContext cotext)
        {
            _context = cotext;
        }

        public async Task AddMessage(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }

        public async Task SetLastAsRead(string username, int conversationId)
        {
            var message = await (from c in _context.Conversations
                                 where c.Id == conversationId
                                 select c.LastMessage).FirstOrDefaultAsync();
            if (message != null && message.User.UserName == username)
            {
                message.Read = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
