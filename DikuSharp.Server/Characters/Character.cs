using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DikuSharp.Server.Characters
{
    public class Character
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
