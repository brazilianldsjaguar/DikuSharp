function score(ch, args) {

          ch.SendLine(".------ DIKU SHARP ------.");
    ch.SendFormatLine("| {{W{0,-22} {{w|", ch.Name);
          ch.SendLine("+------------------------+");
    ch.SendFormatLine("| Strength:     {0,-2}       |", ch.Str);
    ch.SendFormatLine("| Dexterity:    {0,-2}       |", ch.Dex);
    ch.SendFormatLine("| Intelligence: {0,-2}       |", ch.Int);
    ch.SendFormatLine("| Charisma:     {0,-2}       |", ch.Cha);
    ch.SendLine("'------------------------'");

    return 0;
}
