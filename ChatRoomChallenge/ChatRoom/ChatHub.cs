using ChatRoomChallenge.ChatRoom;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using ServicesInterface;
using System;
using System.IO;
using System.Net;
using System.Text;
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

        private readonly BotSettings _options;

        public ChatHub(
                   IOptions<BotSettings> options,
                   IMessageService messageService)
        {
            _messageService = messageService;
            _options = options.Value;
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

            Clients.All.SendAsync("ReceiveMessage", string.Empty, string.Empty, ChatHub.Events.REFRESH);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message, string who)
        {
            if (message.StartsWith("/"))
            {
                try
                {
                    await BotMessage(user, message);                    
                    await Clients.Client(UserHandler._connections.GetConnection(user)).SendAsync("ReceiveMessage", "Bot", "Message sent to Bot", Events.MESSAGE);
                }
                catch (Exception ex)
                {
                    await Clients.Client(UserHandler._connections.GetConnection(user)).SendAsync("ReceiveMessage", "Bot", ex.Message, Events.MESSAGE);
                }
            }
            else
            {
                _messageService.Add(new Message
                {
                    User = user,
                    MessageStrin = message,
                    MessageDateMessage = DateTime.Now
                });

                if (who.Equals("0"))
                    await Clients.All.SendAsync("ReceiveMessage", user, message, Events.MESSAGE);
                else
                    await Clients.Client(UserHandler._connections.GetConnection(who)).SendAsync("ReceiveMessage", user, message, Events.MESSAGE);
            }

        }

        public async Task BotMessage(string user, string message)
        {
            try
            {
                var request = WebRequest.CreateHttp($"{_options.Url}/bot/{user}/{message.Replace("/","")}");
                request.Method = "POST";
                request.ContentType = "application/json";
                var resposta = request.GetResponse();
                
            }
            catch (WebException ex)
            {
                var response = ex.Response.GetResponseStream();
                StreamReader reader = new StreamReader(response);
                object objResponse = reader.ReadToEnd();
                await Clients.Client(UserHandler._connections.GetConnection(user)).SendAsync("ReceiveMessage", "bot", objResponse, Events.MESSAGE);
            }
        }

        public string GetConnectionId() => Context.ConnectionId;

    }
}
