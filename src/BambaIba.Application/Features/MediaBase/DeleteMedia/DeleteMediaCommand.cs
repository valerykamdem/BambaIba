using BambaIba.SharedKernel;
using Cortex.Mediator.Commands;

namespace BambaIba.Application.Features.MediaBase.DeleteMedia;

public sealed record DeleteMediaCommand(Guid MediaId) : 
    ICommand<Result<DeleteMediaResult>>;
