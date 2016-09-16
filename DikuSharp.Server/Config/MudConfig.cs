using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace DikuSharp.Server.Config
{
    //Main entry point for the server
    public class MudConfig
    {
        [JsonProperty("mudName")]
        public string MudName { get; set; }

        [JsonProperty("version")]
        public float Version { get; set; }

        [JsonProperty("areaFiles")]
        public List<string> AreaFiles { get; set; }

        [JsonProperty("classFiles")]
        public List<string> ClassFiles { get; set; }
        [JsonProperty( "accountFileRootDirectory" )]
        public string AccountFileRootDirectory { get; set; }

        [JsonProperty( "helpFileDirectory" )]
        public string HelpFileDirectory { get; set; }
    }
}
