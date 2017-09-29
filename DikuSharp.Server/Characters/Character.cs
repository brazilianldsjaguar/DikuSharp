using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DikuSharp.Server.Models;
using Newtonsoft.Json;
using Jint.Runtime.Interop;
using Jint.Native;

namespace DikuSharp.Server.Characters
{
    public class Character
    {
        #region Serializable

        [JsonProperty( "name" )]
        public string Name { get; set; }
        [JsonProperty( "description" )]
        public string Description { get; set; }
        [JsonProperty( "shortDescription" )]
        public string ShortDescription { get; set; }
        [JsonProperty( "level" )]
        public int Level { get; set; }
        [JsonProperty("hitPoints")]
        public int HitPoints { get; set; }
        [JsonProperty( "maxHitPoints" )]
        public int MaxHitPoints { get; set; }
        [JsonProperty( "manaPoints" )]
        public int ManaPoints { get; set; }
        [JsonProperty( "maxManaPoints" )]
        public int MaxManaPoints { get; set; }
        [JsonProperty( "movePoints" )]
        public int MovePoints { get; set; }
        [JsonProperty( "maxMovePoints" )]
        public int MaxMovePoints { get; set; }
        [JsonProperty("currentRoomVnum")]
        public long CurrentRoomVnum
        {
            get { return _currentRoomVnum; }
            set { _currentRoomVnum = value; _currentRoom = Mud.I.AllRooms.First( r => r.Vnum == value ); }
        }
        private long _currentRoomVnum;
        #endregion

        [JsonIgnore]
        public Room CurrentRoom { get { return _currentRoom; } set { _currentRoom = value; _currentRoomVnum = value.Vnum; } }
        private Room _currentRoom;


        #region Aliases
        //These are shortcut properties to access the real ones
        [JsonIgnore]
        public int Hp { get { return HitPoints; } set { HitPoints = value; } }
        [JsonIgnore]
        public int Mp { get { return ManaPoints; } set { ManaPoints = value; } }
        [JsonIgnore]
        public int Mv { get { return MovePoints;} set { MovePoints = value; } }

        #endregion


    }
}
