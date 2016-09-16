using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public string RawJavascript { get; set; }
    }
}
