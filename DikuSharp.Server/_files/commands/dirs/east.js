function east(ch, args) {
    var exits = ch.CurrentRoom.Exits;
    
    if (!exits["east"]) {
        ch.SendLine("There is no exit in that direction.");
        return 0;
    }

    var exit = exits["east"];
    ch.SendLine("You walk east.");
    for (var i = 0 ; i < ch.CurrentRoom.Players.Count ; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue;}
        player.SendLine(ch.Name + ' walks east.');
    }
    ch.CurrentRoom.RemoveCharacter(ch);
    ch.CurrentRoomVnum = exit.DestinationVnum;
    ch.CurrentRoom.AddCharacter(ch);
    for (var i = 0; i < ch.CurrentRoom.Players.Count; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue; }
        player.SendLine(ch.Name + ' arrives from the west.');
    }

    DO_COMMAND(ch, "look");

    return 0;
}
