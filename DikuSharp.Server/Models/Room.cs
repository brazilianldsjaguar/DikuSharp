using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Room
    {
        public long Vnum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
