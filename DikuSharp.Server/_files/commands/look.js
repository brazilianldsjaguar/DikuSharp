function look(ch, args) {
    if ( args.length > 0 ) {
        //try and figure out what they're looking at
        var target = args[0].toLowerCase();
        var found = false;
        //players first...
        for (var i = 0; i < ch.CurrentRoom.Players.Count; i++)
        {
            var player = ch.CurrentRoom.Players[i];
            if(player.Name.toLowerCase().indexOf(target) == 0)
            {
                //match!
                ch.SendLine(player.Description);
                return 0;
            }
        }

        //then mobs...
        for (var i = 0; i < ch.CurrentRoom.Mobs.Count; i++) {
            var mob = ch.CurrentRoom.Mobs[i];
            //mobs can have various 'keyword' names
            var names = mob.Name.split(' ');
            for (var n = 0; n < names.length; n++)
            {
                var name = names[n];
                if (name.toLowerCase().indexOf(target) == 0) {
                    ch.SendLine(mob.Description);
                    return 0;
                }
            }
        }

        //then objects in the room...

        //then objects in their inventory...

        //if we didn't find anything...
        ch.SendLine("You can't see that here.");
    } else {
        var roomName = ch.CurrentRoom.Name;
        if (roomName[0] != '{') {
            roomName = '{C' + roomName;
        }
        ch.SendLine(roomName);
        ch.SendLine("  " + ch.CurrentRoom.Description);
        ch.SendLine("");
        ch.SendLine("{W[{DExits{W]");

        var exitEnumerator = ch.CurrentRoom.Exits.GetEnumerator();
        var exitString = '';
        while (exitEnumerator.MoveNext())
        {
            exitString += ' ' + exitEnumerator.Current.Key;
        }
        ch.SendLine(exitString);        
        for (var i = 0; i < ch.CurrentRoom.Players.Count; i++)
        {
            var player = ch.CurrentRoom.Players[i];
            if (player.Name != ch.Name) {
                ch.SendLine(player.ShortDescription);
            }
        }
        for (var i = 0; i < ch.CurrentRoom.Mobs.Count; i++) {
            var mob = ch.CurrentRoom.Mobs[i];
            ch.SendLine(mob.ShortDescription);
        }
    }
    return 0;
}
