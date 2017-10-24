using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Events
{
    /// <summary>
    /// A huge enum to describe what all the different events might be
    /// </summary>
    public enum EventType
    {
        //None
        None = 0,

        //Game Events. 1-99
        GameWeather = 1,

        //Player Events. 100-499
        PlayerHeal = 100,

        //Mob Events. 500-699
        MobileHeal = 500,

        //Obj Events. 700-899
        ObjectDecay = 700,

        //Room Events. 900-1099
        RoomAct = 900,

        //Area events. 1100-1199
        AreaReset = 1100,

        //Other events, as necessary 1200+

    }
}
