using P2E.Interfaces.DataObjects.Emby.Library;

namespace P2E.DataObjects.Emby.Library
{
    public class CollectionIdentifier : ItemIdentifier, ICollectionIdentifier
    {
        /// <summary>
        /// The "filename" in Path withot " [boxset]".
        /// </summary>
        /// <remarks>If the PlexMovieMetaDataItem was in a collection called "What Ever Collection"
        /// then the Path would be "/some/where/data/collections/What Ever Collection [boxset]".
        /// PathBasename would be "What Ever Collection".
        /// </remarks>
        public string PathBasename { get; set; }
    }
}