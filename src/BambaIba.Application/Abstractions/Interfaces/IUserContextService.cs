using BambaIba.Application.Abstractions.Dtos;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IUserContextService
{
    //Task<UserContext> GetCurrentContext(HttpContext context);
    Task<UserContext> GetCurrentContext();
}
