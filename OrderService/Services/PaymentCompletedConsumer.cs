using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderService.Events;
using OrderService.Data;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Services;

public class PaymentCompletedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _config;
    private IConnection _connection;
    private IModel _channel;

    public PaymentCompletedConsumer(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _config = config;

        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMQ:Host"] ?? "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "payment-completed",
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

            var paymentEvent = JsonSerializer.Deserialize<PaymentCompletedEvent>(message);

            if (paymentEvent != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.Id == paymentEvent.OrderId);

                if (order != null)
                {
                    order.Status = "Paid";
                    await dbContext.SaveChangesAsync();

                    Console.WriteLine($"💰 Order {order.Id} marked as Paid (Stripe ID: {paymentEvent.PaymentIntentId})");
                }
            }
        };

        _channel.BasicConsume(queue: "payment-completed", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
