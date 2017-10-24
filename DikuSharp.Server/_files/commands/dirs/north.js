function north(ch, args) {
    var exits = ch.CurrentRoom.Exits;
    
    if (!exits["north"]) {
        ch.SendLine("There is no exit in that direction.");
        return 0;
    }

    var exit = exits["north"];
    ch.SendLine("You walk north.");
    for (var i = 0 ; i < ch.CurrentRoom.Players.Count ; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue;}
        player.SendLine(ch.Name + ' walks north.');
    }
    ch.CurrentRoom.RemoveCharacter(ch);
    ch.CurrentRoomVnum = exit.DestinationVnum;
    ch.CurrentRoom.AddCharacter(ch);

    for (var i = 0; i < ch.CurrentRoom.Players.Count; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue; }
        player.SendLine(ch.Name + ' arrives from the south.');
    }

    DO_COMMAND(ch, "look");

    return 0;
}
