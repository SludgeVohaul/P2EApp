﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Model.Collections;
using MediaBrowser.Model.Querying;
using P2E.ExtensionMethods;
using P2E.Interfaces.DataObjects.Emby;
using P2E.Interfaces.Repositories.Emby;
using MediaBrowser.Model.Entities;
using Newtonsoft.Json;
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


            await Task.Delay(15000);
            ////////var result = await client.PostAsync<Task<BaseItemDto>>(client.GetApiUrl("Items/209fe3f9525635b01de69121be68a4f3"), baseItemDtoDict);



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




        private class MovieIdentifier
        {
            public string Id { get; set; }
            public string Filename { get; set; }
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
