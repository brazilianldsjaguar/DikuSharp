function restart( ch, args ) {
    if (args.length > 0) {
        ch.SendLine("Sorry can't do that.");
    } else {
        ch.SendLine("Re-loading configuration files...");
        ADMIN_RESTART();
        ch.SendLine("Done!");
    }
    return 0;
}
