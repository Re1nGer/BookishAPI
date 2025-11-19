namespace BookishAPI;

public class StripeModels
{
    public class CreateCheckoutSessionRequest
    {
        public string PriceId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }

    public class CreateCheckoutSessionResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
    }

    public class SubscriptionStatus
    {
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? PlanType { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
    }
}