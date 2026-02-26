namespace BambaIba.Application.Abstractions.Dtos;

public sealed record UserContext(
    string IdentityId,
    Guid LocalUserId,
    string Username,
    string Role,
    string Email,
    string? AvatarUrl
);

//public class UserContext
//{
//    public string IdentityId { get; }
//    public Guid LocalUserId { get; }
//    public string Role { get; }

//    public UserContext(string identityId, Guid localUserId, string role)
//    {
//        IdentityId = identityId;
//        LocalUserId = localUserId;

//        Role = role;
//    }
//}
