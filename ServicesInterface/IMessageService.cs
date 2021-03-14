using Entities;
using System;
using System.Collections.Generic;

namespace ServicesInterface
{
    public interface IMessageService
    {
        void Add(Message entity);

        List<Message> GetAll();
    }
}
