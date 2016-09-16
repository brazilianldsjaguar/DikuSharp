using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DikuSharp.Server.Commands
{
    //Main entry point for the server
    public interface IMudCommand
    {
        void Do( );
    }
}
