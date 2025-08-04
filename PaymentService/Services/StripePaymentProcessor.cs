using Stripe;

namespace PaymentService.Services;

public class StripePaymentProcessor
{
    private readonly IConfiguration _config;

    public StripePaymentProcessor(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<PaymentIntent> ProcessPaymentAsync(decimal amount, string orderId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(amount * 100), // Stripe uses cents
            Currency = "usd",
            Description = $"Order #{orderId}",
            PaymentMethodTypes = new List<string> { "card" },
        };

        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    }
}
