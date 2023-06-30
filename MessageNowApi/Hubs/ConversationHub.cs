using MessageNowApi.Models;
using MessageNowApi.Repositories;
using MessageNowApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace MessageNowApi.Hubs
{
    [Authorize]
    public class ConversationHub : Hub
    {
        private readonly IConversationHubRepository _conversationHubRepo;
        private readonly IMainHubRepository _mainHubRepository;
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IUserService _userService;
        public ConversationHub(IConversationHubRepository conversationHubRepo, IMainHubRepository mainHubRepository, IMessageService messageService, IConversationService conversationService, IUserService userService) {
            _conversationHubRepo = conversationHubRepo;
            _mainHubRepository = mainHubRepository;
            _messageService = messageService;
            _conversationService = conversationService;
            _userService = userService;
        }

        public async Task ConversationOnConnectedAsync(int conversationId)
        {
            var username = Context.User.FindFirst(JwtRegisteredClaimNames.Name).Value;
            if (username == null)
            {
                return;
            }
            _conversationHubRepo.AddUser(username, conversationId, Context.ConnectionId);
        }

        public async Task GetConversation(string username, int conversationId)
        {
            var conversation = await _conversationService.GetConversationById(conversationId, username);
            await _messageService.SetLastAsRead(username, conversationId);
            await Clients.Caller.SendAsync("GetConversation", conversation);
        }

        public async Task CreateMessage(string username, string friend, string content, int conversationId)
        {
            var friendTuple = _conversationHubRepo.GetUser(friend);
            var read = true;
            if (friendTuple == null || friendTuple.Item1 != conversationId)
            {
                read = false;
            }
            var message = new Message
            {
                Content = content,
                Read = read,
                Conversation = await _conversationService.GetConversationModel(conversationId),
                User = await _userService.GetUserByUsername(username)
            };
            await _messageService.AddMessage(message);
            message.Conversation = null;
            await Clients.Caller.SendAsync("CreateMessage", message);
            var friendUser = _mainHubRepository.GetUser(friend);
            if (read)
            {
                await Clients.Client(friendTuple.Item2).SendAsync("CreateMessage", message);
            }
            else if(friendUser != null)
            {
                await Clients.Client(friendUser).SendAsync("CreateMessage", message, conversationId);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User.FindFirst(JwtRegisteredClaimNames.Name).Value;
            if (username == null)
            {
                return;
            }
            _conversationHubRepo.RemoveUser(username);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
