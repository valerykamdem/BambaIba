using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.MediaBase.UploadMedia;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.UpdateMediaMetadata;

public sealed record UpdateMediaMetadataCommand(
    Guid MediaId,
    string? Title,
    string? Description,
    string? Speaker,
    string? Category,
    string? Topic,
    List<string>? Tags);


public sealed class UpdateMediaMetadataHandler
{
    public static async Task<Result> Handle(
       UpdateMediaMetadataCommand command,
       IBIDbContext dbContext,
       ILogger<UploadMediaHandler> logger,
       CancellationToken cancellationToken)
    {

        MediaAsset media = await dbContext.MediaAssets.FirstOrDefaultAsync(m => m.Id == command.MediaId, cancellationToken);
        if (media == null)
            return Result.Failure(Error.NotFound("401", "Not Found"));

        // Mise à jour uniquement des champs fournis
        if (!string.IsNullOrWhiteSpace(command.Title))
            media.Title = command.Title;

        if (!string.IsNullOrWhiteSpace(command.Description))
            media.Description = command.Description;

        if (!string.IsNullOrWhiteSpace(command.Speaker))
            media.Speaker = command.Speaker;

        if (!string.IsNullOrWhiteSpace(command.Category))
            media.Category = command.Category;

        if (!string.IsNullOrWhiteSpace(command.Topic))
            media.Topic = command.Topic;

        if (command.Tags != null)
            media.Tags = command.Tags;

        media.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Media {MediaId} metadata updated", command.MediaId);

        return Result.Success();
    }
}
