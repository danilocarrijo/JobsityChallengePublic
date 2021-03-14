using Entities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository;
using RepositoryInterface;
using Services;
using ServicesInterface;
using System;

namespace ChatRoomTests
{
    public class MessageServiceTests
    {
        public IMessageService _messageService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ChatDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            _messageService = new MessageService(new ChatDbContext(options));

        }

        [Test]
        public void AddMessage_ShouldNotThrowException()
        {
            try
            {
                _messageService.Add(new Message
                {
                    User = "teste",
                    MessageStrin = "teste",
                    MessageDateMessage = DateTime.Now
                });
            }
            catch (Exception)
            {
                Assert.Fail();
            }

        }

        [Test]
        public void GetAllMessage_ShouldReturnOneMessage()
        {
            try
            {
                _messageService.Add(new Message
                {
                    User = "teste",
                    MessageStrin = "teste",
                    MessageDateMessage = DateTime.Now
                });

                var mesage = _messageService.GetAll();

                Assert.True(mesage.Count != 0 );
            }
            catch (Exception)
            {
                Assert.Fail();
            }

        }
    }
}