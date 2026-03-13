using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Application.Features.MediaBase.ProcessMedia;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BambaIba.Application.Features.MediaBase.RetryProcessing;

public sealed record RetryProcessingCommand(Guid MediaId);


public sealed class RetryProcessingHandler(IBIDbContext dbContext,
        IUserContextService userContextService,
        IMessageBus bus,
        ILogger<RetryProcessingHandler> logger)
{

    public async Task<Result> Handle(
       RetryProcessingCommand command,
       CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await userContextService.GetCurrentContext();

            logger.LogInformation("Retrieving media with ID: {MediaId}", command.MediaId);

            // 2. Récupération du média
            MediaAsset? media = await dbContext.MediaAssets.
                FindAsync([command.MediaId], cancellationToken: cancellationToken);

            if (media == null)
            {
                return Result.Failure(Error.NotFound("Not.Found", "Média non trouvé."));
            }


            // 3. Sécurité : Vérifier que l'utilisateur est le propriétaire
            if (media.UserId != userContext.LocalUserId)
            {
                return Result.Failure(Error.Forbidden("Forbidden", "Not your Media."));
            }

            // 4. Validation : On ne peut retry que si c'est échoué
            if (media.Status != MediaStatus.Failed && media.Status != MediaStatus.Pending)
            {
                return Result.Failure(Error.Failure("Not.Found", "Seuls les médias en échec peuvent être relancés."));
            }

            // 5. Mise à jour du statut immédiat (pour le frontend)
            media.Status = MediaStatus.Processing;
            media.UpdatedAt = DateTime.UtcNow; // Optionnel
            await dbContext.SaveChangesAsync(cancellationToken);

            // 6. Publication de la commande de re-traitement via Wolverine
            await bus.PublishAsync(new ProcessMediaCommand(command.MediaId, userContext.LocalUserId));

            return Result.Success(new { Message = "Le re-traitement a été lancé avec succès." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving Media with ID: {MediaId}", command.MediaId);
            //throw;
            return Result.Failure<Result>(Error.Problem("500", ex.Message + "Processing failed"));
        }
    }

}
