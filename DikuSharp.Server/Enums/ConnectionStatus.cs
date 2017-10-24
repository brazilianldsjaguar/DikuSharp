using System;

namespace DikuSharp.Server
{
    public enum ConnectionStatus
    {
        Connected,
        PutInPassword,
        ChoosingAccount,
        CreatingCharacter,
        Playing,
        ChooseCharacter
    }
}
