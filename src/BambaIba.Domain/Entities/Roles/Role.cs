using BambaIba.Domain.Entities.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.Roles;

public sealed class Role : Entity<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public bool IsSystem { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
