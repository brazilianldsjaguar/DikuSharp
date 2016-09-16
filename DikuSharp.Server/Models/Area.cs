using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Area
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int Id { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
