using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.Domain.Entities.Videos;
using BambaIba.SharedKernel;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.AddComment;

public sealed record AddCommentCommand(
    Guid MediaId,
    string Content,
    Guid? ParentCommentId = null);

public sealed class AddCommentHandler(IBIDbContext dbContext,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        IValidator<AddCommentCommand> validator,
        IBIMongoContext mongoContext,
        IMediaStatisticsService statsService,
        ILogger<AddCommentHandler> logger)
{

    public async Task<Result<string>> Handle(
        AddCommentCommand command,       
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validation
            ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
                return Result.Failure<string>(Error.Problem("400", string.Join(", ", validationResult.Errors)));

            UserContext userContext = await userContextService
                .GetCurrentContext(httpContextAccessor.HttpContext);

            MediaAsset media = await dbContext.MediaAssets
                .FindAsync([command.MediaId, cancellationToken], 
                cancellationToken: cancellationToken);

            if (media == null)
                return Result.Failure<string>(VideoErrors.NotFound(command.MediaId));

            //// Validation : Parent comment existe ? (si réponse)
            //if (command.ParentCommentId.HasValue)
            //{
            //    bool parentExists = await dbContext.Comments
            //        .AnyAsync(c => c.Id == command.ParentCommentId.Value,
            //        cancellationToken: cancellationToken);

            //    if (!parentExists)
            //        return Result.Failure<Guid>(CommentErrors.NotFoundParent);
            //}

            // Créer le commentaire
            var comment = new Comment
            {
                //Id = Guid.CreateVersion7(),
                MediaId = command.MediaId.ToString(),
                UserId = userContext.LocalUserId.ToString(),
                Content = command.Content,
                ParentId = command.ParentCommentId?.ToString(),
            };

            //await dbContext.Comments.AddAsync(comment, cancellationToken);
            await mongoContext.Comments.InsertOneAsync(comment, cancellationToken: cancellationToken);

            // Incrémenter le compteur de commentaires de la vidéo
            await statsService.IncrementCommentCountAsync(command.MediaId, cancellationToken);

            logger.LogInformation(
                "Comment created: {CommentId} by {UserId} on video {MediaId}",
                comment.Id, userContext.LocalUserId, command.MediaId);

            return Result.Success(comment.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating comment");
            return Result.Failure<string>(CommentErrors.ErrorCreating(ex.Message));
        }
    }
}
