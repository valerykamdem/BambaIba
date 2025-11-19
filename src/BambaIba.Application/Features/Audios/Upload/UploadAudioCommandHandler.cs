using BambaIba.Application.Abstractions.Data;
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Audios;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BambaIba.Application.Features.Audios.Upload;
public class UploadAudioCommandHandler : ICommandHandler<UploadAudioCommand, Result<UploadAudioResult>>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserContextService _userContextService;
    private readonly ILogger<UploadAudioCommandHandler> _logger;

    public UploadAudioCommandHandler(
        IServiceScopeFactory serviceScopeFactory,
        IHttpContextAccessor httpContextAccessor,
        IUserContextService userContextService,
        ILogger<UploadAudioCommandHandler> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _httpContextAccessor = httpContextAccessor;
        _userContextService = userContextService;
        _logger = logger;
    }

    public async Task<Result<UploadAudioResult>> Handle(
        UploadAudioCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            UserContext userContext = await _userContextService.GetCurrentContext(_httpContextAccessor.HttpContext);

            _logger.LogInformation("Starting audio upload for user {UserId}", command.UserId);

            // Validation
            string? validationError = Validatecommand(command);
            if (validationError != null)
                return UploadAudioResult.Failure(validationError);

            // Créer une scope pour l'opération principale
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IAudioRepository audioRepository = scope.ServiceProvider.GetRequiredService<IAudioRepository>();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();

            // Créer l'entité Audio
            var audio = new Audio
            {
                Title = command.Title,
                Description = command.Description,
                Speaker = command.Speaker,
                Category = command.Category,
                Topic = command.Topic,
                UserId = userContext.LocalUserId,
                FileName = command.AudioFileName,
                FileSize = command.AudioFileSize,
                Status = MediaStatus.Uploading,
                Tags = command.Tags ?? []
            };

            await audioRepository.AddAsync(audio);
            //await _context.SaveChangesAsync(cancellationToken);

            // Upload audio vers MinIO
            string audioPath = await storageService.UploadAudioAsync(
                audio.Id,
                command.AudioStream,
                command.AudioFileName,
                command.AudioContentType,
                cancellationToken);

            audio.StoragePath = audioPath;

            // Upload thumbnail si fourni
            if (command.CoverImageStream != null && !string.IsNullOrEmpty(command.CoverImageFileName))
            {
                string coverPath = await storageService.UploadImageAsync(
                    audio.Id,
                    command.CoverImageStream,
                    command.CoverImageFileName,
                    MediaType.AudioCover,
                    cancellationToken);

                audio.ThumbnailPath = coverPath;
            }

            audio.Status = MediaStatus.Processing;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Audio uploaded: {AudioId}", audio.Id);

            // Traitement asynchrone (durée uniquement)
            _ = Task.Run(async () => await ProcessAudioAsync(audio.Id, cancellationToken), cancellationToken);

            return UploadAudioResult.Success(audio.Id, audio.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading audio");
            return UploadAudioResult.Failure("An error occurred during upload");
        }
    }

    private string? Validatecommand(UploadAudioCommand command)
    {
        //// Taille max : 200MB
        //if (command.AudioFileSize > 209_715_200)
        //    return "Audio file exceeds maximum size of 200MB";

        // Formats autorisés
        string[] allowedFormats = new[] { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/aac", "audio/flac" };
        if (!allowedFormats.Contains(command.AudioContentType.ToLower()))
            return "Invalid audio format. Allowed: MP3, WAV, OGG, AAC, FLAC";

        if (string.IsNullOrWhiteSpace(command.Title))
            return "Title is required";

        return null;
    }

    private async Task ProcessAudioAsync(Guid audioId, CancellationToken cancellationToken)
    {
        // Créer une nouvelle scope pour le traitement en arrière-plan
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        IAudioRepository audioRepository = scope.ServiceProvider.GetRequiredService<IAudioRepository>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IMediaStorageService storageService = scope.ServiceProvider.GetRequiredService<IMediaStorageService>();
        IMediaProcessingService processingService = scope.ServiceProvider.GetRequiredService<IMediaProcessingService>();
        ILogger<UploadAudioCommandHandler> logger = scope.ServiceProvider.GetRequiredService<ILogger<UploadAudioCommandHandler>>();

        Audio? audio = null;
        try
        {      
            audio = await audioRepository.GetByIdAsync(audioId, cancellationToken);

            if (audio == null)
                return;

            _logger.LogInformation("Processing audio {AudioId}", audioId);

            // Télécharger pour extraire la durée
            string localPath = await storageService.DownloadAudioAsync(audio.StoragePath, cancellationToken);

            try
            {
                // Extraire la durée avec FFmpeg
                TimeSpan duration = await processingService.GetDurationAsync(localPath);

                audio.Duration = duration;
                audio.Status = MediaStatus.Ready;
                audio.PublishedAt = DateTime.UtcNow;
                audio.UpdatedAt = DateTime.UtcNow;

                await audioRepository.UpdateAsync(audio);

                _logger.LogInformation("Audio processing completed: {AudioId}", audioId);
            }
            finally
            {
                if (File.Exists(localPath))
                    File.Delete(localPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audio {AudioId}", audioId);

            if (audio != null)
            {
                audio.Status = MediaStatus.Failed;
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
