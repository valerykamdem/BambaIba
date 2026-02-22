using FluentValidation;

namespace BambaIba.Application.Features.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        // ✅ Email valide
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");

        // ✅ Mot de passe ≥ 6 caractères
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password too short");

        // ✅ Prénom obligatoire
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First name required");

        // ✅ Nom de famille obligatoire
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name required");

        //// ✅ Genre valide – ex: "male", "female", "other"
        //RuleFor(x => x.CivilStatus).Must(g =>
        //        g == "male" ||
        //        g == "female")
        //    .WithMessage("Genre invalide – doit être 'male', 'female'");
    }
}
