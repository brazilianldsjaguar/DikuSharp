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

        public Room()
        {
            Players = new List<PlayerCharacter>();
            Mobs = new List<NonPlayerCharacter>();
        }

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
        
        public void AddCharacter(Character character)
        {
            if ( character is PlayerCharacter ) { Players.Add(character as PlayerCharacter); }
            else if (character is NonPlayerCharacter ) { Mobs.Add(character as NonPlayerCharacter); }
        }

        public void RemoveCharacter(Character character)
        {
            if (character is PlayerCharacter) { Players.Remove(character as PlayerCharacter); }
            else if (character is NonPlayerCharacter) { Mobs.Remove(character as NonPlayerCharacter); }
        }

    }
}
