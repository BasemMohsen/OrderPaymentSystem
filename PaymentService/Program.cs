using PaymentService.Consumers;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<StripePaymentProcessor>();
builder.Services.AddHostedService<OrderCreatedConsumer>();
builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
