using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName);
        Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(ILibraryIdentifier libraryIdentifier);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync();
        Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName);

        Task<bool> TryAddMovieToCollectionAsync(IMovieIdentifier movieIdentifier, ICollectionIdentifier collectionIdentifier);
        Task<bool> TryAddImageToMovieAsync(IMovieIdentifier movieIdentifier, ImageType imageType, Uri imageUrl);
        Task<bool> TryDeleteImagesFromMovieAsync(IMovieIdentifier movieIdentifier, ImageType imageType);


        //Task<IMovieUpdateResult> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, IMovieIdentifier movieIdentifier);
        //Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName);

    }
}
