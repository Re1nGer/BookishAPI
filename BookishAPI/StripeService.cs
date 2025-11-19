using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace BookishAPI;

public class StripeService
{
    private readonly IConfiguration _configuration;
    private readonly BookAppContext  _dbContext;

    public StripeService(IConfiguration configuration, BookAppContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<Session> CreateCheckoutSessionAsync(string priceId, string userId)
    {
        var options = new SessionCreateOptions
        {
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1,
                },
            },
            SuccessUrl = "myapp://subscription-success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "myapp://subscription-cancel",
            ClientReferenceId = userId, // Link to your user
            Metadata = new Dictionary<string, string>
            {
                { "userId", userId }
            }
        };

        var service = new SessionService();
        return await service.CreateAsync(options);
    }

    public async Task<StripeModels.SubscriptionStatus> GetSubscriptionStatusAsync(string customerId)
    {
        var service = new SubscriptionService();
        var subscriptions = await service.ListAsync(new SubscriptionListOptions
        {
            Customer = customerId,
            Status = "all",
            Limit = 1
        });

        var subscription = subscriptions.Data.FirstOrDefault();

        if (subscription == null)
        {
            return new StripeModels.SubscriptionStatus
            {
                Status = "none",
                IsActive = false
            };
        }

        return new StripeModels.SubscriptionStatus
        {
            Status = subscription.Status,
            IsActive = subscription.Status is "active" or "trialing",
            PlanType = subscription.Items.Data.FirstOrDefault()?.Price.Nickname,
        };
    }

    public async Task<bool> CancelSubscriptionAsync(string subscriptionId)
    {
        var service = new SubscriptionService();
        var subscription = await service.CancelAsync(subscriptionId);
        return subscription.Status == "canceled";
    }
    
    public async Task<(string ClientSecret, string CustomerId)> CreateSubscriptionPaymentSheetAsync(
        string priceId, 
        string userId)
    {
        var parsedUserId = Guid.Parse(userId);
        var userEmail = (await _dbContext.Users.FirstOrDefaultAsync(a => a.Id == parsedUserId))?.Email;
        // Get or create customer
        var customerService = new CustomerService();
        var customers = await customerService.ListAsync(new CustomerListOptions
        {
            Email = userEmail, // Use actual user email
            Limit = 1
        });

        Customer customer;
        if (customers.Data.Count > 0)
        {
            customer = customers.Data[0];
        }
        else
        {
            customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = userEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId }
                }
            });
        }
        var paymentSettings = new SubscriptionPaymentSettingsOptions {
            SaveDefaultPaymentMethod = "on_subscription",
        };

        // Create subscription with payment setup
        var subscriptionService = new SubscriptionService();
        
        var subscriptionOptions = new SubscriptionCreateOptions
        {
            Customer = customer.Id,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    Price = priceId,
                },
            },
            PaymentSettings = paymentSettings,
            PaymentBehavior = "default_incomplete",
            BillingMode = new SubscriptionBillingModeOptions
            {
                Type = "flexible",
            },
        };
        subscriptionOptions.AddExpand("latest_invoice.confirmation_secret");
        
        var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

        var clientSecret = subscription.LatestInvoice.ConfirmationSecret.ClientSecret;
    
        return (clientSecret, customer.Id);
    }
}