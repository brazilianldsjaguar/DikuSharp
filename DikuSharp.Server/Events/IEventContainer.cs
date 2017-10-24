using System;
using System.Collections.Generic;
using System.Text;

namespace DikuSharp.Server.Events
{
    public interface IEventContainer
    {
        IList<MudEvent> Events { get; }
    }
}
