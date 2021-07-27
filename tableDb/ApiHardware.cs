using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace IntelligenceFarmer.tableDb
{
    public partial class Welcome
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("feeds")]
        public Feed[] Feeds { get; set; }
    }
    public partial class Channel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("field1")]
        public string Field1 { get; set; }

        [JsonProperty("field2")]
        public string Field2 { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("last_entry_id")]
        public long LastEntryId { get; set; }
    }
    public partial class Feed
    {
        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("entry_id")]
        public long EntryId { get; set; }

        [JsonProperty("field1")]
        public string Field1 { get; set; }

        [JsonProperty("field2")]
        public string Field2 { get; set; }
        [JsonProperty("field3")]
        public string Field3 { get; set; }
    }

}
