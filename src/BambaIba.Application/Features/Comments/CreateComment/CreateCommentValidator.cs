using FluentValidation;

namespace BambaIba.Application.Features.Comments.CreateComment;
public class CreateCommentValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content is required")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");

        //RuleFor(x => x.UserId)
        //    .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.VideoId)
            .NotEmpty().WithMessage("Video ID is required");
    }
}
