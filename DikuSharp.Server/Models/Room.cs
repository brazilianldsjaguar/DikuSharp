using System;
using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Characters;
using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Room
    {
        #region Serializable

        [JsonProperty( "vnum" )]
        public long Vnum { get; set; }
        [JsonProperty( "name" )]
        public string Name { get; set; }
        [JsonProperty( "description" )]
        public string Description { get; set; }

        public Dictionary<string,Exit> Exits { get; set; }
        #endregion

        [JsonIgnore]
        public List<PlayerCharacter> Players { get; set; }
        [JsonIgnore]
        public List<NonPlayerCharacter> Mobs { get; set; }

        [JsonIgnore]
        public List<Character> AllCharacters {
            get
            {
                var players = Players.Select( p => p as Character );
                var mobs = Mobs.Select( m => m as Character );
                return players.Concat( mobs ).ToList( );
            }
        }

    }
}
