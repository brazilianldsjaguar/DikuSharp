using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;
using DikuSharp.Server.Commands;
using DikuSharp.Server.Extensions;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using DikuSharp.Server.Models;
using Newtonsoft.Json;
using DikuSharp.Server.Enums;

namespace DikuSharp.Server
{
    //Main entry point for the server
    public static class OutputParser
    {
        public static void Parse( Connection connection )
        {
            if (string.IsNullOrEmpty(connection.OutputBuffer)) { return; }
            if ( connection.ConnectionStatus == ConnectionStatus.Playing )
            {
                //here we'll add the prompt
                connection.SendLine(Prompt.ParsePrompt(connection.CurrentCharacter), sendNewLine: false);
            }
        }
    }
}
