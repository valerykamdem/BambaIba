using BambaIba.Application.Abstractions.Dtos;
using BambaIba.Application.Abstractions.Interfaces;
using BambaIba.Domain.Entities.Audios;
using BambaIba.Domain.Entities.MediaAssets;
using BambaIba.Domain.Entities.Videos;
using BambaIba.Domain.Enums;
using BambaIba.SharedKernel.Videos;

namespace BambaIba.Application.Abstractions.Mappings;

public static class MediaMapper
{
    public static MediaDto ToDto(Audio audio)
    {
        return new MediaDto
        {
            Id = audio.Id,
            Type = "audio",
            Title = audio.Title,
            Description = audio.Description,
            //Url = audio.StoragePath,
            ThumbnailUrl = audio.ThumbnailPath,
            Duration = audio.Duration,
            //LikeCount = audio.LikeCount,
            //DislikeCount = audio.DislikeCount,
            //PlayCount = audio.PlayCount,
            //CommentCount = audio.CommentCount,
            IsPublic = audio.IsPublic,
            PublishedAt = audio.PublishedAt,
            Tags = audio.Tags
        };
    }

    public static MediaDto ToDto(Video video)
    {
        return new MediaDto
        {
            Id = video.Id,
            Type = "video",
            Title = video.Title,
            Description = video.Description,
            //Url = video.StoragePath,
            ThumbnailUrl = video.ThumbnailPath,
            Duration = video.Duration,
            //LikeCount = video.LikeCount,
            //DislikeCount = video.DislikeCount,
            //PlayCount = video.PlayCount,
            //CommentCount = video.CommentCount,
            IsPublic = video.IsPublic,
            PublishedAt = video.PublishedAt,
            Tags = video.Tags
        };
    }

    public static MediaDetailsDto ToResult(MediaAsset media, IMediaStorageService storageService)
    {
        return media switch
        {
            Video video => new MediaDetailsDto
            {
                Id = video.Id,
                Type = "video",
                Title = video.Title,
                Description = video.Description,
                MediaUrl = storageService.GetPublicUrl(BucketType.Video, video.StoragePath),
                ThumbnailUrl = storageService.GetPublicUrl(BucketType.Image, video.ThumbnailPath),
                Duration = video.Duration,
                //PlayCount = video.PlayCount,
                //LikeCount = video.LikeCount,
                //DislikeCount = video.DislikeCount,
                //CommentCount = video.CommentCount,
                CreatedAt = video.CreatedAt,
                PublishedAt = video.PublishedAt,
                UserId = video.UserId,
                IsPublic = video.IsPublic,
                Tags = video.Tags,
                Qualities = video.Qualities.Select(q => new VideoQualityDto
                {
                    Quality = q.Quality,
                    VideoUrl = storageService.GetPublicUrl(BucketType.Video, q.StoragePath)
                }).ToList()
            },

            Audio audio => new MediaDetailsDto
            {
                Id = audio.Id,
                Type = "audio",
                Title = audio.Title,
                Description = audio.Description,
                MediaUrl = storageService.GetPublicUrl(BucketType.Audio, audio.StoragePath),
                ThumbnailUrl = storageService.GetPublicUrl(BucketType.Image, audio.ThumbnailPath),
                Duration = audio.Duration,
                CreatedAt = audio.CreatedAt,
                PublishedAt = audio.PublishedAt,
                UserId = audio.UserId,
                IsPublic = audio.IsPublic,
                Tags = audio.Tags,
                Speaker = audio.Speaker,
                Category = audio.Category,
                Topic = audio.Topic,
                //Qualities = new List<VideoQualityDto>() // vide pour audio
            },

            _ => throw new InvalidOperationException("Unsupported media type")
        };
    }
}

