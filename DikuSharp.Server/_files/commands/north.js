function north(ch, args) {
    if (!EXITS["north"]) {
        ch.SendLine("There is no exit in that direction.");
        return 0;
    }

    var exit = ch.CurrentRoom.Exits["north"];
    ch.SendLine("You walk north.");
    for (var i = 0 ; i < ch.CurrentRoom.Players.Count ; i++) {
        var player = ch.CurrentRoom.Players[i];
        if (player.Name === ch.Name) { continue;}
        player.SendLine(ch.Name + 'walks north.');
    }
    ch.CurrentRoom = exit.DestinationRoom;
    ch.SendLine(exit.DestinationRoom.Name);
    ch.SendLine(exit.DestinationRoom.Description);

    return 0;
}
