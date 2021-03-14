using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepositoryInterface
{
    public interface IMessageRepository
    {
        void Add(Message entity);

        List<Message> GetAll();
    }
}
