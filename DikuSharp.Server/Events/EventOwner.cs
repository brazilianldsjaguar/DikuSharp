using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Events
{
    public enum EventOwnerType
    {
        Unowned,
        Object,
        Character,
        Connection,
        Room,
        Area,
        Game
    }
}
