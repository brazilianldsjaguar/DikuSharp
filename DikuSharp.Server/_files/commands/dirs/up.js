function up(ch, args) {
    var exits = ch.CurrentRoom.Exits;
    
    if (!exits["up"]) {
        ch.SendLine("There is no exit in that direction.");
        return 0;
    }

    var exit = exits["up"];
    ch.SendLine("You walk up.");
    for (var i = 0 ; i < ch.CurrentRoom.Players.Count ; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue;}
        player.SendLine(ch.Name + ' walks up.');
    }
    ch.CurrentRoom.RemoveCharacter(ch);
    ch.CurrentRoomVnum = exit.DestinationVnum;
    ch.CurrentRoom.AddCharacter(ch);
    for (var i = 0; i < ch.CurrentRoom.Players.Count; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue; }
        player.SendLine(ch.Name + ' arrives from below.');
    }

    DO_COMMAND(ch, "look");

    return 0;
}
