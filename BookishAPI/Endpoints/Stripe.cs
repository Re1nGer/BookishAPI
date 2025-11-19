using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BookishAPI.Endpoints;

public static class Stripe
{
    public static RouteGroupBuilder MapStripeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/stripe")
            .WithTags("Stripe")
            .WithOpenApi();
        
        group.MapPost("create-payment-sheet", CreatePaymentSheet)
            .WithName("Creates payment sheet")
            .WithSummary("Creates payment sheet");
        
        group.MapGet("keys", GetPublishableKeys)
            .WithName("Get publishable key")
            .WithSummary("Get publishable key");
        
        return group;
    }
    private static async Task<IResult> CreatePaymentSheet(
        [FromBody] StripeModels.CreateCheckoutSessionRequest request,
        StripeService stripeService,
        IConfiguration configuration)
    {
        try
        {
            var (clientSecret, customerId) = await stripeService
                .CreateSubscriptionPaymentSheetAsync(request.PriceId, request.UserId);

            return Results.Ok(new
            {
                clientSecret,
                customerId,
                publishableKey = configuration["Stripe:PublishableId"]
            });
        }
        catch (StripeException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static IResult GetPublishableKeys(IConfiguration configuration)
    {
        return Results.Ok(new { publishableKey = configuration["Stripe:PublishableId"] });
    }
}