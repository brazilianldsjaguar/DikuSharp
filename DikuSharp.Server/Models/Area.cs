using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using DikuSharp.Server.Events;

namespace DikuSharp.Server.Models
{
    public class Area : IEventContainer
    {
        public Area()
        {
            Events = new List<MudEvent>();
        }
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

        [JsonIgnore]
        public IList<MudEvent> Events { get; }

        /// <summary>
        /// Resets the entire area
        /// </summary>
        public void Reset()
        {
            foreach( var room in Rooms)
            {
                room.Reset();
            }
        }
    }
}
