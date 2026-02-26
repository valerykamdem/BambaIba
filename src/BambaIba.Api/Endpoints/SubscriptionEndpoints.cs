using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Features.ToggleSubscriptions;
using BambaIba.SharedKernel;
using Carter;
using Wolverine;

namespace BambaIba.Api.Endpoints;

// BambaIba.Api/Endpoints/MediaEndpoints.cs
public class SubscriptionEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/users")
            .WithTags("Users");

        // Upload vidéo (multipart/form-data)
        group.MapPost("/{id}", Subscribe)
            .RequireAuthorization()
            .WithName("Subscribe");
    }

    // Handler pour Upload (avec Request object)
    private static async Task<IResult> Subscribe(
        Guid id,
        IMessageBus bus, ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {

        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new ToggleSubscriptionCommand(id);


        Result<Result> result =
            await bus.InvokeAsync<Result>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

}
