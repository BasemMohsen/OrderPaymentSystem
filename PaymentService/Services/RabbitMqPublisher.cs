using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using PaymentService.Events;

namespace PaymentService.Services;

public class RabbitMqPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "payment-completed",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public void Publish(PaymentCompletedEvent evt)
    {
        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "",
                              routingKey: "payment-completed",
                              basicProperties: null,
                              body: body);
    }
}
