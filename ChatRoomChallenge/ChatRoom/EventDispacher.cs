using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Events
{
    public class NewUserEventArgs : EventArgs
    {
        public NewUserEventArgs(AppUser user)
        {
            this.user = user;
        }

        public AppUser user { get; set; }
    }
    public class RabbitMQMessageEventArgs : EventArgs
    {
        public RabbitMQMessageEventArgs(string user,
            string message)
        {
            this.user = user;
            this.message = message;
        }

        public string user { get; set; }
        public string message { get; set; }
    }

    public class EventDispacher
    {
        public delegate void NewUserHandler(object sender, NewUserEventArgs e);
        public delegate void RabbitMQMessageHandler(object sender, RabbitMQMessageEventArgs e);

        public event NewUserHandler OnNewUser;

        public event EventHandler OnRefreshUserList;

        public event RabbitMQMessageHandler OnRabbitMQMessage;

        public void NewChatUser(AppUser user)
        {
            NewUserEventArgs eve = new NewUserEventArgs(user);

            OnNewUser(this, eve);
        }

        public void RefreshUserList()
        {
            OnRefreshUserList(this, new EventArgs());
        }

        public void RabbitMQMessage(string user,
            string message)
        {
            OnRabbitMQMessage(this, new RabbitMQMessageEventArgs(user, message));
        }
    }
}
