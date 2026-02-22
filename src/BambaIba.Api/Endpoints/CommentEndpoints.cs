using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Extensions;
using BambaIba.Application.Features.Comments.AddComment;
using BambaIba.Application.Features.Comments.AddReactionToComment;
using BambaIba.Application.Features.Comments.DeleteComment;
using BambaIba.Application.Features.Comments.EditComment;
using BambaIba.Application.Features.Comments.GetComments;
using BambaIba.Application.Features.Comments.GetReplies;
using BambaIba.SharedKernel;
using Carter;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace BambaIba.Api.Endpoints;

public class CommentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/comments")
            .WithTags("Comments");


        group.MapPost("/{mediaId:guid}/comments", AddComment)
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithValidation<AddCommentCommand>()
            .WithName("CreateComment");

        group.MapGet("/{mediaId:guid}/comments", GetComments)
            .Produces<CursorPagedResult<CommentDto>>(StatusCodes.Status200OK)
            .WithName("GetComments");

        group.MapPut("/{commentId}", UpdateComment)
            //group.MapPut("/{commentId:guid}", UpdateComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .WithName("UpdateComment");

        group.MapDelete("/{commentId}", DeleteComment)
            //group.MapDelete("/{commentId:guid}", DeleteComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .WithName("DeleteComment");

        group.MapGet("/{commentId:guid}/replies", GetReplies)
            .Produces<CursorPagedResult<CommentDto>>(StatusCodes.Status200OK)
            .WithName("GetReplies");

        group.MapPost("/{commentId}/reaction", AddReaction)
            .Produces<PagedResult<CommentDto>>(StatusCodes.Status200OK)
            .WithName("GetReplies");
    }

    private static async Task<IResult> AddComment(
        Guid mediaId,
        [FromBody] AddCommentCommand command,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        AddCommentCommand cmdWithId = command with { MediaId = mediaId };

        Result<string> result =
            await bus.InvokeAsync<Result<string>>(
            cmdWithId, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetComments(
        [AsParameters] GetCommentsQuery query,
        IMessageBus bus, CancellationToken cancellationToken)
    {
        Result<CursorPagedResult<CommentDto>> result =
             await bus.InvokeAsync<Result<CursorPagedResult<CommentDto>>>(
             query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> UpdateComment(
        string commentId,
        [FromBody] EditCommentCommand command,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        EditCommentCommand cmd = command with { CommentId = commentId };

        Result<Result> result =
            await bus.InvokeAsync<Result>(command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> DeleteComment(
        string commentId,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        Result<Result> result = await bus.InvokeAsync<Result>(new DeleteCommentCommand(commentId), cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetReplies(
        string commentId,
        string? cursor,
        IMessageBus bus,
        CancellationToken cancellationToken,
        int pageSize = 25)
    {
        var query = new GetRepliesQuery
        (
            Guid.Parse(commentId),
            cursor,
            pageSize,
            CurrentUserId: null
        );

        Result<CursorPagedResult<CommentDto>> result =
            await bus.InvokeAsync<Result<CursorPagedResult<CommentDto>>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> AddReaction(
        string commentId,
        [FromBody] AddReactionToCommentCommand command,
        IMessageBus bus,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        AddReactionToCommentCommand cmdWithId = command with { CommentId = commentId };

        Result<Result> result =
            await bus.InvokeAsync<Result>(
            cmdWithId, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
