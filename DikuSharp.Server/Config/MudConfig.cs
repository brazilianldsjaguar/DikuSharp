﻿using System;
using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Commands;
using Newtonsoft.Json;

namespace DikuSharp.Server.Config
{
    //Main entry point for the server
    public class MudConfig
    {
        [JsonProperty("mudName")]
        public string MudName { get; set; }
        [JsonProperty("portNumber")]
        public int PortNumber { get; set; }
        [JsonProperty("version")]
        public float Version { get; set; }
        [JsonProperty("maxLevel")]
        public int MaxLevel { get; set; }
        [JsonProperty("levels")]
        public Dictionary<string, string> Levels { get; set; }
        [JsonProperty( "roomVnumForNewPlayers" )]
        public int RoomVnumForNewPlayers { get; set; }
        [JsonProperty("areaFiles")]
        public List<string> AreaFiles { get; set; }
        [JsonProperty("raceFiles")]
        public List<string> RaceFiles { get; set; }

        [JsonProperty("classFiles")]
        public List<string> ClassFiles { get; set; }
        [JsonProperty( "accountFileRootDirectory" )]
        public string AccountFileRootDirectory { get; set; }

        [JsonProperty( "helpFileDirectory" )]
        public string HelpFileDirectory { get; set; }
        [JsonProperty( "commandDirectory")]
        public string CommandDirectory { get; set; }

        public List<CommandMetaData> Commands { get; set; }
    }
}
