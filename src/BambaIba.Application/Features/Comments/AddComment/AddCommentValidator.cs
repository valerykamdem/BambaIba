using FluentValidation;

namespace BambaIba.Application.Features.Comments.AddComment;
public class AddCommentValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(2000).WithMessage("Comment cannot exceed 1000 characters");

        //RuleFor(x => x.UserId)
        //    .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.MediaId)
            .NotEmpty().WithMessage("Video ID is required");
    }
}
