using BambaIba.Application.Common.Dtos;
using Microsoft.AspNetCore.Http;

namespace BambaIba.Infrastructure.Services.Authentications;

public interface IUserContextService
{
    Task<UserContext> GetCurrentContext(HttpContext context);
}
