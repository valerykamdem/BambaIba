using FluentValidation;
using FluentValidation.Results;

namespace BambaIba.Api.Extensions;

public static class ValidationExtensions
{
    public static RouteHandlerBuilder WithValidation<TRequest>(
         this RouteHandlerBuilder builder)
         where TRequest : class
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            // Récupère l’argument du handler qui correspond à TRequest
            if (context.Arguments.FirstOrDefault(a => a is TRequest) is not TRequest request)
            {
                return Results.BadRequest("Invalid request payload.");
            }

            // Récupère le validateur depuis DI
            IValidator<TRequest> validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
            if (validator is not null)
            {
                ValidationResult validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }

            // Si tout est OK → continuer le pipeline
            return await next(context);
        });

        return builder;
    }
}
