using BambaIba.Domain.Entities;
using BambaIba.SharedKernel;


namespace BambaIba.Domain.Entities.Users;

public sealed class User : Entity<Guid>, ISoftDeletable
{
    public string IdentityId { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pseudo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
