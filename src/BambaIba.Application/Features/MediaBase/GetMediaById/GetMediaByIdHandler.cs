using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Abstractions.Mappings;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Videos;
using BambaIba.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.MediaBase.GetMediaById;

public sealed record GetMediaByIdQuery(Guid MediaId);


public sealed class GetMediaByIdHandler(IBIDbContext dbContext,
        IMediaStorageService mediaStorageService,
        ILogger<GetMediaByIdHandler> logger)
{

    public async Task<Result<MediaDetailsDto>> Handle(
       GetMediaByIdQuery query,
       CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving media with ID: {MediaId}", query.MediaId);

            // 1) Récupération du média
            MediaAsset media = await dbContext.MediaAssets
                .Include(m => ((Video)m).Qualities)
                .AsNoTracking()
                .Where(m => m.Id == query.MediaId && m.Status == Domain.Enums.MediaStatus.Ready)
                .FirstOrDefaultAsync(cancellationToken);

            if (media == null)
            {
                logger.LogWarning("media with ID: {MediaId} not found", query.MediaId);
                return Result.Failure<MediaDetailsDto>(
                    Error.NotFound("Not.Found", $"media with ID: {query.MediaId} not found"));
            }

            // 2) Mise à jour atomique du compteur de vues
            await dbContext.MediaStats
                .Where(s => s.MediaId == query.MediaId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.PlayCount, s => s.PlayCount + 1),
                    cancellationToken);

            // 3) Mise à jour du timestamp
            await dbContext.MediaAssets
                .Where(m => m.Id == query.MediaId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(m => m.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            // 4) Retour du résultat
            return Result.Success(MediaMapper.ToResult(media, mediaStorageService));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving Media with ID: {MediaId}", query.MediaId);
            throw;
        }
    }

}
