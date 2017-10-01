using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Emby.ApiClient;
using Emby.ApiClient.Model;
using MediaBrowser.Model.Collections;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories.Emby;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
using P2E.DataObjects.Emby.Library;
using P2E.Interfaces.DataObjects.Emby.Library;
using P2E.Interfaces.Logging;

namespace P2E.Repositories.Emby
{
    public class EmbyRepository : IEmbyRepository
    {
        private readonly IAppLogger _logger;

        public EmbyRepository(IAppLogger logger)
        {
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<ILibraryIdentifier>> GetLibraryIdentifiersAsync(IEmbyClient client)
        {
            var itemsResult = await client.GetUserViews(client.CurrentUserId);

            return itemsResult.Items
                .Select(x => new LibraryIdentifier
                {
                    Name = x.Name,
                    Id = x.Id
                })
                .ToArray();
        }

        public async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdentifiersAsync(IEmbyClient client, string libraryId)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,
                ParentId = libraryId,
                Filters = new[] { ItemFilter.IsNotFolder },
                IncludeItemTypes = new[] { "Movie" },
                Fields = new[] { ItemFields.Path },
                Recursive = true
            };
            var itemsResult = await client.GetItemsAsync(query);

            return itemsResult.Items
                .Select(x => new MovieIdentifier
                {
                    Filename = Path.GetFileName(x.Path),
                    Id = x.Id
                })
                .ToArray();
        }

        public async Task<IReadOnlyCollection<ICollectionIdentifier>> GetCollectionIdentifiersAsync(IEmbyClient client)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,
                // FYI: Collections are folders.
                Filters = new[] { ItemFilter.IsFolder },
                IncludeItemTypes = new[] { "Boxset" },
                Fields = new[] { ItemFields.Path },
                Recursive = true
            };
            var itemsResult = await client.GetItemsAsync(query);

            return itemsResult.Items
                .Select(x => new CollectionIdentifier
                {
                    PathBasename = Path.GetFileName(x.Path)?.Replace(" [boxset]", ""),
                    Id = x.Id
                })
                .ToArray();
        }

        /// <remarks>Please read
        /// https://emby.media/community/index.php?/topic/50514-apiclient-how-to-check-whether-an-arbitrary-string-matches-an-existing-boxset/
        /// </remarks>
        public async Task<ICollectionIdentifier> CreateCollectionAsync(IEmbyClient client, string pathBasename)
        {
            var args = new QueryStringDictionary
            {
                {"IsLocked", "false"},
                {"Name", pathBasename},
                {"ParentId", ""},
                {"Ids", ""}
            };
            var url = client.GetApiUrl("Collections");
            var collectionCreationResult = await client.SendAsync<CollectionCreationResult>(url, "POST", args);

            return new CollectionIdentifier
            {
                PathBasename = pathBasename,
                Id = collectionCreationResult.Id,
            };
        }

        /// <remarks>See
        /// https://github.com/MediaBrowser/Emby/blob/master/MediaBrowser.Api/Movies/CollectionService.cs
        /// for the POST URL.</remarks>
        public async Task AddMovieToCollectionAsync(IEmbyClient client, string movieId, string collectionId)
        {
            var args = new QueryStringDictionary
            {
                {"Ids", movieId}
            };
            var url = client.GetApiUrl($"Collections/{collectionId}/Items");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);

        }

        public async Task AddImageToMovieAsync(IEmbyClient client, ImageType imageType, Uri imageUrl, string movieId)
        {
            var args = new QueryStringDictionary
            {
                {"Type", imageType.ToString()},
                {"ImageUrl", imageUrl.AbsoluteUri}
            };

            var url = client.GetApiUrl($"Items/{movieId}/RemoteImages/Download");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);
        }


        /// <remarks>
        /// There seems to be some issue somewhere, which prevents a image from being reindexed.
        /// See
        /// https://emby.media/community/index.php?/topic/50794-apiclient-server-3230-cannot-reindex-the-backdrop-image-of-a-movie/
        /// </remarks>
        public async Task ReindexImageOfMovieAsync(IEmbyClient client, ImageType imageType, int index, int newIndex, string movieId)
        {
            var args = new QueryStringDictionary
            {
                { "newIndex", newIndex.ToString()}
            };

            var url = client.GetApiUrl($"Items/{movieId}/Images/{imageType}/{index}/Index");
            await client.SendAsync<EmptyRequestResult>(url, "POST", args);
        }

        public async Task<IReadOnlyCollection<ImageInfo>> GetImageInfosAsync(IEmbyClient client, string movieId)
        {
            var url = client.GetApiUrl($"Items/{movieId}/Images");
            return await client.SendAsync<List<ImageInfo>>(url, "GET");
        }

        public async Task DeleteImageFromMovieAsync(IEmbyClient client, ImageType imageType, int index, string movieId)
        {
            var url = client.GetApiUrl($"Items/{movieId}/Images/{imageType}/{index}");
            await client.SendAsync<EmptyRequestResult>(url, "DELETE");
        }

        public async Task UpdateMetadataAsync(IEmbyClient client, IEmbyMovieMetadata movieMetadata, string movieId)
        {
            var baseItemDto = await client.GetItemAsync(movieId, client.CurrentUserId);

            baseItemDto.Name = movieMetadata.Name;
            baseItemDto.OriginalTitle = movieMetadata.OriginalTitle;
            baseItemDto.ForcedSortName = movieMetadata.ForcedSortName;

            await client.UpdateItemAsync(movieId, baseItemDto);
        }







        public async Task<IReadOnlyCollection<IMovieIdentifier>> GetMovieIdsAsync(IEmbyClient client, string libraryName)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,
                // das ist die id der Movies CZ lib
                //Ids = new [] { "33a3ebcb3615fbc88bdfacf96075317e" },
                ParentId = "33a3ebcb3615fbc88bdfacf96075317e",

                // Get media only, don't return folder items
                Filters = new[] { ItemFilter.IsNotFolder },

                IncludeItemTypes = new[] { "Movie" },

                //Limit = 11,
                Fields = new[] { ItemFields.Path },
                //Fields = new[] { ItemFields.MediaSources },



                // Search recursively through the user's library
                Recursive = true
            };
            //var itemsResult = await client.GetItemsAsync(query);
            var itemsResult = await client.GetItemsAsync(query);
            // das holt die library namen samt id.
            var libs = await client.GetUserViews(client.CurrentUserId);

            var paretiddto = await client.GetItemAsync("83ef7ab9c13d82603663b7fd841a169b", client.CurrentUserId);
            var viewiddto = await client.GetItemAsync("33a3ebcb3615fbc88bdfacf96075317e", client.CurrentUserId);

            return itemsResult.Items
                .Select(x => new MovieIdentifier
                {
                    Filename = Path.GetFileName(x.Path),
                    Id = x.Id,
                })
                .ToArray();
            //var baseItemDto = await client.GetItemAsync("209fe3f9525635b01de69121be68a4f3", client.CurrentUserId);

        }

        public async Task GetStuffAsync(IEmbyClient client)
        {
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,

                SortBy = new[] { ItemSortBy.DateCreated },
                SortOrder = SortOrder.Descending,

                // Get media only, don't return folder items
                Filters = new[] { ItemFilter.IsNotFolder },
                //Filters = new[] { ItemFilter.IsFolder },

                IncludeItemTypes = new[] { "Movie" },

                //Limit = 11,
                //Fields = new [] {ItemFields.OriginalTitle, ItemFields.Path, ItemFields.SortName, ItemFields.MediaSources},
                //Fields = new[] { ItemFields.MediaSources },


                // Search recursively through the user's library
                Recursive = true
            };

            //var itemsResult = await client.GetItemsAsync(query);

            var baseItemDto = await client.GetItemAsync("209fe3f9525635b01de69121be68a4f3", client.CurrentUserId);
            //var baseItemDto = client.GetItemAsync("209fe3f9525635b01de69121be68a4f3", client.CurrentUserId).Result;

            //var newBaseItemDto = await UpdateItemDtoAsync(baseItemDto);

            var baseItemDtoDict = new Dictionary<string, string>();
            //baseItemDto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //   .ToList()
            //   .ForEach(x =>
            //   {
            //       var propertyValue = x.GetValue(baseItemDto);
            //       //baseItemDtoDict.Add(x.Name, propertyValue == null ? "" : JsonConvert.SerializeObject(propertyValue).Replace("\\", "").Replace("\"", ""));
            //       baseItemDtoDict.Add(x.Name, JsonConvert.SerializeObject(propertyValue));
            //   });

            //////var stringBuilder = new StringBuilder();
            //////using (var stringWriter = new StringWriter(stringBuilder))
            //////using (var writer = new JsonTextWriter(stringWriter))
            //////{
            //////    writer.QuoteChar = '\'';
            //////    var jsonSerializer = new JsonSerializer();

            //////    baseItemDto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //////        .ToList()
            //////        .ForEach(x =>
            //////        {
            //////            var propertyValue = x.GetValue(baseItemDto);
            //////            var jsonProperty = propertyValue ?? "";
            //////            jsonSerializer.Serialize(writer, jsonProperty);

            //////            baseItemDtoDict.Add(x.Name, stringBuilder.ToString().Replace("'", ""));
            //////            stringBuilder.Clear();
            //////        });
            //////}

            var baseItemDtoJson = JsonConvert.SerializeObject(baseItemDto, Formatting.None);
            var baseItemDtoDict2 = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(baseItemDtoJson);

            var aa = baseItemDtoDict2.Keys
                .Select(x => Convert.ToString(baseItemDtoDict2[x]))
                .ToArray();

            //baseItemDtoDict2.Keys
            //    .ToList()
            //    .ForEach(x => baseItemDtoDict.Add(x, Convert.ToString(baseItemDtoDict2[x])));

            //var jsSerializer = new JavaScriptSerializer();
            //var baseItemDtoJson = jsSerializer.Serialize(baseItemDto);
            //var dict = (Dictionary<string, object>)jsSerializer.DeserializeObject(baseItemDtoJson);



            //Dictionary<string, string> FD = (from x in data.GetType().GetProperties() select x).ToDictionary(x => x.Name, x => (x.GetGetMethod().Invoke(data, null) == null ? "" : x.GetGetMethod().Invoke(data, null).ToString()));
            var fd = baseItemDto.GetType().GetProperties()
                .ToDictionary(x => x.Name, x => x.GetGetMethod()
                    .Invoke(baseItemDto, null) == null
                        ? ""
                        : x.GetGetMethod().Invoke(baseItemDto, null).ToString());


            //baseItemDto.ForcedSortName = "ugauga";
            //var baseItemDtoDict = GetItemDict(baseItemDto);
            //foreach (var key in baseItemDtoDict.Keys)
            //{
            //    if (baseItemDtoDict[key] == null) baseItemDtoDict[key] = "";
            //}

            //var filteredItems = itemsResult.Items
            //    .SelectMany(x => x.MediaSources)
            //    .Select(x => new MovieIdentifier
            //    {
            //        Id = x.Id,
            //        Filename = Path.GetFileName(x.Path)
            //    })
            //    .ToArray();


            //await Task.Delay(15000);
            //var result = await client.PostAsync<Task<BaseItemDto>>(client.GetApiUrl("Items/209fe3f9525635b01de69121be68a4f3"), baseItemDtoDict);



            //var filteredItems = itemsResult.Items
            //    .Where(x => x.MediaSources
            //        .Any(
            //            y =>
            //                Equals(y.Path,
            //                    "/mnt/movies/Sylvester Stallone/Demolition Man (1993)/Demolition Man (1993).mkv")))
            //    .Select(x => x.Id)
            //    .ToArray();

        }

        private class DictionaryConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Dictionary<string, string>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var items = (Dictionary<string, string>)value;
                writer.WriteStartObject();
                foreach (var item in items)
                {

                    writer.WritePropertyName(item.Key);
                    writer.WriteRawValue(item.Value);

                }
                writer.WriteEndObject();
                writer.Flush();

            }
        }


        public void GetStuff(IEmbyClient client)
        {
            // TODO cleanup
            // Get the ten most recently added items for the current user.
            var query = new ItemQuery
            {
                UserId = client.CurrentUserId,

                SortBy = new[] { ItemSortBy.DateCreated },
                SortOrder = SortOrder.Descending,

                // Get media only, don't return folder items
                //Filters = new[] {ItemFilter.IsNotFolder},
                Filters = new[] { ItemFilter.IsFolder },

                IncludeItemTypes = new[] { "Boxset" },

                Limit = 11,

                // Search recursively through the user's library
                Recursive = true
            };

            try
            {
                var getItemsTask = client.GetItemsAsync(query);

                getItemsTask.Wait();
                var items = getItemsTask.Result;

                var args = new Dictionary<string, string>
                {
                    {"IsLocked", "false"},
                    {"Name", "Addams Collection"},
                    {"ParentId", ""},
                    {"Ids", ""}
                };
                var colTask = client.PostAsync<CollectionCreationResult>(client.GetApiUrl("Collections"), args);
                colTask.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .ToList()
                    .ForEach(e => _logger.ErrorException(e.Message, e));
            }
        }

        //private string DictDing(Dictionary<string, string> args)
        //{
        //    return  string.Join("&", args.Keys.Select<string, string>((Func<string, string>)(key => string.Format("{0}={1}", (object)key, (object)args[key]))).ToArray<string>());
        //}

        //private void InitializeMovieItem(MovieItemDto movieItemDto, BaseItemDto baseItemDto)
        //{
        //    movieItemDto.Id = baseItemDto.Id;
        //    movieItemDto.Name = baseItemDto.Name;
        //    movieItemDto.OriginalTitle = baseItemDto.OriginalTitle;
        //    movieItemDto.ForcedSortName = baseItemDto.ForcedSortName;
        //    movieItemDto.DisplayMediaType = baseItemDto.DisplayMediaType;
        //    movieItemDto.CommunityRating = baseItemDto.CommunityRating;
        //    movieItemDto.HomePageUrl = baseItemDto.HomePageUrl;
        //    movieItemDto.CriticRating = baseItemDto.CriticRating;
        //    movieItemDto.IndexNumber = baseItemDto.IndexNumber;
        //    movieItemDto.AbsoluteEpisodeNumber = baseItemDto.AbsoluteEpisodeNumber;
        //    movieItemDto.DvdEpisodeNumber = baseItemDto.DvdEpisodeNumber;
        //    movieItemDto.DvdSeasonNumber = baseItemDto.DvdSeasonNumber;
        //    movieItemDto.AirsBeforeSeasonNumber = baseItemDto.AirsBeforeSeasonNumber;
        //    movieItemDto.AirsAfterSeasonNumber = baseItemDto.AirsAfterSeasonNumber;
        //    movieItemDto.AirsBeforeEpisodeNumber = baseItemDto.AirsBeforeEpisodeNumber;
        //    movieItemDto.ParentIndexNumber = baseItemDto.ParentIndexNumber;
        //    movieItemDto.DisplayOrder = baseItemDto.DisplayOrder;
        //    movieItemDto.Players = baseItemDto.Players;
        //    movieItemDto.Album = baseItemDto.Album;
        //    movieItemDto.AlbumArtists = baseItemDto.AlbumArtists ?? new List<NameIdPair>();
        //    movieItemDto.ArtistItems = baseItemDto.ArtistItems ?? new List<NameIdPair>();
        //    movieItemDto.Overview = baseItemDto.Overview;
        //    movieItemDto.Status = baseItemDto.Status;
        //    movieItemDto.AirDays = baseItemDto.AirDays ?? new List<DayOfWeek>();
        //    movieItemDto.AirTime = baseItemDto.AirTime;
        //    movieItemDto.Genres = baseItemDto.Genres ?? new List<string>();
        //    movieItemDto.Tags = baseItemDto.Tags ?? new List<string>();
        //    movieItemDto.Keywords = baseItemDto.Keywords ?? new List<string>();
        //    var bla = new List<StudioDto>();
        //    bla.AddRange(baseItemDto.Studios);
        //    bla.Add(new StudioDto
        //    {
        //        Name = "FuckFlick Ltd.",
        //        Id = "123456",
        //        PrimaryImageTag = null
        //    });
        //    movieItemDto.Studios = bla.ToArray();
        //    //movieItemDto.Studios = baseItemDto.Studios;
        //    movieItemDto.PremiereDate = baseItemDto.PremiereDate;
        //    movieItemDto.DateCreated = baseItemDto.DateCreated;
        //    movieItemDto.EndDate = baseItemDto.EndDate;
        //    movieItemDto.ProductionYear = baseItemDto.ProductionYear;
        //    movieItemDto.AspectRatio = baseItemDto.AspectRatio;
        //    movieItemDto.Video3DFormat = baseItemDto.Video3DFormat;
        //    movieItemDto.OfficialRating = baseItemDto.OfficialRating;
        //    movieItemDto.CustomRating = baseItemDto.CustomRating;
        //    movieItemDto.People = baseItemDto.People;
        //    movieItemDto.LockData = baseItemDto.LockData;
        //    movieItemDto.LockedFields = baseItemDto.LockedFields ?? new List<MetadataFields>();
        //    movieItemDto.ProviderIds = baseItemDto.ProviderIds;
        //    movieItemDto.PreferredMetadataLanguage = baseItemDto.PreferredMetadataLanguage ?? "";
        //    movieItemDto.PreferredMetadataCountryCode = baseItemDto.PreferredMetadataCountryCode ?? "";
        //    movieItemDto.Taglines = baseItemDto.Taglines ?? new List<string>();
        //}

        //private class MovieItemDto
        //{
        //    public string Id { get; set; }
        //    public string Name { get; set; }

        //    public string OriginalTitle { get; set; }
        //    public string ForcedSortName { get; set; }
        //    public string DisplayMediaType { get; set; }
        //    public float? CommunityRating { get; set; }
        //    public string HomePageUrl { get; set; }
        //    public float? CriticRating { get; set; }
        //    public int? IndexNumber { get; set; }
        //    public int? AbsoluteEpisodeNumber { get; set; }
        //    public float? DvdEpisodeNumber { get; set; }
        //    public int? DvdSeasonNumber { get; set; }
        //    public int? AirsBeforeSeasonNumber { get; set; }

        //    public int? AirsAfterSeasonNumber { get; set; }

        //    public int? AirsBeforeEpisodeNumber { get; set; }
        //    public int? ParentIndexNumber { get; set; }
        //    public string DisplayOrder { get; set; }
        //    public int? Players { get; set; }
        //    public string Album { get; set; }
        //    public List<NameIdPair> AlbumArtists { get; set; }
        //    public List<NameIdPair> ArtistItems { get; set; }
        //    public string Overview { get; set; }
        //    public string Status { get; set; }
        //    public List<DayOfWeek> AirDays { get; set; }
        //    public string AirTime { get; set; }
        //    public List<string> Genres { get; set; }
        //    public List<string> Tags { get; set; }
        //    public List<string> Keywords { get; set; }
        //    public StudioDto[] Studios { get; set; }
        //    public DateTime? PremiereDate { get; set; }
        //    public DateTime? DateCreated { get; set; }
        //    public DateTime? EndDate { get; set; }
        //    public int? ProductionYear { get; set; }
        //    public string AspectRatio { get; set; }
        //    public MediaBrowser.Model.Entities.Video3DFormat? Video3DFormat { get; set; }
        //    public string OfficialRating { get; set; }
        //    public string CustomRating { get; set; }
        //    public BaseItemPerson[] People { get; set; }
        //    public bool? LockData { get; set; }
        //    public List<MetadataFields> LockedFields { get; set; }

        //    public Dictionary<string, string> ProviderIds { get; set; }
        //    public string PreferredMetadataLanguage { get; set; }
        //    public string PreferredMetadataCountryCode { get; set; }
        //    public List<string> Taglines { get; set; }
        //}
    }
}
