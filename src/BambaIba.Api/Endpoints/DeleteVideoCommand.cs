
namespace BambaIba.Api.Endpoints;

internal class DeleteVideoCommand
{
    public Guid VideoId { get; set; }
    public object UserId { get; set; }
}