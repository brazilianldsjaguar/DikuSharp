using DikuSharp.Server.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Models
{
    public class Reset
    {
        [JsonProperty("repopulate")]
        public bool Repopulate { get; set; }
        [JsonProperty("resetType")]
        public ResetType ResetType { get; set; }
        public int Arg1 { get; set; }
        public int Arg2 { get; set; }
        public int Arg3 { get; set; }
    }
}
