using BambaIba.Application.Abstractions.Dtos;
using Microsoft.AspNetCore.Http;

namespace BambaIba.Application.Abstractions.Interfaces;

public interface IUserContextService
{
    Task<UserContext> GetCurrentContext(HttpContext context);
}
