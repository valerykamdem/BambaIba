using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Videos;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using BambaIba.Domain.Entities.MediaAssets;

namespace BambaIba.Application.Features.Search;

public record IndexMediaCommand(Guid MediaId);


public sealed class IndexMediaHandler(
    IBIDbContext dbContext,
    ElasticsearchClient elasticClient,
    ILogger<IndexMediaHandler> logger)
{
    public async Task Handle(IndexMediaCommand command, CancellationToken ct)
    {
        MediaAsset? media = await dbContext.MediaAssets
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == command.MediaId, ct);

        if (media is null || media.Status != Domain.Enums.MediaStatus.Ready)
        {
            logger.LogWarning("Media {Id} not found or not ready for indexing.", command.MediaId);
            return;
        }

        // Mapping vers le Document
        var document = new MediaDocument
        {
            Id = media.Id,
            Title = media.Title,
            Description = media.Description,
            Speaker = media.Speaker,
            Category = media.Category,
            Tags = media.Tags ?? [],
            MediaType = media is Video ? "video" : "audio",
            PublishedAt = media.PublishedAt ?? DateTime.UtcNow
        };

        IndexResponse response = await elasticClient.IndexAsync(document, idx => idx.Index("media_index"), ct);

        if (response.IsValidResponse)
        {
            logger.LogInformation("Media {Id} indexed successfully.", media.Id);
        }
        else
        {
            logger.LogError("Failed to index media {Id}: {Error}", media.Id, response.DebugInformation);
            // Wolverine va retry automatiquement ici grâce à la gestion des erreurs
            throw new Exception("Elasticsearch indexing failed");
        }
    }
}
