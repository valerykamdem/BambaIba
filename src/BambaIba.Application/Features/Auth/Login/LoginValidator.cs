using FluentValidation;

namespace BambaIba.Application.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        // ✅ Email valide
        RuleFor(x => x.Email).EmailAddress().WithMessage("Invalid email address");

        // ✅ Mot de passe ≥ 6 caractères
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password too short");

        //// ✅ Prénom obligatoire
        //RuleFor(x => x.FirstName).NotEmpty().WithMessage("Prénom requis");

        //// ✅ Nom de famille obligatoire
        //RuleFor(x => x.LastName).NotEmpty().WithMessage("Nom requis");

        //// ✅ Genre valide – ex: "male", "female", "other"
        //RuleFor(x => x.CivilStatus).Must(g =>
        //        g == "male" ||
        //        g == "female" ||
        //        g == "other")
        //    .WithMessage("Genre invalide – doit être 'male', 'female' ou 'other'");
    }
}
