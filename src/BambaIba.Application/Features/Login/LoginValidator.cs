using BambaIba.Application.Features.Login;
using FluentValidation;

namespace Concertation.Banking.API.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        // ✅ Email valide
        RuleFor(x => x.Email).EmailAddress().WithMessage("Email invalide");

        // ✅ Mot de passe ≥ 6 caractères
        RuleFor(x => x.Password).MinimumLength(6).WithMessage("Mot de passe trop court");

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
