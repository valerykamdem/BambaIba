using FluentValidation;

namespace BambaIba.Application.Features.Videos.Upload;
internal sealed class UploadVideoValidator : AbstractValidator<UploadVideoRequest>
{
    public UploadVideoValidator()
    {
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.File).NotNull().WithMessage("Le fichier est obligatoire.");
        RuleFor(c => c.Description).NotEmpty();
    }
}
