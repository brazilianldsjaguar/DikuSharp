function look( ch, args ) {
    if ( args.length > 0 ) {
        ch.SendLine("Not implemented yet...");
    } else {
        ch.SendLine(ch.CurrentRoom.Name);
        ch.SendLine(ch.CurrentRoom.Description);
        ch.SendLine("[Exits]");
        var exitString = '';
        for (var i = 0 ; i < EXITS.length ; i++) {
            var exit = EXITS[i];
            exitString += exit.name + ' ';
        }
        ch.SendLine(exitString);
    }
    return 0;
}
