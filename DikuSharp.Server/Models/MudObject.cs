using DikuSharp.Server.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Models
{
    public class MudObject : IEventContainer
    {
        public MudObject()
        {
            Events = new List<MudEvent>();
        }
        #region Serializable
        [JsonProperty("vnum")]
        public long Vnum { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        #endregion

        [JsonIgnore]
        public IList<MudEvent> Events { get; }
    }
}
