using ChatRoomChallenge.ChatRoom;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ServicesInterface;
using System;
using System.Threading.Tasks;

namespace ChatTask.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        

        public enum Events
        {
            MESSAGE = 0,
            NEWUSER = 1,
            REFRESH = 2
        }
        private readonly IMessageService _messageService;

        public ChatHub(
                   IMessageService messageService)
        {
            _messageService = messageService;
        }

        public override Task OnConnectedAsync()
        {
            string name = Context.User.Identity.Name;

            UserHandler._connections.AddOrUpdateConnection(name, Context.ConnectionId);

            Clients.All.SendAsync("ReceiveMessage", string.Empty, string.Empty, ChatHub.Events.REFRESH);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string name = Context.User.Identity.Name;

            UserHandler._connections.Remove(name);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message, string who)
        {

            _messageService.Add(new Message
            {
                User = user,
                MessageStrin = message,
                MessageDateMessage = DateTime.Now
            });

            if(who.Equals("0"))
                await Clients.All.SendAsync("ReceiveMessage", user, message, Events.MESSAGE);
            else
                await Clients.Client(UserHandler._connections.GetConnection(who)).SendAsync("ReceiveMessage", user, message, Events.MESSAGE);

        }

        public string GetConnectionId() => Context.ConnectionId;

    }
}
