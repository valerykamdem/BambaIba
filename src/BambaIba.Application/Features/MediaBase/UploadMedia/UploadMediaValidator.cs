using FluentValidation;

namespace BambaIba.Application.Features.MediaBase.UploadMedia;
internal sealed class UploadMediaValidator : AbstractValidator<UploadMediaRequest>
{
    public UploadMediaValidator()
    {
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.MediaFile).NotNull().WithMessage("Le fichier est obligatoire.");
        RuleFor(c => c.Description).NotEmpty();
    }
}
