function look(ch, args) {
    if ( args.length > 0 ) {
        ch.SendLine("Not implemented yet...");
    } else {
        var roomName = ch.CurrentRoom.Name;
        if (roomName[0] != '{') {
            roomName = '{C' + roomName;
        }
        ch.SendLine(roomName);
        ch.SendLine("  " + ch.CurrentRoom.Description);
        ch.SendLine("");
        ch.SendLine("{W[{DExits{W]");

        var exitEnumerator = ch.CurrentRoom.Exits.GetEnumerator();
        var exitString = '';
        while (exitEnumerator.MoveNext())
        {
            exitString += ' ' + exitEnumerator.Current.Key;
        }
        ch.SendLine(exitString);        
        for (var i = 0; i < ch.CurrentRoom.Players.Count; i++)
        {
            var player = ch.CurrentRoom.Players[i];
            if (player.Name != ch.Name) {
                ch.SendLine(player.ShortDescription);
            }
            
        }
    }
    return 0;
}
