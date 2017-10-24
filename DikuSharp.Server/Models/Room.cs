using System;
using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Characters;
using Newtonsoft.Json;
using DikuSharp.Server.Events;
using DikuSharp.Server.Enums;

namespace DikuSharp.Server.Models
{
    public class Room : IEventContainer
    {
        #region Serializable

        [JsonProperty( "vnum" )]
        public long Vnum { get; set; }
        [JsonProperty( "name" )]
        public string Name { get; set; }
        [JsonProperty( "description" )]
        public string Description { get; set; }
        [JsonProperty("resets")]
        public List<Reset> Resets { get; set; }


        public Dictionary<string,Exit> Exits { get; set; }
        #endregion

        public Room()
        {
            Players = new List<PlayerCharacter>();
            Mobs = new List<NonPlayerCharacter>();
            Events = new List<MudEvent>();
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

        [JsonIgnore]
        public IList<MudEvent> Events { get; }

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

        /// <summary>
        /// Main code to reset a room's contents
        /// </summary>
        public void Reset()
        {
            foreach( var reset in Resets )
            {
                if ( reset.ResetType == ResetType.Mobile )
                {
                    NonPlayerCharacter mob = new NonPlayerCharacter() { Name = "test mob", ShortDescription = "Test mob is here.", Description = "This test mob looks great." };
                    this.AddCharacter(mob);
                }
            }
        }
    }
}
