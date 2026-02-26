using BambaIba.Application.Abstractions.DomainEvents;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Services;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Mongo.Comments;
using BambaIba.SharedKernel;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BambaIba.Application.Features.Comments.AddComment;

public sealed record AddCommentCommand(
    Guid MediaId,
    string Content,
    Guid? ParentCommentId = null);

public sealed class AddCommentHandler(
    IBIDbContext dbContext,
    IUserContextService userContextService,
    IValidator<AddCommentCommand> validator,
    IBIMongoContext mongoContext,
    IMediaStatisticsService statsService,
    IMessageBus bus,
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
                return Result.Failure<string>(Error.Validation("400", string.Join(", ", validationResult.Errors)));

            UserContext userContext = await userContextService
                .GetCurrentContext();

            if (userContext == null)
                return Result.Failure<string>(Error.Unauthorized("401", "User not authenticated"));

            // 2. Récupération du média (Nécessaire pour savoir QUI notifier)
            MediaAsset media = await dbContext.MediaAssets
                .FindAsync([command.MediaId], cancellationToken: cancellationToken);

            if (media == null)
                return Result.Failure<string>(CommentErrors.NotFound(command.MediaId));

            //// Validation : Parent comment existe ? (si réponse)
            //if (command.ParentCommentId.HasValue)
            //{
            //    bool parentExists = await dbContext.Comments
            //        .AnyAsync(c => c.Id == command.ParentCommentId.Value,
            //        cancellationToken: cancellationToken);

            //    if (!parentExists)
            //        return Result.Failure<Guid>(CommentErrors.NotFoundParent);
            //}


            // 3. Création du commentaire
            var comment = new Comment
            {
                MediaId = command.MediaId.ToString(),
                UserId = userContext.LocalUserId.ToString(),
                Content = command.Content,
                ParentId = command.ParentCommentId?.ToString(),
            };

            await mongoContext.Comments.InsertOneAsync(comment, cancellationToken: cancellationToken);

            // 4. Mise à jour des stats
            await statsService.IncrementCommentCountAsync(command.MediaId, cancellationToken);

            logger.LogInformation(
                "Comment created: {CommentId} by {UserId} on video {MediaId}",
                comment.Id, userContext.LocalUserId, command.MediaId);

            // 5. NOUVEAU : Déclenchement de la Notification
            // On vérifie qu'on ne se notifie pas soi-même
            if (media.UserId != userContext.LocalUserId)
            {
                var notificationEvent = new NotificationCreatedEvent(
                    RecipientUserId: media.UserId, // Le propriétaire de la vidéo
                    TriggeredByUserId: userContext.LocalUserId!,
                    TriggeredByUsername: userContext.Username ?? "a User", // Adapte selon ton UserContext
                    MessageType: "Comment",
                    MessageContent: "commented your media",
                    MediaId: media.Id,
                    MediaTitle: media.Title
                );

                // Publication asynchrone (Fire and Forget via Outbox)
                await bus.PublishAsync(notificationEvent);
            }

            return Result.Success(comment.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating comment");
            return Result.Failure<string>(CommentErrors.ErrorCreating(ex.Message));
        }
    }
}
