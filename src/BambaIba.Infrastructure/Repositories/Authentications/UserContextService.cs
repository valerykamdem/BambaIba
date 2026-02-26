using System.Security.Claims;
using BambaIba.Application.Abstractions.Caching; // Ton interface
using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Users;
using BambaIba.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BambaIba.Infrastructure.Repositories.Authentications;

public class UserContextService(
    BIDbContext db,
    IHttpContextAccessor httpContextAccessor,
    ICacheService cacheService, // Injection de ton service de cache
    ILogger<UserContextService> logger) : IUserContextService
{
    // Cache duration: Sliding expiration works best for active users
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

    public async Task<UserContext> GetCurrentContext()
    {
        ClaimsPrincipal? claimUser = httpContextAccessor.HttpContext?.User;

        if (claimUser == null || claimUser.Identity == null || !claimUser.Identity.IsAuthenticated)
        {
            return null;
        }

        // --- 1. EXTRACTION FROM JWT (Fast, No DB) ---

        // Extract Keycloak Identity ID
        Claim? userIdClaim = claimUser.FindFirst(ClaimTypes.NameIdentifier) ?? claimUser.FindFirst("sub");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid identityIdGuid))
        {
            logger.LogWarning("User ID claim is missing or invalid.");
            return null;
        }
        string identityId = identityIdGuid.ToString();

        // Extract standard profile info from Token (no DB needed)
        string username = claimUser.FindFirst(ClaimTypes.Name)?.Value
                        ?? claimUser.FindFirst("preferred_username")?.Value
                        ?? "Unknown";

        string email = claimUser.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        string? avatar = claimUser.FindFirst("picture")?.Value ?? claimUser.FindFirst("avatar")?.Value;

        // --- 2. GET LOCAL DATA (LocalUserId & Role) FROM CACHE OR DB ---

        // Define a unique cache key for this mapping
        string cacheKey = $"user_mapping_{identityId}";

        // Try to get from Cache
        LocalUserMapping? mapping = await cacheService.GetAsync<LocalUserMapping>(cacheKey);

        if (mapping == null)
        {
            // Cache Miss: Query the Database
            logger.LogDebug("Cache miss for user {IdentityId}, querying DB.", identityId);

            User? user = await db.Users
                .AsNoTracking() // Read-only optimization
                .FirstOrDefaultAsync(u => u.IdentityId == identityId);

            if (user == null)
            {
                logger.LogWarning("User with IdentityId {IdentityId} not found in local DB.", identityId);
                return null; // Or handle sync logic if needed
            }

            string role = await db.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.Role.Name)
                .FirstOrDefaultAsync() ?? "Viewer"; // Default role if null

            mapping = new LocalUserMapping(user.Id, role);

            // Save to Cache for future requests
            await cacheService.SetAsync(cacheKey, mapping, _cacheDuration);
        }
        else
        {
            logger.LogDebug("Cache hit for user {IdentityId}.", identityId);
        }

        // --- 3. CONSTRUCT FINAL CONTEXT ---

        return new UserContext(
            identityId,
            mapping.LocalUserId,
            username,
            mapping.Role,
            email,
            avatar
        );
    }

    // Internal DTO for caching only the DB-specific data
    private record LocalUserMapping(Guid LocalUserId, string Role);
}
