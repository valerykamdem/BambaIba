using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Extensions;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.GetReplies;

public sealed record GetRepliesQuery(
    Guid ParentId,
    string? Cursor,
    int PageSize = 5,
    string? CurrentUserId = null // Pour savoir si l'user a liké
);

public sealed class GetRepliesHandler(
    IBIMongoContext mongoContext,
    IUserContextService userContextService,
    //IHttpContextAccessor httpContextAccessor,
    ILogger<GetRepliesHandler> logger
    )
{

    public async Task<Result<CursorPagedResult<CommentDto>>> Handle(GetRepliesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await userContextService
                .GetCurrentContext();

            CursorPagedResult<CommentDto> result = await mongoContext.GetRepliesAsync(
            query.ParentId.ToString(),
            query.Cursor,
            query.PageSize,
            userContext.LocalUserId.ToString(),
            cancellationToken
        );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting replies");
            throw;
        }
    }
}
