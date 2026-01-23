// GetLiveStreamsQueryHandler.cs
using BambaIba.Domain.Entities.LiveStream;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel;
using Cortex.Mediator.Queries;
using Microsoft.EntityFrameworkCore;


namespace BambaIba.Application.Features.LiveStreams.GetLiveStreams;

public class GetLiveStreamsQueryHandler : IQueryHandler<GetLiveStreamsQuery, Result<GetLiveStreamsResult>>
{

    private readonly ILiveStreamRepository _liveStreamRepository;

    public GetLiveStreamsQueryHandler(ILiveStreamRepository liveStreamRepository)
    {
        _liveStreamRepository = liveStreamRepository;
    }

    public async Task<Result<GetLiveStreamsResult>> Handle(
        GetLiveStreamsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<LiveStream?> query = _liveStreamRepository.GetLiveStream();

        if (request.OnlyLive)
        {
            query = query.Where(s => s!.Status == LiveStreamStatus.Live);
        }

        int totalCount = await query.CountAsync(cancellationToken); // Pass cancellationToken

        List<LiveStreamDto> streams = await query
            .OrderByDescending(s => s!.StartedAt ?? s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new LiveStreamDto
            {
                Id = s!.Id,
                StreamerId = s.StreamerId,
                StreamerName = s.StreamerId, // TODO: Récupérer depuis Keycloak
                Title = s.Title,
                ThumbnailUrl = s.ThumbnailPath,
                Status = s.Status,
                ViewerCount = s.ViewerCount,
                StartedAt = s.StartedAt
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new GetLiveStreamsResult
        {
            Streams = streams,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        });
    }
}
