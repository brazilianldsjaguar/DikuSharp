using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DikuSharp.Server.Characters;

namespace DikuSharp.Server.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Inspired by https://www.dotnetperls.com/uppercase-first-letter.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string source)
        {
            char[] array = source.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
    }
}
