using BambaIba.Domain.Entities;

namespace BambaIba.Application.Common.Interfaces;
public interface IVideoQualityRepository
{
    Task AddVideoQuality(VideoQuality videoQuality);
}
