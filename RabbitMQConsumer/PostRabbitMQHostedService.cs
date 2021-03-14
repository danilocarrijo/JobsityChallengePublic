using System;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQConsumer
{
    public class PostRabbitMQHostedService
    {
        public void Post(string message)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "demo.exchange",
                                         routingKey: "demo.queue.*",
                                         basicProperties: null,
                                         body: body);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
