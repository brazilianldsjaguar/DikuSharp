using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DikuSharp.Server.Colors
{
    public class Color
    {
        public char Tag { get; set; }
        public string Code { get; set; }
        public bool IsBold { get; set; }

        public Color( char tag, string code, bool isBold )
        {
            this.Tag = tag;
            this.Code = code;
            this.IsBold = isBold;
        }

        public static string Reset = "\x1B[0m";
        public static string Bold = "\x1B[1m";
        public static string Italics = "\x1B[3m";
        public static string Underline = "\x1B[4m";
        public static string Inverse = "\x1B[7m";
        public static string StrikeThrough = "\x1B[9m";
        public static string BoldOff = "\x1B[22m";
        public static string ItalicsOff = "\x1B[23m";
        public static string UnderlineOff = "\x1B[24m";
        public static string InverseOff = "\x1B[27m";
        public static string StrikeThroughOff = "\x1B[29m";
        public static string Black = "\x1B[30m";
        public static string Red = "\x1B[31m";
        public static string Green = "\x1B[32m";
        public static string Yellow = "\x1B[33m";
        public static string Blue = "\x1B[34m";
        public static string Magenta = "\x1B[35m";
        public static string Cyan = "\x1B[36m";
        public static string White = "\x1B[37m";
        public static string Default = "\x1B[39m";
        public static string BackgroundBlack = "\x1B[40m";
        public static string BackgroundRed = "\x1B[41m";
        public static string BackgroundGreen = "\x1B[42m";
        public static string BackgroundYellow = "\x1B[43m";
        public static string BackgroundBlue = "\x1B[44m";
        public static string BackgroundMagenta = "\x1B[45m";
        public static string BackgroundCyan = "\x1B[46m";
        public static string BackgroundWhite = "\x1B[47m";
        public static string BackgroundDefault = "\x1B[49m";
    }
}
