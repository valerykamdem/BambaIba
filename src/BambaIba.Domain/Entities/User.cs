using BambaIba.Domain.Shared;

namespace BambaIba.Domain.Entities;

public class User : Entity<Guid>, ISoftDeletable
{
    public string IdentityId { get; set; } = string.Empty;
    public string CivilStatus { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Pseudo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    //public ICollection<UserRole> Roles { get; set; } = [];
}
