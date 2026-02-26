using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.MediaProgresses;
using BambaIba.SharedKernel;
using MongoDB.Driver;

namespace BambaIba.Application.Features.MediaBase.UpdateMediaProgresses;

public sealed record UpdateMediaProgressCommand(
    string MediaId,
    int PositionSeconds
);

public sealed class UpdateMediaProgressHandler(
    IBIMongoContext mongoContext,
    IUserContextService userContextService)
{
    public async Task<Result> Handle(UpdateMediaProgressCommand cmd, CancellationToken ct)
    {
        UserContext user = await userContextService.GetCurrentContext();
        if (user == null)
            return Result.Failure(Error.Forbidden("Not.Allow", "Unauthorized"));

        // Création de l'ID composé
        string compositeId = $"{user.LocalUserId}_{cmd.MediaId}";

        // UPSERT ATOMIQUE (Update ou Insert en une seule requête)
        FilterDefinition<MediaProgress> filter = Builders<MediaProgress>.Filter.Eq(x => x.Id, compositeId);

        UpdateDefinition<MediaProgress> update = Builders<MediaProgress>.Update
            .SetOnInsert(x => x.UserId, user.LocalUserId.ToString())
            .SetOnInsert(x => x.MediaId, cmd.MediaId)
            .Set(x => x.PositionSeconds, cmd.PositionSeconds)
            .Set(x => x.IsCompleted, false) // Reset completed si on bouge
            .Set(x => x.LastUpdated, DateTime.UtcNow);

        var options = new UpdateOptions { IsUpsert = true };

        // Cette ligne est ultra-rapide et thread-safe
        await mongoContext.MediaProgresses.UpdateOneAsync(filter, update, options, cancellationToken: ct);

        return Result.Success();
    }
}
