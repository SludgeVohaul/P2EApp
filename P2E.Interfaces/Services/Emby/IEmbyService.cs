﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediaBrowser.Model.Entities;
using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.Interfaces.Services.Emby
{
    public interface IEmbyService : IService
    {
        Task<ILibraryIdentifier> GetLibraryIdentifierAsync(string libraryName);
        Task<IReadOnlyCollection<IFilenameIdentifier>> GetFilenameIdentifiersAsync(ILibraryIdentifier libraryIdentifier);

        Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync();
        Task<ICollectionIdentifier> CreateCollectionAsync(string collectionName);

        Task<bool> TryAddMovieToCollectionAsync(IFilenameIdentifier filenameIdentifier, ICollectionIdentifier collectionIdentifier);
        Task<bool> TryAddImageToMovie(IFilenameIdentifier filenameIdentifier, ImageType imageType, string imageUrl);
        Task<bool> TryDeleteImagesFromMovie(IFilenameIdentifier filenameIdentifier, ImageType imageType);


        //Task<IMovieUpdateResult> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, IFilenameIdentifier filenameIdentifier);
        //Task<bool> UpdateItemAsync(IPlexMovieMetadata plexMovieMetadata, string embyLibraryName);

    }
}
