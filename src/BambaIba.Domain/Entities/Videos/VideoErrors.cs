using BambaIba.SharedKernel;

namespace BambaIba.Domain.Entities.Videos;

public static class VideoErrors
{
    public static Error NotFound(Guid videoId) => Error.NotFound(
        "Videos.NotFound",
        $"The video with the Id = '{videoId}' was not found");

    public static readonly Error NotFoundById = Error.NotFound(
        "Videos.NotFoundById",
        "The video with the specified id was not found");

    //public static readonly Error EmailNotUnique = Error.Conflict(
    //    "Users.EmailNotUnique",
    //    "The provided email is not unique");
}
