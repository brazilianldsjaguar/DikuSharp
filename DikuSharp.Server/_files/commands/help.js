function help( ch, args ) {
    if ( args.length > 0 ) {
        var arg = args[0].toLowerCase();
        for( var i = 0; i < HELPS.length ; i++ ) {
            var help = HELPS[i];
            if ( help.keywords.toLowerCase().indexOf(arg) != -1 ) {
                ch.SendLine(help.Contents);
                return 0;
            }
        }
        ch.SendLine("NOT A VALID HELP FILE...");
    } else {
        ch.SendLine("NOT A VALID HELP FILE...");
    }
    return 0;
}
