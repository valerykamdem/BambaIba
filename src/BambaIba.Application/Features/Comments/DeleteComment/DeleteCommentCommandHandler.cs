using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Comments;
using BambaIba.Domain.Entities.Users;
using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Comments;
using Cortex.Mediator.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Comments.DeleteComment;
public sealed class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, Result<DeleteCommentResult>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserContextService _userContextService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DeleteCommentCommandHandler> _logger;

    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository,
        IUserContextService userContextService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteCommentCommandHandler> logger)
    {
        _commentRepository = commentRepository;
        _userContextService = userContextService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<Result<DeleteCommentResult>> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            Comment comment = await _commentRepository.GetComment(command.CommentId);

            if (comment == null)
                return Result.Failure<DeleteCommentResult>(CommentErrors.NotFound(command.CommentId));

            if (comment.UserId != userContext.LocalUserId)
                return Result.Failure<DeleteCommentResult>(UserErrors.NotFound(userContext.LocalUserId));

            // Supprimer la vidéo de la base de données seulement si l'utilisateur est le propriétaire
            _commentRepository.DeleteComment(comment);

            _logger.LogInformation("Deleting video files from storage for VideoId: {VideoId}", command.VideoId);

            return Result.Success(DeleteCommentResult.Success());

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting video with ID: {VideoId}", command.VideoId);
            throw;
        }
    }
}
