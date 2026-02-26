
//namespace BambaIba.Api.Extensions;

//public static class MediatorExtensions
//{
//    public static RouteHandlerBuilder MapCommand<TCommand, TResult>(
//        this IEndpointRouteBuilder app,
//        string pattern)
//        where TCommand : class, ICommand<TResult>
//    {
//        return app.MapPost(pattern, async (IMediator mediator, TCommand command) =>
//        {
//            TResult? result = await mediator.SendCommandAsync<TCommand, TResult>(command);
//            return Results.Ok(result);
//        });
//    }

//    public static RouteHandlerBuilder MapQuery<TQuery, TResult>(
//        this IEndpointRouteBuilder app,
//        string pattern)
//        where TQuery : class, IQuery<TResult>, new()
//    {
//        return app.MapGet(pattern, async (IMediator mediator) =>
//        {
//            TResult? result = await mediator.SendQueryAsync<TQuery, TResult>(new TQuery());
//            return Results.Ok(result);
//        });
//    }
//}

