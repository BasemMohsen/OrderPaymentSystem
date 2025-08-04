using Microsoft.EntityFrameworkCore.Metadata;
using OrderService.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderService.Services;

public class RabbitMqPublisher
{
    private readonly IConnection _connection;
    private readonly RabbitMQ.Client.IModel _channel;

    public RabbitMqPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost"
        };

        _connection = factory.CreateConnection();   // This should now work
        _channel = _connection.CreateModel();       // This too

        _channel.QueueDeclare(
            queue: "order-created",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public void Publish(OrderCreatedEvent orderEvent)
    {
        var json = JsonSerializer.Serialize(orderEvent);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: "",
            routingKey: "order-created",
            basicProperties: null,
            body: body
        );
    }
}
