using BambaIba.Domain.Users;
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities;

public sealed class UserRole : Entity<Guid>, ISoftDeletable
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
