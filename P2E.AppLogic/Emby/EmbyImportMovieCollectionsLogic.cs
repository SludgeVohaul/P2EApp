using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P2E.Interfaces.AppLogic.Emby;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;
using P2E.Interfaces.Services.Emby;
using OperatingSystem = MediaBrowser.Model.System.OperatingSystem;

namespace P2E.AppLogic.Emby
{
    public class EmbyImportMovieCollectionsLogic : IEmbyImportMovieCollectionsLogic
    {
        private readonly IEmbyCollectionService _collectionService;
        private readonly IAppLogger _logger;

        public EmbyImportMovieCollectionsLogic(IEmbyCollectionService collectionService, IAppLogger logger)
        {
            _collectionService = collectionService;
            _logger = logger;
        }

        public async Task<bool> RunAsync(IReadOnlyCollection<string> plexMovieCollections, IMovieIdentifier embyMovieIdentifier)
        {
            var serverOperatingSystem = await _collectionService.GetServerOperatingSystemAsync();
            if (serverOperatingSystem == null)
            {
                var msg = "Cannot determine the server's operating system. Movie will not be added to any collection.";
                _logger.Log(Severity.Warn, msg);
                return false;
            }
            var isWindowsServer = serverOperatingSystem.Value == OperatingSystem.Windows;

            // Get required collections already existing in Emby.
            var existingCollections = await GetExistingCollectionsAsync(plexMovieCollections, isWindowsServer);
            if (existingCollections == null)
            {
                var msg = "Failed to query existing collections. Movie will not be added to any collection.";
                _logger.Log(Severity.Warn, msg);
                return false;
            }

            // Create missing collections.
            var createdCollections = plexMovieCollections.Count == existingCollections.Count
                ? new ICollectionIdentifier[] {}
                : await CreateMissingCollectionsAsync(plexMovieCollections, existingCollections, isWindowsServer);
            if (createdCollections == null)
            {
                var msg = "Failed to create missing collections. Movie will not be added to any collection.";
                _logger.Log(Severity.Warn, msg);
                return false;
            }

            // Add the movie to all collections.
            var allCollections = existingCollections.Union(createdCollections).ToArray();
            if (await AddMovieToCollectionsAsync(allCollections, embyMovieIdentifier) == false)
            {
                _logger.Log(Severity.Warn, "Failed to add movie to one or more collections.");
                return false;
            }

            return true;
        }

        private async Task<IReadOnlyCollection<ICollectionIdentifier>> GetExistingCollectionsAsync(IReadOnlyCollection<string> plexCollections,
                                                                                                   bool isWindowsServer)
        {
            var collectionIdentifiers = await _collectionService.GetCollectionIdentifiersAsync();

            return collectionIdentifiers?
                .Where(x => plexCollections.Contains(x.Filename, new CollectionNameComparer(isWindowsServer)))
                .ToArray();
        }

        private async Task<IReadOnlyCollection<ICollectionIdentifier>> CreateMissingCollectionsAsync(IReadOnlyCollection<string> plexCollections,
                                                                                                     IReadOnlyCollection<ICollectionIdentifier> existingCollections,
                                                                                                     bool isWindowsServer)
        {
            var missingCollections = plexCollections
                .Except(existingCollections.Select(x => x.Filename), new CollectionNameComparer(isWindowsServer))
                .ToArray();

            var createdCollections = await Task.WhenAll(missingCollections.Select(_collectionService.CreateCollectionAsync));
            return createdCollections.Any(x => x == null)
                ? null
                : createdCollections.ToArray();
        }

        private async Task<bool> AddMovieToCollectionsAsync(IReadOnlyCollection<ICollectionIdentifier> allCollections,
                                                            IMovieIdentifier embyMovieIdentifier)
        {
            var addMovieToCollectionsTasks = allCollections
                .Select(x => _collectionService.TryAddMovieToCollectionAsync(embyMovieIdentifier, x));
            var addMovieToCollectionsResults = await Task.WhenAll(addMovieToCollectionsTasks);
            return addMovieToCollectionsResults.All(x => x);
        }

        /// <inheritdoc />
        /// <remarks>
        /// The comparison feature is the collection's filename taken from Path without the suffix " [boxset]",
        /// as the Path is computed during creation of the collection based on the provided collection name
        /// (and stays untouched even if the name (i.e. Title) is changed later by the user).
        /// As not all characters are available for filenames, a simple comparison might fail
        /// (the ':' has been replaced by ' '):
        /// Plex collection name    : "Mission: Impossible Filmreihe"
        /// Emby collection Title   : "Mission: Impossible Filmreihe"
        /// Emby collection Filename: "Mission  Impossible Filmreihe"
        /// To handle this, the Plex collection name is modified using the identical mechanism
        /// as used in the collection creation code in Emby Server (Method GetValidFilename() in
        /// Emby.Server.Implementations.IO.ManagedFileSystem.cs)
        /// </remarks>
        private class CollectionNameComparer : IEqualityComparer<string>
        {
            // Characters to be replaced by the Space-character.
            private readonly List<char> _replaceableChars;

            public CollectionNameComparer(bool hasWindowsFilenames)
            {
                // This is taken from Emby Server code (Method SetInvalidFileNameChars() in
                // Emby.Server.Implementations.IO.ManagedFileSystem.cs)
                _replaceableChars = hasWindowsFilenames
                    ? Path.GetInvalidFileNameChars().ToList()
                    : new List<char> { '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
                                   '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F',
                                   '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17',
                                   '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F',
                                   '\x22', '\x3C', '\x3E', '\x7C', ':', '*', '?', '\\', '/' };
            }

            public bool Equals(string x, string y)
            {
                return string.Equals(GetFilename(x), GetFilename(y), StringComparison.InvariantCulture);
            }

            public int GetHashCode(string obj)
            {
                return GetFilename(obj).GetHashCode();
            }

            private string GetFilename(string collectionName)
            {
                var sb = new StringBuilder(collectionName);
                _replaceableChars.ForEach(c => sb.Replace(c, ' '));
                return sb.ToString();
            }
        }
    }
}