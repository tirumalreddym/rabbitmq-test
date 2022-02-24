using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rabbitmq_test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumeMessageController : ControllerBase
    {
        private readonly ILogger<ConsumeMessageController> _logger;

        public ConsumeMessageController(ILogger<ConsumeMessageController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<string> consumeMessage()
        {
            List<string> messages = new List<string>();
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    messages.Add(message);
                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);
            }
            return messages;
        }
    }
}
