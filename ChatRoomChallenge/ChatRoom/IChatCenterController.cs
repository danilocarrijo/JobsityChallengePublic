using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatRoomChallenge.Hubs.ChatCenter
{
    public interface IChatCenterController
    {
        void AddUser(AppUser user);
        List<string> GetUsers();

        void UpdateUser(AppUser user);
    }
}
