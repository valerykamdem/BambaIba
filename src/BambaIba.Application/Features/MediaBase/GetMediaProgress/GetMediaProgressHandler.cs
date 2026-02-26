
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Mongo.MediaProgresses;
using BambaIba.SharedKernel;
using MongoDB.Driver;

namespace BambaIba.Application.Features.MediaBase.GetMediaProgress;

public sealed record GetMediaProgressQuery(Guid MediaId);

public sealed record MediaProgressDto(
    int LastPosition,
    bool IsCompleted
);

public sealed class GetMediaProgressHandler(
    IBIMongoContext mongoContext,
    IUserContextService userContextService)
{

    public async Task<Result<MediaProgressDto>> Handle(GetMediaProgressQuery query, CancellationToken cancellationToken)
    {
        // 1. Récupérer l'utilisateur connecté
        UserContext user = await userContextService.GetCurrentContext();
        if (user == null)
            return Result.Failure<MediaProgressDto>(Error.Forbidden("Not.Allow", "Unauthorized"));

        // 2. Construire l'ID composite (Clé primaire unique définie dans le modèle précédent)
        string compositeId = $"{user.LocalUserId}_{query.MediaId}";

        // 3. Lecture optimisée dans MongoDB (Lookup par _id est instantané)
        MediaProgress progress = await mongoContext.MediaProgresses
            .Find(p => p.Id == compositeId)
            .FirstOrDefaultAsync(cancellationToken);

        // 4. Retourner le résultat (ou une position par défaut à 0)
        if (progress == null)
        {
            return Result.Success(new MediaProgressDto(0, false));
        }

        return Result.Success(new MediaProgressDto(
            progress.PositionSeconds,
            progress.IsCompleted
        ));
    }
}
