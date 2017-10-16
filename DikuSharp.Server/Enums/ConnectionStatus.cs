using System;

namespace DikuSharp.Server
{
    public enum ConnectionStatus
    {
        Connected,
        PutInUsername,
        PutInPassword,
        ChoosingAccount,
        CreatingCharacter,
        Playing,
        ChooseCharacter
    }
}
