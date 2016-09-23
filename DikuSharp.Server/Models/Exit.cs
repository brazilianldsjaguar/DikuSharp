using System;
using System.Collections.Generic;
using System.Linq;
using DikuSharp.Server.Characters;
using Newtonsoft.Json;

namespace DikuSharp.Server.Models
{
    public class Exit
    {
        #region Serializable
        
        [JsonProperty( "destinationVnum" )]
        public long DestinationVnum {
            get { return _destinationVnum; }
            set {
                _destinationVnum = value;
                if ( Mud.I.AllRooms != null )
                {
                    _destinationRoom = Mud.I.AllRooms.First( r => r.Vnum == value );
                }
            }
        }
        private long _destinationVnum;
        [JsonIgnore]
        public Room DestinationRoom {
            get { return _destinationRoom; }
            set { _destinationRoom = value; _destinationVnum = value.Vnum; }
        }
        private Room _destinationRoom;
        #endregion


    }
}
