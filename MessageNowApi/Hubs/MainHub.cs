using MessageNowApi.Dtos;
using MessageNowApi.Models;
using MessageNowApi.Repositories;
using MessageNowApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace MessageNowApi.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {
        private readonly IMainHubRepository _mainHubRepository;
        private readonly IConversationService _conversationService;
        private readonly IUserService _userService;
        public MainHub(IMainHubRepository mainHubRepository, IConversationService conversationService, IUserService userService)
        {
            _mainHubRepository = mainHubRepository;
            _conversationService = conversationService;
            _userService = userService;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User.FindFirst(JwtRegisteredClaimNames.Name).Value;
            if(username == null)
            {
                return;
            }
            _mainHubRepository.AddUser(username, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task GetConversations(string username)
        {
            var conversations = await _conversationService.GetConversationsForUser(username);
            await Clients.Caller.SendAsync("GetConversations", conversations);
        }

        public async Task CreateConversation(string username, string friend)
        {
            var conversation = new Conversation
            {
                LastMessage = null,
                Messages = new List<Message>(),
            };
            conversation.UserConvConnectors = new List<UserConvConnector>
            {
                new UserConvConnector{Conversation = conversation, User = await _userService.GetUserByUsername(username) },
                new UserConvConnector{Conversation = conversation, User = await _userService.GetUserByUsername(friend) },
            };
            await _conversationService.AddConversation(conversation);
            var conversationDto = new ConversationDto
            {
                Id = conversation.Id,
                Messages = conversation.Messages,
                LastMessage = conversation.LastMessage,
                Name = friend,
            };
            await Clients.Caller.SendAsync("CreateConversation", conversationDto);
            var friendConnectionId = _mainHubRepository.GetUser(friend);
            if (friendConnectionId != null)
            {
                await Clients.Client(friendConnectionId).SendAsync("CreateConversation", conversationDto);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User.FindFirst(JwtRegisteredClaimNames.Name).Value;
            if (username == null)
            {
                return;
            }
            _mainHubRepository.RemoveUser(username);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
