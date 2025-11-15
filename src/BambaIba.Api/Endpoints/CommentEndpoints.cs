using System.Security.Claims;
using BambaIba.Api.Extensions;
using BambaIba.Api.Infrastructure;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Extensions;
using BambaIba.Application.Features.Audios.GetAudios;
using BambaIba.Application.Features.Comments.CreateComment;
using BambaIba.Application.Features.Comments.DeleteComment;
using BambaIba.Application.Features.Comments.GetComments;
using BambaIba.Application.Features.Comments.GetReplies;
using BambaIba.Application.Features.Comments.UpdateComment;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Carter;
using Cortex.Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BambaIba.Api.Endpoints;

public class CommentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/comments")
            .WithTags("Comments")
            .WithOpenApi();


        group.MapPost("/", CreateComment)
            .RequireAuthorization()
            .Produces<CreateCommentResult>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithValidation<CreateCommentRequest>()
            .WithName("CreateComment");

        group.MapGet("/", GetComments)
            .Produces<PagedResult<CommentDto>>(StatusCodes.Status200OK)
            .WithName("GetComments");

        group.MapPut("/", UpdateComment)
            //group.MapPut("/{commentId:guid}", UpdateComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .WithName("UpdateComment");

        group.MapDelete("/", DeleteComment)
            //group.MapDelete("/{commentId:guid}", DeleteComment)
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .WithName("DeleteComment");

        group.MapGet("/{commentId:guid}/replies", GetReplies)
            .Produces<PagedResult<CommentDto>>(StatusCodes.Status200OK)
            .WithName("GetReplies");
    }

    private static async Task<IResult> CreateComment(
        //Guid videoId,
        [FromBody] CreateCommentRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new CreateCommentCommand
        {
            VideoId = request.VideoId,
            //UserId = userId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId
        };

        Result<CreateCommentResult> result =
            await mediator.SendCommandAsync<CreateCommentCommand, Result<CreateCommentResult>>(
            command,
            cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetComments(
        Guid videoId,
        [FromBody] GetCommentsRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentsQuery
        {
            VideoId = videoId,
            Page = request.Page,
            PageSize = request.PageSize
        };

        Result<PagedResult<CommentDto>> result =
            await mediator.SendQueryAsync<GetCommentsQuery, Result<PagedResult<CommentDto>>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> UpdateComment(
        [FromBody] UpdateCommentRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        var command = new UpdateCommentCommand(
            request.VideoId,
            request.CommentId,
            request.Content);

        Result<UpdateCommentResult> result = await mediator
            .SendCommandAsync<UpdateCommentCommand, Result<UpdateCommentResult>>(
            command,
            cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> DeleteComment(
        [FromBody] DeleteCommentRequest request,
        IMediator mediator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user.FindFirstValue("sub");

        var command = new DeleteCommentCommand(
            request.CommentId, request.VideoId);

        Result<DeleteCommentResult> result = await mediator
            .SendCommandAsync<DeleteCommentCommand, Result<DeleteCommentResult>>(
            command, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }

    private static async Task<IResult> GetReplies(
        Guid videoId, Guid commentId,
        IMediator mediator, 
        CancellationToken cancellationToken)
    {
        var query = new GetRepliesQuery
        {
            VideoId = videoId,
            ParentCommentId = commentId
        };

        Result<PagedResult<CommentDto>> result = await mediator.
            SendQueryAsync<GetRepliesQuery, Result<PagedResult<CommentDto>>>(
            query, cancellationToken);

        return result.Match(Results.Ok, CustomResults.Problem);
    }
}
