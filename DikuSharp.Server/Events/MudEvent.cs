using DikuSharp.Server.Characters;
using DikuSharp.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Events
{
    public class MudEvent
    {
        /// <summary>
        /// Takes a MudEvent and returns True if the event dequeued itself, False otherwise.
        /// </summary>
        public EventDelegate Func { get; set; }
        public string Argument { get; set; }
        public int Passes { get; set; }
        public EventType EventType { get; set; }
        public EventOwnerType OwnerType { get; set; }
        public object Owner { get; set; }
        public int Bucket { get; set; }

        public Area Area => Owner as Area;
        public Character Character => Owner as Character;
        public MudObject Object => Owner as MudObject;
        public Room Room => Owner as Room;
        public Connection Connection => Owner as Connection;
    }
}
