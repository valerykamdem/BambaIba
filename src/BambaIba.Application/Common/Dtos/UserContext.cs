namespace BambaIba.Application.Common.Dtos;

public class UserContext
{
    public string IdentityId { get; }
    public Guid LocalUserId { get; }

    public UserContext(string identityId, Guid localUserId)
    {
        IdentityId = identityId;
        LocalUserId = localUserId;
    }
}
