using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DikuSharp.Server.Enums;

namespace DikuSharp.Server.Commands
{
    public class CommandMetaData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("priority")]
        public int Priority { get; set; }
        [JsonProperty("level")]
        public int Level { get; set; }
        [JsonProperty("aliases")]
        public IEnumerable<string> Aliases { get; set; }
        [JsonProperty("commandType")]
        public CommandType CommandType { get; set; }
        [JsonProperty("preserveQuotes")]
        public bool PreserveQuotes { get; set; }

        [JsonIgnore]
        public string RawJavascript { get; set; }
    }
}
