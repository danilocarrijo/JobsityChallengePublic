using Entities;
using RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class MessageRepository : IMessageRepository
    {
        public readonly ChatDbContext _context;

        public MessageRepository(ChatDbContext context)
        {
            _context = context;
        }

        public void Add(Message entity)
        {
            try
            {
                _context.Messages.Add(entity);

                _context.SaveChanges();
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
                return _context.Messages.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
