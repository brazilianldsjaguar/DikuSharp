function who(ch, args) {

    ch.SendLine(".------ DIKU SHARP ------.");
    ch.SendLine("| Player                 |");
    ch.SendLine("+------------------------+");
    
    for (var i = 0; i < MUD.AllPlayers.length; i++) {
        var player = MUD.AllPlayers[i];
        ch.SendFormatLine("| {0,-22} |", player.Name);
    }

    ch.SendLine("'------------------------'");

    return 0;
}
