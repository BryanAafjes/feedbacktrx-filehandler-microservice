using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using feedbacktrx.filehandlermicroservice.Service;

namespace feedbacktrx.filehandlermicroservice.RabbitMQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly RabbitMQConnection _rabbitMQConfig;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumer(RabbitMQConnection rabbitMQConfig, IServiceProvider serviceProvider)
        {
            _rabbitMQConfig = rabbitMQConfig;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Uri uri = new Uri(_rabbitMQConfig.Hostname);

            var factory = new ConnectionFactory
            {
                HostName = uri.Host,
                Port = uri.Port,
                UserName = _rabbitMQConfig.Username,
                Password = _rabbitMQConfig.Password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "deleteposts",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine("Received message: " + message);

                    // Process the message and retrieve the username from the database
                    string response = await ProcessMessage(ea, message);

                    // Respond with the username
                    Respond(channel, ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
                };

                channel.BasicConsume(queue: "deleteposts",
                                      autoAck: true,
                                      consumer: consumer);

                Console.WriteLine("Listening for messages...");

                // Wait for the cancellation token to be triggered (e.g., when the application shuts down)
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
        }

        private async Task<string> ProcessMessage(BasicDeliverEventArgs ea, string message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var fileHandlerService = scope.ServiceProvider.GetRequiredService<IFileHandlerService>();

                var result = await DeleteFileFromBlobStorage(fileHandlerService, message);
                return result;
            }
        }

        private async Task<string> DeleteFileFromBlobStorage(IFileHandlerService fileHandlerService, string message)
        {
            bool result = await fileHandlerService.DeleteFileFromBlobStorage(message);
            return result.ToString();
        }

        private void Respond(IModel channel, string replyTo, string correlationId, string username)
        {
            var responseProperties = channel.CreateBasicProperties();
            responseProperties.CorrelationId = correlationId;

            var responseMessage = Encoding.UTF8.GetBytes(username);

            channel.BasicPublish(exchange: "",
                                  routingKey: replyTo,
                                  basicProperties: responseProperties,
                                  body: responseMessage);
        }
    }
}
