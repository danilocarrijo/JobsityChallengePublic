using Entities;
using Repository;
using RepositoryInterface;
using ServicesInterface;
using System;
using System.Collections.Generic;

namespace Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(ChatDbContext _context)
        {
            _messageRepository = new MessageRepository(_context);
        }

        public void Add(Message entity)
        {
            try
            {
                _messageRepository.Add(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Message> GetAll()
        {
            try
            {
                return _messageRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
