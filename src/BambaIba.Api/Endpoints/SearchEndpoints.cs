// Fichier: Endpoints/SearchEndpoints.cs
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Features.Searchs;
using BambaIba.SharedKernel;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BambaIba.Api.Endpoints;

public class SearchEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/search")
            .WithTags("Search");

        group.MapGet("/", async (
            string term,
            int limit,
            IMessageBus bus,
            CancellationToken ct) =>
        {
            var query = new SearchMediaQuery(term, limit == 0 ? 20 : limit);

            // Appel au handler via Wolverine
            Result<CursorPagedResult<MediaDto>> result = await bus.InvokeAsync<Result<CursorPagedResult<MediaDto>>>(query, ct);

            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Error);
        })
        .WithName("SearchMedia")
        .Produces<CursorPagedResult<MediaDto>>()
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}
