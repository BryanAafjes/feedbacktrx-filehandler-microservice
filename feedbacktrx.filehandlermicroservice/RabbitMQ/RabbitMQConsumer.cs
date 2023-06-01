using feedbacktrx.filehandlermicroservice.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly string _routingKey;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(string rabbitMQConnectionString, string exchangeName, string queueName, string routingKey, string username, string password, IServiceProvider serviceProvider)
    {
        Uri uri = new Uri(rabbitMQConnectionString);

        var factory = new ConnectionFactory
        {
            HostName = uri.Host,
            Port = uri.Port,
            UserName = username,
            Password = password,
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _exchangeName = exchangeName;
        _queueName = queueName;
        _routingKey = routingKey;
        _serviceProvider = serviceProvider;

        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(_queueName, _exchangeName, _queueName, null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // Process the message and retrieve the username based on the userID
            var username = await ProcessMessage(message);

            var properties = _channel.CreateBasicProperties();
            properties.CorrelationId = eventArgs.BasicProperties.CorrelationId;
            var replyBody = Encoding.UTF8.GetBytes(username);
            _channel.BasicPublish(_exchangeName, eventArgs.BasicProperties.ReplyTo, properties, replyBody);
        };

        _channel.BasicConsume(_queueName, true, consumer);

        await Task.CompletedTask;
    }

    private async Task<string> ProcessMessage(string message)
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

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();

        base.Dispose();
    }
}
