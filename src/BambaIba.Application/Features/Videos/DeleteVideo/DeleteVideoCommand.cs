using BambaIba.SharedKernel;
using BambaIba.SharedKernel.Videos;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.Videos.DeleteVideo;

public sealed record DeleteVideoCommand(Guid VideoId) : 
    ICommand<Result<DeleteVideoResult>>;
