using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Area
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("minLevel")]
        public int MinLevel { get; set; }
        [JsonProperty("maxLevel")]
        public int MaxLevel { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("rooms")]
        public List<Room> Rooms { get; set; }
    }
}
