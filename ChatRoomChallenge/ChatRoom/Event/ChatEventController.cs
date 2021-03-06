using ChatRoomChallenge.ChatRoom;
using ChatRoomChallenge.Hubs.ChatCenter;
using ChatTask.Hubs;
using Events;
using Microsoft.AspNetCore.SignalR;
using RabbitMQConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Hubs.ChatEvent
{
    public class ChatEventController
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatEventController(IHubContext<ChatHub> chatHub, EventDispacher dispacher
            )
        {

            _chatHub = chatHub;
            dispacher.OnNewUser += new EventDispacher.NewUserHandler(OnNewUser);
            dispacher.OnRefreshUserList += new EventHandler(OnRefreshUserList);
            dispacher.OnRabbitMQMessage += new EventDispacher.RabbitMQMessageHandler(OnRabbitMQMessage);
        }


        public async void OnRabbitMQMessage(object sender, RabbitMQMessageEventArgs e)
        {
            await _chatHub.Clients.Client(UserHandler._connections.GetConnection(e.user)).SendAsync("ReceiveMessage", "bot", e.message, ChatHub.Events.MESSAGE);
        }


        public async void OnNewUser(object sender, NewUserEventArgs e)
        {
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", e.user.Name, e.user.signalRId, ChatHub.Events.NEWUSER);
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "Chat", $"New user {e.user.Name}", ChatHub.Events.MESSAGE);
        }

        public async void OnRefreshUserList(object sender, EventArgs e)
        {
            await _chatHub.Clients.All.SendAsync("ReceiveMessage",string.Empty, string.Empty, ChatHub.Events.REFRESH);
        }

    }
}
