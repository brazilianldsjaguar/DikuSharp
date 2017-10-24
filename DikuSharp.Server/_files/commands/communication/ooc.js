function ooc(ch, args) {
    if (args.length < 1) 
    {
        ch.SendLine("Ooc what?");
    }
    else
    {
        for (var i = 0; i < MUD.AllPlayers.length; i++) {
            var player = MUD.AllPlayers[i];
            if (player.Name === ch.Name) { player.SendLine("{WYou ooc, '{C"+ args.join(' ') +"{W'{x"); }
            else { player.SendLine("{W"+ ch.Name + " oocs '{C" + args.join(' ') + "{W'{x"); }

        }
        
    }

    return 0;
}
