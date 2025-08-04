using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using PaymentService.Events;
using PaymentService.Services;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PaymentService.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private RabbitMQ.Client.IModel _channel;

    public OrderCreatedConsumer(IConfiguration config, IServiceProvider serviceProvider)
    {
        _config = config;
        _serviceProvider = serviceProvider;

        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "order-created",
                              durable: false,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonSerializer.Deserialize<OrderCreatedEvent>(message);

            if (order != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var stripeService = scope.ServiceProvider.GetRequiredService<StripePaymentProcessor>();

                var intent = await stripeService.ProcessPaymentAsync(order.Price, order.OrderId.ToString());

                Console.WriteLine($"✅ Payment succeeded for Order {order.OrderId}, Stripe PaymentIntent: {intent.Id}");

                // [NEXT STEP]: You will publish a PaymentCompletedEvent here.
                var publisher = scope.ServiceProvider.GetRequiredService<RabbitMqPublisher>();
                publisher.Publish(new PaymentCompletedEvent
                {
                    OrderId = order.OrderId,
                    PaymentIntentId = intent.Id
                });

            }
        };

        _channel.BasicConsume(queue: "order-created", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
