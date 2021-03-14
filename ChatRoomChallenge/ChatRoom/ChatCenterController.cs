using ChatRoomChallenge.ChatRoom;
using ChatTask.Hubs;
using Entities;
using Events;
using Microsoft.AspNetCore.SignalR;
using RabbitMQConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Hubs.ChatCenter
{
    public class ChatCenterController : IChatCenterController
    {
        private List<AppUser> ChatUsers { get; set; }
        private readonly EventDispacher _dispacher;

        public ChatCenterController(EventDispacher dispacher)
        {
            ChatUsers = new List<AppUser>();
            _dispacher = dispacher;
        }

        public void AddUser(AppUser user)
        {
            ChatUsers.Add(user);
            _dispacher.NewChatUser(user);
        }

        public List<string>  GetUsers()
        {
            return UserHandler._connections.GetConnectionUserNames();
        }

        public void UpdateUser(AppUser user)
        {
            var chatUser = ChatUsers.Where(x => x.Name.Equals(user.Name)).FirstOrDefault();
            chatUser.signalRId = user.signalRId;
            _dispacher.RefreshUserList();
        }

    }
}
