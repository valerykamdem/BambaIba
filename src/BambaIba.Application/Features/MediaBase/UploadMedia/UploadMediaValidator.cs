using FluentValidation;

namespace BambaIba.Application.Features.MediaBase.UploadMedia;

internal sealed class UploadMediaValidator : AbstractValidator<UploadMediaRequest>
{

    private static readonly string[] AllowedAudioFormats = { "audio/mpeg", "audio/mp3", "audio/wav", "audio/ogg", "audio/aac", "audio/flac" };
    private static readonly string[] AllowedVideoFormats = { "video/mp4", "video/mpeg", "video/quicktime", "video/webm" };

    public UploadMediaValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");

        RuleFor(x => x.MediaFile.ContentType.ToLower())
            .Must(type => type.Equals("audio", StringComparison.OrdinalIgnoreCase) ||
                          type.Equals("video", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Media type must be 'audio' or 'video'");

        // Règles spécifiques aux AUDIOS
        When(x => x.MediaFile.ContentType.ToLower().Equals("audio", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.MediaFile.Length)
                .LessThanOrEqualTo(209_715_200) // 200MB
                .WithMessage("Audio file exceeds maximum size of 200MB");

            RuleFor(x => x.MediaFile.ContentType.ToLower())
                .Must(ct => !string.IsNullOrWhiteSpace(ct) && AllowedAudioFormats.Contains(ct.ToLower()))
                .WithMessage("Invalid audio format. Allowed: MP3, WAV, OGG, AAC, FLAC");
        });

        // Règles spécifiques aux VIDÉOS
        When(x => x.MediaFile.ContentType.ToLower().Equals("video", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.MediaFile.Length)
                .LessThanOrEqualTo(5_368_709_120) // 5GB
                .WithMessage("Video file exceeds maximum size of 5GB");

            RuleFor(x => x.MediaFile.ContentType.ToLower())
                .Must(ct => !string.IsNullOrWhiteSpace(ct) && AllowedVideoFormats.Contains(ct.ToLower()))
                .WithMessage("Invalid video format. Allowed: MP4, MPEG, MOV, WEBM");
        });
    }
}
