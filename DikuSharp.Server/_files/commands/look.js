function look( ch, args ) {
    if ( args.length > 0 ) {
        ch.SendLine("Not implemented yet...");
    } else {
        ch.SendLine(ch.CurrentRoom.Name);
        ch.SendLine(ch.CurrentRoom.Description);
    }
    return 0;
}
