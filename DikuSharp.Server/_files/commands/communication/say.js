function say(ch, args) {
    if (args.length < 1) 
    {
        ch.SendLine("Say what?");
    }
    else
    {
        for (var i = 0; i < ch.CurrentRoom.Players.Count; i++) {
            var player = ch.CurrentRoom.Players[i];
            if (player.Name === ch.Name) { player.SendLine("{GYou say, '{g"+ args.join(' ') +"{G'{x"); }
            else { player.SendLine("{g"+ ch.Name + " says, '{G" + args.join(' ') + "{g'{x"); }

        }
        
    }

    return 0;
}
