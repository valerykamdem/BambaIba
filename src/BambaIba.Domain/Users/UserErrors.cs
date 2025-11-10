
using BambaIba.SharedKernel;

namespace BambaIba.Domain.Users;
public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "User.NotFound",
        $"The user with the Id = '{userId}' is not your");

    public static readonly Error NotFoundById = Error.NotFound(
        "Users.NotFoundById",
        "The user with the specified id was not found");

    //public static readonly Error EmailNotUnique = Error.Conflict(
    //    "Users.EmailNotUnique",
    //    "The provided email is not unique");
}
