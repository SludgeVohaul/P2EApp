using System;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyImageService : IService
    {
        Task<bool> TryAddImageToMovieAsync(ImageType imageType, Uri imageUrl, IMovieIdentifier movieIdentifier);
        Task<int?> GetMaxImageIndex(ImageType imageType, IMovieIdentifier movieIdentifier);
        Task<bool> ReindexImageOfMovieAsync(ImageType imageType, int currentIndex, int newIndex, IMovieIdentifier movieIdentifier);
        Task<bool> TryDeleteImageFromMovieAsync(ImageType imageType, int imageIndex, IMovieIdentifier movieIdentifier);
    }
}